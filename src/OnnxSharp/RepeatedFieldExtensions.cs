using System;
using Google.Protobuf.Collections;

namespace Onnx
{
    internal static class RepeatedFieldExtensions
    {
        internal static bool TryRemove<T, TSelect>(this RepeatedField<T> fields, Func<T, TSelect> select, Predicate<TSelect> predicate)
        {
            for (int i = 0; i < fields.Count; i++)
            {
                var field = fields[i];
                var value = select(field);
                if (predicate(value))
                {
                    fields.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        internal static bool TryRemove<T, TSelect>(this RepeatedField<T> fields, Func<T, TSelect> select, TSelect valueToRemove)
            where TSelect : IEquatable<TSelect>
        {
            var index = fields.IndexOf(select, valueToRemove);
            if (index >= 0)
            {
                fields.RemoveAt(index);
                return true;
            }
            return false;
        }

        internal static T Single<T, TSelect>(this RepeatedField<T> fields, Func<T, TSelect> select, TSelect valueToFind)
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

        internal static int IndexOf<T, TSelect>(this RepeatedField<T> fields, Func<T, TSelect> select, TSelect valueToFind)
            where TSelect : IEquatable<TSelect>
        {
            for (int i = 0; i < fields.Count; i++)
            {
                var field = fields[i];
                var value = select(field);
                if (value.Equals(valueToFind))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
