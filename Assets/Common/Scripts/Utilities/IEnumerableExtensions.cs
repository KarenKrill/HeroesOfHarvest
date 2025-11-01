using System.Collections.Generic;
using System.Linq;

namespace KarenKrill.Utilities
{
    public static class IEnumerableExtensions
    {
        public static void MinAvgMax(this IEnumerable<float> enumerable, out float min, out float avg, out float max)
        {
            avg = 0;
            min = float.MaxValue;
            max = float.MinValue;
            var itemsCount = enumerable.Count();
            if (itemsCount > 0)
            {
                float sum = 0;
                foreach (var item in enumerable)
                {
                    sum += item;
                    if (item < min)
                    {
                        min = item;
                    }
                    if (item > max)
                    {
                        max = item;
                    }
                }
                avg = sum / itemsCount;
            }
        }
    }
}
