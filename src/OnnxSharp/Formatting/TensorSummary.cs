using System;
using System.Collections.Generic;
using Onnx.Collections;

namespace Onnx.Formatting
{
    internal class TensorSummary
    {
        internal static readonly IReadOnlyList<ColumnSpec<TensorProto>> ColumnSpecs =
            new ColumnSpec<TensorProto>[]
            {
                new ("Name",       Align.Left,  t => t.Name),
                new ("DataType",   Align.Left,  t => t.DataType().ToString()),
                new ("Dims",       Align.Right, t => string.Join("x", t.Dims)),
                new ("Π(Dims)",    Align.Right, t => t.Dims.Product().ToString()),
                new ("SizeInFile", Align.Right, t => t.CalculateSize().ToString()),
            };
    }
}
