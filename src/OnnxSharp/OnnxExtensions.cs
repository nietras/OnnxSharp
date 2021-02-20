using Google.Protobuf.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Onnx
{
    /// <summary>
    /// Extensions to ONNX protobuf functionality.
    /// </summary>
    public static partial class OnnxExtensions
    {
        /// <summary>
        /// Remove initializers from inputs of graph.
        /// </summary>
        // https://github.com/microsoft/onnxruntime/blob/master/tools/python/remove_initializer_from_input.py
        public static void RemoveInitializersFromInputs(this GraphProto graph)
        {
            var inputs = graph.Input;
            var nameToInput = inputs.ToDictionary(i => i.Name, i => i);

            foreach (var initializer in graph.Initializer)
            {
                if (nameToInput.TryGetValue(initializer.Name, out var input))
                {
                    // https://github.com/protocolbuffers/protobuf/blob/master/csharp/src/Google.Protobuf/Collections/RepeatedField.cs
                    var removed = inputs.Remove(input);
                    Trace.WriteLine($"{removed} {inputs.Count}");
                }
            }
        }

        /// <summary>
        /// Remove unnecessary initializer reshapes from graph.
        /// </summary>
        // https://github.com/microsoft/onnxruntime/blob/master/tools/python/remove_initializer_from_input.py
        public static void RemoveUnnecessaryInitializerReshapes(this GraphProto graph)
        {
            var nameToInitializer = graph.Initializer.ToDictionary(i => i.Name, i => i);

            var nodesToRemove = new List<NodeProto>();
            var nodes = graph.Node;
            for (int nodeIndex = 0; nodeIndex < nodes.Count; nodeIndex++)
            {
                var node = nodes[nodeIndex];

                if (node.OpType == "Reshape")
                {
                    var inputs = node.Input;
                    var outputs = node.Output;
                    // Expected Reshape takes two inputs
                    if (inputs.Count == 2 && outputs.Count == 1) 
                    {
                        var dataName = inputs[0];
                        var shapeName = inputs[1];
                        var reshapeOutputName = outputs[0]; // To use to rename input to next node

                        if (nameToInitializer.TryGetValue(dataName, out var dataInitializer) &&
                            nameToInitializer.TryGetValue(shapeName, out var shapeInitializer))
                        {
                            // TODO: Check initializer not used in other nodes

                            var outputShapeValue = graph.ValueInfo.Where(v => v.Name.Equals(reshapeOutputName)).Single();

                            var outputShapeDims = outputShapeValue.Type.TensorType.Shape.Dim;
                            var allValue = outputShapeDims.All(d => d.ValueCase == TensorShapeProto.Types.Dimension.ValueOneofCase.DimValue);
                            if (allValue)
                            {
                                var outputShape = outputShapeDims.Select(d => d.DimValue).ToArray();
                                var allPositive = outputShape.All(d => d > 0);
                                if (allPositive)
                                {
                                    // Check shape compared to initializer shape
                                    var dataShape = dataInitializer.Dims.ToArray();

                                    var outputShapeProductSum = outputShape.Aggregate(1L, (s, i) => s * i);
                                    var dataShapeProductSum = dataShape.Aggregate(1L, (s, i) => s * i);

                                    if (outputShapeProductSum == dataShapeProductSum)
                                    {
                                        dataInitializer.Dims.Clear();
                                        dataInitializer.Dims.AddRange(outputShape);

                                        // Remove reshape data shape both as initializer and input
                                        TryRemove(graph.Initializer, i => i.Name, shapeName);
                                        TryRemove(graph.Input, i => i.Name, shapeName);

                                        nodesToRemove.Add(node);

                                        // Replace reshape output name with data name directly in all nodes
                                        for (int updateNodeIndex = 0; updateNodeIndex < nodes.Count; updateNodeIndex++)
                                        {
                                            var updateNodeInputs = nodes[updateNodeIndex].Input;
                                            for (int inputIndex = 0; inputIndex < updateNodeInputs.Count; inputIndex++)
                                            {
                                                if (updateNodeInputs[inputIndex] == reshapeOutputName)
                                                {
                                                    updateNodeInputs[inputIndex] = dataName;
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
            }
            foreach (var node in nodesToRemove)
            {
                nodes.Remove(node);
            }
        }

        static bool TryRemove<T, TSelect>(RepeatedField<T> fields, Func<T, TSelect> select, Predicate<TSelect> predicate)
        {
            for (int i = 0; i < fields.Count; i++)
            {
                var field = fields[i];
                var value = select(field);
                if (predicate(value))
                {
                    fields.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        static bool TryRemove<T, TSelect>(RepeatedField<T> fields, Func<T, TSelect> select, TSelect valueToRemove)
            where TSelect : IEquatable<TSelect>
        {
            for (int i = 0; i < fields.Count; i++)
            {
                var field = fields[i];
                var value = select(field);
                if (value.Equals(valueToRemove))
                {
                    fields.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }
    }
}
