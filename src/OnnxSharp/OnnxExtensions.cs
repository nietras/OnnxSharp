using Google.Protobuf.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Onnx
{
    /// <summary>
    /// Extensions to ONNX protobuf functionality.
    /// </summary>
    public static partial class OnnxExtensions
    {
        // TODO: Having these overloads makes things less scalable and cumbersome, remove?
        /// <summary>
        /// Remove initializers from inputs of graph in model.
        /// </summary>
        public static ModelProto RemoveInitializersFromInputs(this ModelProto model)
        {
            RemoveInitializersFromInputs(model.Graph);
            return model;
        }

        /// <summary>
        /// Remove initializers from inputs of graph.
        /// </summary>
        // https://github.com/microsoft/onnxruntime/blob/master/tools/python/remove_initializer_from_input.py
        public static GraphProto RemoveInitializersFromInputs(GraphProto graph)
        {
            var inputs = graph.Input;
            var nameToInput = inputs.ToDictionary(i => i.Name, i => i);

            foreach (var initializer in graph.Initializer)
            {
                if (nameToInput.TryGetValue(initializer.Name, out var input))
                {
                    inputs.Remove(input);
                }
            }

            return graph;
        }

        // TODO: Having these overloads makes things less scalable and cumbersome, remove?
        /// <summary>
        /// Remove unnecessary initializer reshapes from graph.
        /// </summary>
        public static ModelProto RemoveUnnecessaryInitializerReshapes(this ModelProto model)
        {
            RemoveUnnecessaryInitializerReshapes(model.Graph);
            return model;
        }

        /// <summary>
        /// Remove unnecessary initializer reshapes from graph.
        /// </summary>
        // https://github.com/microsoft/onnxruntime/blob/master/tools/python/remove_initializer_from_input.py
        public static GraphProto RemoveUnnecessaryInitializerReshapes(GraphProto graph)
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
                        var outputName = outputs[0]; // To use to rename input to next node

                        if (nameToInitializer.TryGetValue(dataName, out var dataInitializer) &&
                            nameToInitializer.TryGetValue(shapeName, out var shapeInitializer))
                        {
                            // TODO: Check initializer not used in other nodes

                            var outputShapeValue = graph.ValueInfo.Where(v => v.Name.Equals(outputName)).Single();

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
                                        var dataInitializerIndex = graph.Initializer.IndexOf(dataInitializer);
                                        var newDataInitializer = new TensorProto(dataInitializer, outputShape);
                                        graph.Initializer.RemoveAt(dataInitializerIndex);
                                        graph.Initializer.Insert(dataInitializerIndex, newDataInitializer);
                                        //dataInitializer.D = outputShapeDims;
                                        // Use outputShape as the new shape

                                        // Remove reshape data e.g. shape
                                        TryRemove(graph.Initializer, i => i.Name == shapeName);

                                        nodesToRemove.Add(node);

                                        // Replace reshape output name with data name directly in all nodes
                                        for (int updateNodeIndex = 0; updateNodeIndex < nodes.Count; updateNodeIndex++)
                                        {
                                            var updateNodeInputs = node.Input;
                                            //inputs.
                                        }
                                    }

                                }
                            }
                            //outputShape.Di

                            // Get reshape shape as "Dims" comparable to data
                            //var shapeDims = GetShape(shapeInitializer);

                            // Find nodes that use the outputName in inputs (and replace in that)

                            // 
                        }
                    }
                }
            }
            foreach (var node in nodesToRemove)
            {
                nodes.Remove(node);
            }

            return graph;
        }

        // TODO: Make non-allocating version
        static bool TryRemove<T>(RepeatedField<T> fields, Predicate<T> predicate)
        {
            for (int i = 0; i < fields.Count; i++)
            {
                var field = fields[i];
                if (predicate(field))
                {
                    fields.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        static long[] GetData1D(TensorProto tensor)
        {
            return null;
        }
    }
}
