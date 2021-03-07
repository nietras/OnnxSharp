using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Google.Protobuf;
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

            internal static readonly IReadOnlyList<ColumnSpec<ValueInfoProto>> Map =
                new ColumnSpec<ValueInfoProto>[]
                {
                    new ("Name",       Align.Left, i => i.Name),
                    new ("Type",       Align.Left, i => i.Type.ValueCase.ToString()),
                    new ("KeyType",    Align.Left, i => i.Type.MapType.KeyType().ToString()),
                    new ("ValueType",  Align.Left, i => i.Type.MapType.ValueType.ValueCase.ToString()),
                    new ("SizeInFile", Align.Left, i => i.CalculateSize().ToString()),
                };

            internal static readonly IReadOnlyList<ColumnSpec<ValueInfoProto>> None =
                new ColumnSpec<ValueInfoProto>[]
                {
                    new ("Name",       Align.Left, i => i.Name),
                    new ("Type",       Align.Left, i => i.Type.ValueCase.ToString()),
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
                new ("[v0,v1..vN] | (Min,Mean,Max)", Align.Right, t => FormatValuesOrStats(t)),
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

        static unsafe string FormatValuesOrStats(TensorProto tensor) => tensor.DataType() switch
        {
            // NOTE: Long lines accepted below for structure
            TensorProto.Types.DataType.Float => FormatValuesOrStats<float, float>(tensor.FloatData, tensor.RawData, &Math.Min, (m, v) => m + v, (m, c) => m / c, &Math.Max),
            TensorProto.Types.DataType.Double => FormatValuesOrStats<double, double>(tensor.DoubleData, tensor.RawData, &Math.Min, (m, v) => m + v, (m, c) => m / c, &Math.Max),
            TensorProto.Types.DataType.Int32 => FormatValuesOrStats<int, double>(tensor.Int32Data, tensor.RawData, &Math.Min, (m, v) => m + v, (m, c) => m / c, &Math.Max),
            TensorProto.Types.DataType.Int64 => FormatValuesOrStats<long, double>(tensor.Int64Data, tensor.RawData, &Math.Min, (m, v) => m + v, (m, c) => m / c, &Math.Max),
            // TODO: StringData
            _ => "N/A",
        };

        // NOTE: Perf below is not great since function pointer and func calls cannot be inlined.
        //       If necessary refactor to use "value type functor"s.
        static unsafe string FormatValuesOrStats<T, TMean>(
            IReadOnlyList<T> values,
            ByteString rawData,
            delegate*<T, T, T> min,
            Func<TMean, T, TMean> add,
            Func<TMean, int, TMean> divide,
            delegate*<T, T, T> max)
            where T : struct
        {
            // Data may not be in typed part but in raw data
            // Unfortunately there is no common and efficient "ground" for
            // "IReadOnlyList<T> values" and  "ByteString rawData",
            // so we have to go through hoops.
            // RawData and talk about Constant nodes
            // https://github.com/onnx/onnx/issues/2825#issuecomment-644334359

            var useRawData = values.Count == 0 && rawData.Length > 0;
            var rawValues = MemoryMarshal.Cast<byte, T>(rawData.Span);
            var count = useRawData ? rawValues.Length : values.Count;

            const int MaxValueCountToShow = 4;
            if (count <= MaxValueCountToShow)
            {
                return useRawData 
                    ? FormatValues(rawValues.ToArray())
                    : FormatValues(values);
            }
            else if (count > 0)
            {
                if (useRawData) { Thrower.EnsureLittleEndian(); }
                var stats = useRawData
                    ? GetStats(rawValues, min, add, divide, max)
                    : GetStats(values, min, add, divide, max);

                return $"({stats.min:E3},{stats.mean:E3},{stats.max:E3})";
            }
            else
            {
                return "[]";
            }
        }

        static string FormatValues<T>(IReadOnlyList<T> values) => $"[{string.Join(",", values)}]";

        static unsafe (T min, TMean mean, T max) GetStats<T, TMean>(
            ReadOnlySpan<T> values,
            delegate*<T, T, T> min,
            Func<TMean, T, TMean> add,
            Func<TMean, int, TMean> divide,
            delegate*<T, T, T> max) 
            where T : struct
        {
            T minValue = values[0];
            T maxValue = values[0];
            TMean sum = add(default, values[0]);
            for (int i = 1; i < values.Length; i++)
            {
                var value = values[i];
                minValue = min(minValue, value);
                maxValue = max(maxValue, value);
                sum = add(sum, value);
            }
            var mean = divide(sum, values.Length);
            return (minValue, mean, maxValue);
        }

        static unsafe (T min, TMean mean, T max) GetStats<T, TMean>(
            IReadOnlyList<T> values,
            delegate*<T, T, T> min,
            Func<TMean, T, TMean> add,
            Func<TMean, int, TMean> divide,
            delegate*<T, T, T> max)
            where T : struct
        {
            T minValue = values[0];
            T maxValue = values[0];
            TMean sum = add(default, values[0]);
            for (int i = 1; i < values.Count; i++)
            {
                var value = values[i];
                minValue = min(minValue, value);
                maxValue = max(maxValue, value);
                sum = add(sum, value);
            }
            var mean = divide(sum, values.Count);
            return (minValue, mean, maxValue);
        }
    }
}
