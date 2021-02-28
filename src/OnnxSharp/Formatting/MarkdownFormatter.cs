using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Onnx.Formatting
{
    internal static class MarkdownFormatter
    {
        internal static void Format(this IReadOnlyList<ValueInfoSummary> summaries, TextWriter writer)
        {
            Format(summaries,
                ValueInfoSummary.ColumnNames, ValueInfoSummary.ColumnAligns, ValueInfoSummary.ColumnGetters,
                writer);
        }

        internal static void Format(this IReadOnlyList<TensorSummary> summaries, TextWriter writer)
        {
            Format(summaries, 
                TensorSummary.ColumnNames, TensorSummary.ColumnAligns, TensorSummary.ColumnGetters, 
                writer);
        }

        internal static void Format<T>(
            IReadOnlyList<T> values,
            IReadOnlyList<string> columnNames,
            IReadOnlyList<Align> columnAligns, 
            IReadOnlyList<Func<T, string>> columnGetters, 
            TextWriter writer)
        {
            var maxColumnWidth = columnNames.Select(n => n.Length).ToArray();

            int rows = values.Count;
            int cols = columnNames.Count;

            var table = new string[rows, cols];
            for (int row = 0; row < rows; row++)
            {
                var summary = values[row];

                for (int col = 0; col < cols; col++)
                {
                    var getter = columnGetters[col];
                    var text = getter(summary);
                    table[row, col] = text;
                    maxColumnWidth[col] = Math.Max(maxColumnWidth[col], text.Length);
                }
            }

            Format(table, columnNames, columnAligns, maxColumnWidth, writer);
        }

        internal static void Format(
            string[,] table,
            IReadOnlyList<string> columnNames,
            IReadOnlyList<Align> columnAligns,
            IReadOnlyList<int> columnWidths,
            TextWriter writer)
        {
            // TODO: Define constants below
            
            var rows = table.GetLength(0);
            var cols = table.GetLength(1);

            // Column Names
            for (int col = 0; col < cols; col++)
            {
                var columnName = columnNames[col];
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
                var alignment = columnAligns[col];
                if (alignment == Align.Left)
                {
                    writer.Write(':');
                }
                writer.Write('-', columnWidths[col] - 1);
                if (alignment == Align.Right)
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
                    var value = table[row, col];
                    writer.Write('|');
                    writer.WriteAligned(value, columnAligns[col], ' ', columnWidths[col]);
                }
                writer.Write('|');
                writer.WriteLine();
            }
        }
    }
}
