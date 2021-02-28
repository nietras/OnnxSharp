using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Onnx.Formatting
{
    internal static class MarkdownFormatter
    {
        internal static void FormatAsTensors(this IReadOnlyList<ValueInfoProto> valueInfos, TextWriter writer)
        {
            Format(valueInfos, ColumnSpecs.ValueInfo.Tensor, writer);
        }

        internal static void FormatAsSequences(this IReadOnlyList<ValueInfoProto> valueInfos, TextWriter writer)
        {
            Format(valueInfos, ColumnSpecs.ValueInfo.Sequence, writer);
        }

        internal static void Format(this IReadOnlyList<TensorProto> summaries, TextWriter writer)
        {
            Format(summaries, ColumnSpecs.Tensor, writer);
        }

        internal static void Format<T>(
            IReadOnlyList<T> values,
            IReadOnlyList<ColumnSpec<T>> columnSpecs,
            TextWriter writer)
        {
            var maxColumnWidth = columnSpecs.Select(n => n.Name.Length).ToArray();

            int rows = values.Count;
            int cols = columnSpecs.Count;

            var table = new string[rows, cols];
            for (int row = 0; row < rows; row++)
            {
                var summary = values[row];

                for (int col = 0; col < cols; col++)
                {
                    var spec = columnSpecs[col];
                    var text = spec.Get(summary);
                    table[row, col] = text;
                    maxColumnWidth[col] = Math.Max(maxColumnWidth[col], text.Length);
                }
            }

            Format(table, columnSpecs, maxColumnWidth, writer);
        }

        internal static void Format(
            string[,] table,
            IReadOnlyList<ColumnSpec> columnSpecs,
            IReadOnlyList<int> columnWidths,
            TextWriter writer)
        {
            // TODO: Define constants below
            
            var rows = table.GetLength(0);
            var cols = table.GetLength(1);

            // Column Names
            for (int col = 0; col < cols; col++)
            {
                var columnName = columnSpecs[col].Name;
                writer.Write('|');
                writer.Write(columnName);
                writer.Write(' ', columnWidths[col] - columnName.Length);
            }
            writer.Write('|');
            writer.WriteLine();

            // Separator and alignment
            for (int col = 0; col < cols; col++)
            {
                writer.Write('|');
                var align = columnSpecs[col].Align;
                if (align == Align.Left)
                {
                    writer.Write(':');
                }
                writer.Write('-', columnWidths[col] - 1);
                if (align == Align.Right)
                {
                    writer.Write(':');
                }
            }
            writer.Write('|');
            writer.WriteLine();
            
            // Rows
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    var align = columnSpecs[col].Align;
                    var value = table[row, col];
                    writer.Write('|');
                    writer.WriteAligned(value, align, ' ', columnWidths[col]);
                }
                writer.Write('|');
                writer.WriteLine();
            }
        }
    }
}
