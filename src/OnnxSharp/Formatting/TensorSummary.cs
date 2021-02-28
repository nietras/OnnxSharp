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

        internal static readonly IReadOnlyList<Func<TensorProto, string>> ColumnGetters =
            new Func<TensorProto, string>[]
            {  
                t => t.Name,
                t => t.DataType().ToString(),
                t => string.Join("x", t.Dims),
                t => t.Dims.Product().ToString(),
                t => t.CalculateSize().ToString(),
            };
    }
}
