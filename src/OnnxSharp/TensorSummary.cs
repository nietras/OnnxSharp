namespace Onnx
{
    public class TensorSummary
    {
        public TensorSummary(string name, TensorProto.Types.DataType dataType, long[] dims, int sizeInFile)
        {
            Name = name;
            DataType = dataType;
            Dims = dims;
            SizeInFile = sizeInFile;
        }

        public string Name { get; }
        public TensorProto.Types.DataType DataType { get; }
        public long[] Dims { get; }
        public int SizeInFile { get; }
    }
}
