using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Onnx.Formatting
{
    internal static class SummaryExtensions
    {
        public static ValueInfoSummary Summary(this ValueInfoProto valueInfo) =>
            new ValueInfoSummary(valueInfo.Name, valueInfo.Type.ValueCase, 
                valueInfo.CalculateSize());

        public static TensorSummary Summary(this TensorProto tensor) =>
            new TensorSummary(tensor.Name, tensor.DataType(), tensor.Dims.ToArray(), 
                tensor.CalculateSize());
    }
}
