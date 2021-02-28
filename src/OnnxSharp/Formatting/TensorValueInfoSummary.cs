using System;
using System.Collections.Generic;
using System.Linq;

namespace Onnx.Formatting
{
    internal class TensorValueInfoSummary
    {
        internal static readonly IReadOnlyList<string> ColumnNames =
            new string[] { "Name", "Type", "ElemType", "Shape", "SizeInFile" };

        internal static readonly IReadOnlyList<Align> ColumnAligns =
            new Align[] { Align.Left, Align.Left, Align.Left, Align.Right, Align.Right };

        internal static readonly IReadOnlyList<Func<ValueInfoProto, string>> ColumnGetters =
            new Func<ValueInfoProto, string>[]
            {
                i => i.Name,
                i => i.Type.ValueCase.ToString(),
                i => i.Type.TensorType.ElemType().ToString(),
                i => FormatShape(i.Type.TensorType.Shape),
                i => i.CalculateSize().ToString(),
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
