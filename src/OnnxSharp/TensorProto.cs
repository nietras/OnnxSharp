using Google.Protobuf.Collections;

namespace Onnx
{
    public sealed partial class TensorProto
    {
        // What other ways are there to create or manipulate TensorProto??
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        internal TensorProto(TensorProto other, long[] newDims) 
            : this(other)
        {
            var dims = new RepeatedField<long>();
            for (int i = 0; i < newDims.Length; i++)
            {
                dims.Add(newDims[i]);
            }
            dims_ = dims;
        }
    }
}
