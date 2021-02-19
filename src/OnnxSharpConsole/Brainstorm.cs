using Google.Protobuf;
using Onnx;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace OnnxSharpConsole
{
    public static class Brainstorm
    {
        public static GraphProto SetDim(GraphProto graph)
        {
            // Shapes are defined in inputs, values and outputs
            var inputs = graph.Input;
            var values = graph.ValueInfo;
            var nodes = graph.Node;
            var initializers = graph.Initializer;
            var outputs = graph.Output;

            var allValues = inputs.Concat(values).Concat(outputs).ToArray();

            //var namesToIgnore = new HashSet<string>();
            // Only fix reshapes that have data input with dynamic shape after

            var reshapes = nodes.Where(n => n.OpType == "Reshape").ToArray();
            foreach (var reshape in reshapes)
            {
                // Reshape has two inputs (data and shape)
                // Second hence is the shape
                var shapeInputName = reshape.Input[1];
                var shape = initializers.Single(v => v.Name == shapeInputName);

                Debug.Assert(shape.DataType == (int)TensorProto.Types.DataType.Int64);
                var dims = shape.Dims;
                if (dims.Count > 0 && dims[0] > 0)
                {
                    // Data may be stored as Int64 or Raw (fixed-width, little-endian)
                    if (shape.Int64Data.Count > 0)
                    {
                        var int64Data = shape.Int64Data;
                        if (int64Data[0] == 1) // Dimension we replace
                        {
                            int64Data[0] = -1;
                        }
                    }
                    if (!shape.RawData.IsEmpty)
                    {
                        var rawData = shape.RawData;
                        var rawAsInt64Data = MemoryMarshal.Cast<byte, long>(rawData.Span);
                        Debug.Assert(rawAsInt64Data.Length == dims[0]);
                        if (rawAsInt64Data[0] == 1) // Dimension we replace
                        {
                            var newShape = rawAsInt64Data.ToArray();
                            newShape[0] = -1;
                            shape.RawData = ByteString.CopyFrom(MemoryMarshal.Cast<long, byte>(newShape.AsSpan()));
                        }
                    }
                    else
                    {
                        // FAILS IF ADDING
                        //foreach (var output in reshape.Output)
                        //{
                        //    namesToIgnore.Add(output);
                        //}
                    }
                }
                // 
                //log(reshape.ToString());
            }

            foreach (var value in allValues)
            {
                //var denotation = value.Type.Denotation;
                //if (!namesToIgnore.Contains(value.Name))
                {
                    var shape = value.Type.TensorType.Shape;
                    var dims = shape.Dim;
                    var dim = dims[0];
                    dim.ClearValue();
                    //dim.DimValue = -1; // Or don't set it
                    dim.DimParam = "N";
                }
                //else
                //{
                //    // FAILS IF WE DO NOT CHANGE OUTPUT OF THE ONE RESHAPE
                //    // Reshape
                //    log(value.ToString());
                //}
            }

            return graph;
        }
    }
}
