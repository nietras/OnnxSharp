using System;
using System.Collections.Generic;
using System.Linq;

namespace Onnx.Formatting
{
    internal class SequenceValueInfoSummary
    {
        internal static readonly IReadOnlyList<ColumnSpec<ValueInfoProto>> ColumnSpecs = 
            new ColumnSpec<ValueInfoProto>[] 
            { 
                new ("Name",       Align.Left, i => i.Name),
                new ("Type",       Align.Left, i => i.Type.ValueCase.ToString()),
                new ("ElemType",   Align.Left, i => i.Type.SequenceType.ElemType.ValueCase.ToString()),
                new ("SizeInFile", Align.Left, i => i.CalculateSize().ToString()),
            };

        internal static readonly IReadOnlyList<string> ColumnNames =
            new string[] { "Name", "Type", "ElemType", "SizeInFile" };

        internal static readonly IReadOnlyList<Align> ColumnAligns =
            new Align[] { Align.Left, Align.Left, Align.Left, Align.Right, Align.Right };

        internal static readonly IReadOnlyList<Func<ValueInfoProto, string>> ColumnGetters =
            new Func<ValueInfoProto, string>[]
            {
                i => i.Name,
                i => i.Type.ValueCase.ToString(),
                i => i.Type.SequenceType.ElemType.ValueCase.ToString(),
                i => i.CalculateSize().ToString(),
            };
    }
}

