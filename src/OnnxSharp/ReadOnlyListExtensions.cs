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
    }
}
