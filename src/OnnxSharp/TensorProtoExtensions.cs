namespace Onnx
{
    /// <summary>Convenience <see cref="TensorProto"/> extension methods.</summary>
    public static class TensorProtoExtensions
    {
        /// <summary>Get data type of <paramref name="tensor"/> as enum.</summary>
        public static TensorProto.Types.DataType DataType(this TensorProto tensor) =>
            (TensorProto.Types.DataType)tensor.DataType;
    }
}
