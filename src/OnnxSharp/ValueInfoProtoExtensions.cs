namespace Onnx
{
    /// <summary>Convenience <see cref="ValueInfoProto"/> extension methods.</summary>
    public static class ValueInfoProtoExtensions
    {
        ///// <summary>Get <see cref="TypeProto"/> of value of <paramref name="tensor"/> as enum.</summary>
        //public static TypeProto ValueType(this ValueInfoProto valueInfo) => valueInfo.Type.ValueCase switch
        //    {
        //        TypeProto.ValueOneofCase.TensorType => valueInfo.Type.TensorType,
        //        TypeProto.ValueOneofCase.TensorType => valueInfo.Type.MapType,
        //    };
    }
}
