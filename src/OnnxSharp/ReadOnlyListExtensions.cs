using System;
using System.Collections.Generic;
using System.Text;

namespace Onnx
{
    public static class ReadOnlyListExtensions
    {
        public static long Product(this IReadOnlyList<long> values)
        {
            var product = 1L;
            for (int i = 0; i < values.Count; i++)
            {
                product *= values[i];
            }
            return product;
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
