using System;
using System.Collections.Generic;

namespace Onnx.Formatting
{
    internal class ValueInfoSummary
    {
        internal static readonly IReadOnlyList<string> ColumnNames =
            new string[] { "Name", "Type", "SizeInFile" };

        internal static readonly IReadOnlyList<Align> ColumnAligns =
            new Align[] { Align.Left, Align.Left, Align.Right };

        internal static readonly IReadOnlyList<Func<ValueInfoSummary, string>> ColumnGetters =
            new Func<ValueInfoSummary, string>[]
            {
                summary => summary.Name,
                summary => summary.Type.ToString(),
                summary => summary.SizeInFile.ToString(),
            };

        public ValueInfoSummary(string name, TypeProto.ValueOneofCase type, int sizeInFile)
        {
            Name = name;
            Type = type;
            SizeInFile = sizeInFile;
        }

        public string Name { get; }
        public TypeProto.ValueOneofCase Type { get; }
        public int SizeInFile { get; }
    }
}
