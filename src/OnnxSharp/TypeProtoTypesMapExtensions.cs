namespace Onnx
{
    /// <summary>Convenience <see cref="TypeProto.Types.Map"/> extension methods.</summary>
    public static class TypeProtoTypesMapExtensions
    {
        /// <summary>Get key data type of <paramref name="map"/> as enum.</summary>
        public static TensorProto.Types.DataType KeyType(this TypeProto.Types.Map map) =>
            (TensorProto.Types.DataType)map.KeyType;
    }
}
