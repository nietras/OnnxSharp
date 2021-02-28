using System;
using System.Collections.Generic;
using System.Linq;

namespace Onnx.Formatting
{
    internal class TensorValueInfoSummary
    {
        internal static readonly IReadOnlyList<ColumnSpec<ValueInfoProto>> ColumnSpecs =
            new ColumnSpec<ValueInfoProto>[]
            {
                new ("Name",       Align.Left,  i => i.Name),
                new ("Type",       Align.Left,  i => i.Type.ValueCase.ToString()),
                new ("ElemType",   Align.Left,  i => i.Type.TensorType.ElemType().ToString()),
                new ("Shape",      Align.Right, i => FormatShape(i.Type.TensorType.Shape)),
                new ("SizeInFile", Align.Right, i => i.CalculateSize().ToString()),
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
