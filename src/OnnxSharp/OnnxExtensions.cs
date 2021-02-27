using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Google.Protobuf;
using Google.Protobuf.Collections;

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

                                    var outputShapeProductSum = outputShape.ProductSum();
                                    var dataShapeProductSum = dataShape.ProductSum();

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

        /// <summary>
        /// Set dimension of inputs, value infos, outputs and potential Reshape ops.
        /// Default sets leading dimension to dynamic batch size 'N'.
        /// </summary>
        public static void SetDim(this GraphProto graph) =>
            graph.SetDim(dimIndex: 0, DimParamOrValue.NewParam("N"));

        /// <summary>
        /// Set dimension of inputs, value infos, outputs and potential Reshape ops.
        /// Can be used to make models have dynamic batch size or different static batch sizes.
        /// </summary>
        public static void SetDim(this GraphProto graph, int dimIndex, DimParamOrValue dimParamOrValue)
        {
            // Reshape ops have their "new shape" defined as input to the reshape op.
            // This input needs to be changed to reflect new dim e.g. be set -1 if dynamic.
            var reshapeDimValue = dimParamOrValue.IsParam 
                ? Ops.Reshape.DynamicReshapeValue 
                : dimParamOrValue.Value;
            SetDimInReshapes(graph, dimIndex, reshapeDimValue);

            // Should we set this based on nodes instead? Handling input, outputs based on that?

            // Shapes are defined in inputs, valueInfos and outputs
            //
            // Only real inputs should be changed, not "initializer" inputs
            var initializserNames = new HashSet<string>(graph.Initializer.Select(i => i.Name));
            var inferenceInputs = graph.Input.Where(i => !initializserNames.Contains(i.Name));
            foreach (var input in inferenceInputs)
            {
                SetDim(input, dimIndex, dimParamOrValue);
            }
            //SetDim(graph.Input, dimIndex, dimParam);

            SetDim(graph.ValueInfo, dimIndex, dimParamOrValue);
            SetDim(graph.Output, dimIndex, dimParamOrValue);
        }

        static void SetDimInReshapes(GraphProto graph, int dimIndex, int dimValue)
        {
            var nodes = graph.Node;
            var initializers = graph.Initializer;

            // TODO: Only fix reshapes that have data input and with dynamic shape after

            var opSpec = Ops.Reshape.Spec;
            foreach (var node in nodes)
            {
                if (node.OpType == opSpec.OpType)
                {
                    var dataInputName = node.Input[Ops.Reshape.InputDataIndex];

                    // Check if data input is an initializer if so we should not change the reshape
                    // and hence skip this reshape node
                    var dataInitializerIndex = initializers.IndexOf(t => t.Name, dataInputName);
                    if (dataInitializerIndex >= 0)
                    { continue; }

                    var shapeInputName = node.Input[Ops.Reshape.InputShapeIndex];

                    var shape = initializers.Single(tensor => tensor.Name, shapeInputName);

                    SetDimInReshapeTensorShape(shape, dimIndex, dimValue);
                }
            }
        }

        static void SetDimInReshapeTensorShape(TensorProto shape, int dimIndex, int dimValue)
        {
            Debug.Assert(shape.DataType == (int)TensorProto.Types.DataType.Int64);
            var dims = shape.Dims;
            if (dims.Count > 0 && dims[dimIndex] > 0)
            {
                // Data may be stored as Int64 or Raw (fixed-width, little-endian)
                if (shape.Int64Data.Count > 0)
                {
                    var int64Data = shape.Int64Data;
                    if (int64Data[dimIndex] == 1) // Dimension we replace
                    {
                        int64Data[dimIndex] = dimValue;
                    }
                }
                if (!shape.RawData.IsEmpty)
                {
                    var rawData = shape.RawData;
                    var rawAsInt64Data = MemoryMarshal.Cast<byte, long>(rawData.Span);
                    Debug.Assert(rawAsInt64Data.Length == dims[dimIndex]);
                    if (rawAsInt64Data[dimIndex] == 1) // Dimension we replace
                    {
                        var newShape = rawAsInt64Data.ToArray();
                        newShape[dimIndex] = dimValue;
                        var newShapeBytes = MemoryMarshal.Cast<long, byte>(newShape.AsSpan());
                        shape.RawData = ByteString.CopyFrom(newShapeBytes);
                    }
                }
            }
        }

        internal static void SetDim(RepeatedField<ValueInfoProto> valueInfos, 
            int dimIndex, DimParamOrValue dimParamOrValue)
        {
            for (int i = 0; i < valueInfos.Count; i++)
            {
                var valueInfo = valueInfos[i];
                SetDim(valueInfo, dimIndex, dimParamOrValue);
            }
        }

        internal static void SetDim(ValueInfoProto valueInfo, 
            int dimIndex, DimParamOrValue dimParamOrValue)
        {
            var shape = valueInfo.Type.TensorType.Shape;
            var dims = shape.Dim;
            var dim = dims[dimIndex];
            if (dim.ValueCase == TensorShapeProto.Types.Dimension.ValueOneofCase.DimValue)
            {
                // TODO: Should perhaps be parameter that says 
                //       bool shouldSetDimFor(dim)
                if (dim.DimValue == 1)
                {
                    SetDim(dim, dimParamOrValue);
                }
            }
        }

        internal static void SetDim(TensorShapeProto.Types.Dimension dim, 
            DimParamOrValue dimParamOrValue)
        {
            dim.ClearValue();
            if (dimParamOrValue.IsParam)
            {
                dim.DimParam = dimParamOrValue.Param;
            }
            else
            {
                dim.DimValue = dimParamOrValue.Value;
            }
        }
    }
}
