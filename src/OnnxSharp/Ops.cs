using System;

namespace Onnx
{
    internal static class Ops
    {
        internal static class Reshape
        {
            internal const int InputDataIndex = 0;
            internal const int InputShapeIndex = 1;

            // Reshape op supports only one dimension in shape to be dynamic,
            // which is defined as -1.
            internal const int DynamicReshapeValue = -1;

            internal static readonly OpSpec Spec = new OpSpec(nameof(Reshape), 2, 1);
        }

        internal readonly struct OpSpec
        {
            public OpSpec(string opType, int inputs, int outputs)
            {
                OpType = opType ?? throw new ArgumentNullException(nameof(opType));
                Inputs = inputs;
                Outputs = outputs;
            }

            public string OpType { get; }
            public int Inputs { get; }
            public int Outputs { get; }
        }
    }
}
