using System;
using System.Collections.Generic;
using Onnx.Collections;

namespace Onnx.Formatting
{
    internal class TensorSummary
    {
        internal static readonly IReadOnlyList<string> ColumnNames = 
            new string[] { "Name", "DataType", "Dims", "Π Dims", "SizeInFile" };

        internal static readonly IReadOnlyList<Align> ColumnAligns =
            new Align[] { Align.Left, Align.Left, Align.Right, Align.Right, Align.Right };

        internal static readonly IReadOnlyList<Func<TensorSummary, string>> ColumnGetters =
            new Func<TensorSummary, string>[]
            {  
                summary => summary.Name,
                summary => summary.DataType.ToString(),
                summary => string.Join("x", summary.Dims),
                summary => summary.DimsProduct.ToString(),
                summary => summary.SizeInFile.ToString(),
            };

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
}
