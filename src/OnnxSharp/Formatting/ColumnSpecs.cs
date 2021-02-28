using System;
using System.Collections.Generic;
using System.Linq;
using Onnx.Collections;

namespace Onnx.Formatting
{
    internal static partial class ColumnSpecs
    {
        internal static partial class ValueInfo
        {
            internal static readonly IReadOnlyList<ColumnSpec<ValueInfoProto>> Tensor =
                new ColumnSpec<ValueInfoProto>[]
                {
                new ("Name",       Align.Left,  i => i.Name),
                new ("Type",       Align.Left,  i => i.Type.ValueCase.ToString()),
                new ("ElemType",   Align.Left,  i => i.Type.TensorType.ElemType().ToString()),
                new ("Shape",      Align.Right, i => FormatShape(i.Type.TensorType.Shape)),
                new ("SizeInFile", Align.Right, i => i.CalculateSize().ToString()),
                };

            internal static readonly IReadOnlyList<ColumnSpec<ValueInfoProto>> Sequence =
                new ColumnSpec<ValueInfoProto>[]
                {
                new ("Name",       Align.Left, i => i.Name),
                new ("Type",       Align.Left, i => i.Type.ValueCase.ToString()),
                new ("ElemType",   Align.Left, i => i.Type.SequenceType.ElemType.ValueCase.ToString()),
                new ("SizeInFile", Align.Left, i => i.CalculateSize().ToString()),
                };
        }

        internal static readonly IReadOnlyList<ColumnSpec<TensorProto>> Tensor =
            new ColumnSpec<TensorProto>[]
            {
                new ("Name",       Align.Left,  t => t.Name),
                new ("DataType",   Align.Left,  t => t.DataType().ToString()),
                new ("Dims",       Align.Right, t => string.Join("x", t.Dims)),
                new ("Π(Dims)",    Align.Right, t => t.Dims.Product().ToString()),
                new ("SizeInFile", Align.Right, t => t.CalculateSize().ToString()),
            };

        static string FormatShape(TensorShapeProto shape)
        {
            return string.Join("x", shape.Dim.Select(d => Format(d)));
        }

        static string Format(TensorShapeProto.Types.Dimension d) => d.ValueCase switch
        {
            TensorShapeProto.Types.Dimension.ValueOneofCase.DimParam => d.DimParam,
            TensorShapeProto.Types.Dimension.ValueOneofCase.DimValue => d.DimValue.ToString(),
            TensorShapeProto.Types.Dimension.ValueOneofCase.None => "?",
            _ => throw new NotSupportedException(d.ValueCase.ToString()),
        };
    }
}
