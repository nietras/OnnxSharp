using System.Linq;

namespace Onnx
{
    public static class TensorProtoExtensions
    {
        public static TensorProto.Types.DataType DataType(this TensorProto tensor) =>
            (TensorProto.Types.DataType)tensor.DataType;

        public static TensorSummary Summary(this TensorProto tensor)
        {

            return new TensorSummary(tensor.Name, tensor.DataType(), tensor.Dims.ToArray(), tensor.CalculateSize());
        }
    }
}
