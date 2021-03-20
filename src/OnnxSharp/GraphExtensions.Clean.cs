using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Google.Protobuf.Collections;
using Onnx.Collections;

namespace Onnx
{
    /// <summary>Extension methods to ONNX graph.</summary>
    public static partial class GraphExtensions
    {
        /// <summary>Clean graph for inference.</summary>
        public static void Clean(this GraphProto graph)
        {
            graph.RemoveInitializersFromInputs();
            graph.RemoveUnnecessaryInitializerReshapes();
        }

        /// <summary>Remove initializers from inputs of graph.</summary>
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

        /// <summary>Remove unnecessary initializer reshapes from graph.</summary>
        // https://github.com/microsoft/onnxruntime/blob/master/tools/python/remove_initializer_from_input.py
        public static void RemoveUnnecessaryInitializerReshapes(this GraphProto graph)
        {
            var nameToInitializer = graph.Initializer.ToDictionary(i => i.Name, i => i);

            var nodes = graph.Node;
            var valueInfos = graph.ValueInfo;

            var nodesToRemove = new List<NodeProto>();
            for (int nodeIndex = 0; nodeIndex < nodes.Count; nodeIndex++)
            {
                var node = nodes[nodeIndex];

                var opSpec = Ops.Reshape.Spec;
                if (node.OpType == opSpec.OpType)
                {
                    var inputs = node.Input;
                    var outputs = node.Output;

                    // Expected Reshape takes 2 inputs and has 1 output
                    if (inputs.Count == opSpec.Inputs && outputs.Count == opSpec.Outputs) 
                    {
                        var dataName = inputs[0];
                        var shapeName = inputs[1];
                        var reshapeOutputName = outputs[0];

                        // Both inputs must be initializers ("static")
                        if (nameToInitializer.TryGetValue(dataName, out var dataInitializer) &&
                            nameToInitializer.TryGetValue(shapeName, out var shapeInitializer))
                        {
                            // TODO: Check initializer not used in other nodes

                            var outputShapeValue = valueInfos.Single(v => v.Name, reshapeOutputName);

                            var outputShapeDims = outputShapeValue.Type.TensorType.Shape.Dim;
                            var allValue = outputShapeDims.All(d => d.ValueCase == 
                                TensorShapeProto.Types.Dimension.ValueOneofCase.DimValue);
                            if (allValue)
                            {
                                var outputShape = outputShapeDims.Select(d => d.DimValue).ToArray();

                                var allPositive = outputShape.All(d => d > 0);
                                if (allPositive)
                                {
                                    // Check shape compared to initializer shape
                                    var dataShape = dataInitializer.Dims.ToArray();

                                    var outputShapeProductSum = outputShape.Product();
                                    var dataShapeProductSum = dataShape.Product();

                                    if (outputShapeProductSum == dataShapeProductSum)
                                    {
                                        // Change data shape to the reshape output shape directly
                                        dataInitializer.Dims.Clear();
                                        dataInitializer.Dims.AddRange(outputShape);

                                        // Remove reshape data shape both as initializer and input
                                        graph.Initializer.TryRemove(i => i.Name, shapeName);
                                        graph.Input.TryRemove(i => i.Name, shapeName);

                                        nodesToRemove.Add(node);

                                        // Replace reshape output name with data name directly in all nodes
                                        ReplaceInput(nodes, reshapeOutputName, dataName);
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

        internal static void ReplaceInput(RepeatedField<NodeProto> nodes, string oldValue, string newValue)
        {
            for (int nodeIndex = 0; nodeIndex < nodes.Count; nodeIndex++)
            {
                var updateNodeInputs = nodes[nodeIndex].Input;
                for (int inputIndex = 0; inputIndex < updateNodeInputs.Count; inputIndex++)
                {
                    if (updateNodeInputs[inputIndex] == oldValue)
                    {
                        updateNodeInputs[inputIndex] = newValue;
                    }
                }
            }
        }
    }
}
