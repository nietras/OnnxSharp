namespace Onnx
{
    /// <summary>Convenience <see cref="TypeProto.Types.Tensor"/> extension methods.</summary>
    public static class TypeProtoTypesTensorExtensions
    {
        /// <summary>Get element data type of <paramref name="tensor"/> as enum.</summary>
        public static TensorProto.Types.DataType ElemType(this TypeProto.Types.Tensor tensor) =>
            (TensorProto.Types.DataType)tensor.ElemType;
    }
}
