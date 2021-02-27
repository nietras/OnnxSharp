using System;
using System.Collections.Generic;
using System.Text;

namespace Onnx
{
    public static class ReadOnlyListExtensions
    {
        public static long ProductSum(this IReadOnlyList<long> values)
        {
            if (values.Count > 0)
            {
                var sum = 1L;
                for (int i = 0; i < values.Count; i++)
                {
                    sum *= values[i];
                }
                return sum;
            }
            else
            {
                return 0;
            }
        }

        internal static T Single<T, TSelect>(this IReadOnlyList<T> fields, Func<T, TSelect> select, TSelect valueToFind)
            where TSelect : IEquatable<TSelect>
        {
            for (int i = 0; i < fields.Count; i++)
            {
                var field = fields[i];
                var value = select(field);
                if (value.Equals(valueToFind))
                {
                    return field;
                }
            }
            throw new ArgumentException($"Could not find field with value '{valueToFind}'");
        }
    }
}
