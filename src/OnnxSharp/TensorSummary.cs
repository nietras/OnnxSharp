using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Onnx
{
    public class TensorSummary
    {
        public TensorSummary(string name, TensorProto.Types.DataType dataType, long[] dims, int sizeInFile)
        {
            Name = name;
            DataType = dataType;
            Dims = dims;
            DimsProduct = dims.Product();
            SizeInFile = sizeInFile;
        }

        public string Name { get; }
        public TensorProto.Types.DataType DataType { get; }
        public long[] Dims { get; }
        public long DimsProduct { get; }
        public int SizeInFile { get; }
    }

    public static class MarkdownFormatter
    {
        public static void Format(this IReadOnlyList<TensorSummary> summaries, StringBuilder builder)
        {
            var columnNames = new string[] { "Name", "DataType", "Dims", "Π Dims", "SizeInFile" };
            var columnAlignment = new Alignment[] { Alignment.Left, Alignment.Left, Alignment.Right, Alignment.Right, Alignment.Right };

            var maxColumnWidth = columnNames.Select(n => n.Length).ToArray();

            int rows = summaries.Count;
            int cols = columnNames.Length;

            var table = new string[rows, cols];
            for (int row = 0; row < rows; row++)
            {
                var summary = summaries[row];

                table[row, 0] = summary.Name;
                table[row, 1] = summary.DataType.ToString();
                table[row, 2] = string.Join("x", summary.Dims);
                table[row, 3] = summary.DimsProduct.ToString();
                table[row, 4] = string.Join("x", summary.SizeInFile);

                for (int col = 0; col < cols; col++)
                {
                    maxColumnWidth[col] = Math.Max(maxColumnWidth[col], table[row, col].Length);
                }
            }

            Format(columnNames, columnAlignment, maxColumnWidth, table, builder);
        }

        internal static void Format(string[] columnNames, Alignment[] columnAlignment, int[] maxColumnWidth, 
            string[,] table, StringBuilder builder)
        {
            // TODO: Define constants below
            
            var rows = table.GetLength(0);
            var cols = table.GetLength(1);

            // Column Names
            for (int col = 0; col < cols; col++)
            {
                var columnName = columnNames[col];
                builder.Append('|');
                builder.Append(columnName);
                builder.Append(' ', maxColumnWidth[col] - columnName.Length);
            }
            builder.Append('|');
            builder.AppendLine();

            // Separator and alignment
            for (int col = 0; col < cols; col++)
            {
                builder.Append('|');
                var alignment = columnAlignment[col];
                if (alignment == Alignment.Left)
                {
                    builder.Append(':');
                }
                builder.Append('-', maxColumnWidth[col] - 1);
                if (alignment == Alignment.Right)
                {
                    builder.Append(':');
                }
            }
            builder.Append('|');
            builder.AppendLine();
            
            // Rows
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    var value = table[row, col];
                    builder.Append('|');
                    builder.AppendAligned(value, columnAlignment[col], ' ', maxColumnWidth[col]);
                }
                builder.Append('|');
                builder.AppendLine();
            }
        }
    }
    internal static class StringBuilderExtensions
    {
        internal static void AppendAligned(this StringBuilder builder, string columnName, Alignment alignment, char pad, int width)
        {
            var padCount = width - columnName.Length;
            if (alignment == Alignment.Right)
            {
                builder.Append(pad, padCount);
            }
            builder.Append(columnName);
            if (alignment == Alignment.Left)
            {
                builder.Append(pad, padCount);
            }
        }
    }
}
