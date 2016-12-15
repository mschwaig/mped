using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace at.mschwaig.mped.resultplot
{
    static class DataFilter
    {
        public static IEnumerable<T> removedNeighouringDuplicateValues<T>(this IEnumerable<T> source, Func<T, IComparable> duplicate_detector)
        {
            var input = source.GetEnumerator();
            if (!input.MoveNext()) yield break;
            T past = input.Current;
            yield return past;

            if (!input.MoveNext()) yield break;
            T current = input.Current;

            T future = default(T);

            while (input.MoveNext())
            {
                future = input.Current;

                if (!duplicate_detector(past).Equals(duplicate_detector(current)))
                {
                    yield return current;
                }
                else if (!duplicate_detector(current).Equals(duplicate_detector(future)))
                {
                    yield return current;
                }

                past = current;
                current = future;
            }

            yield return current;

        }

        public static IEnumerable<IComparable> removedNeighouringDuplicateValues<T>(this IEnumerable<IComparable> source)
        {
            return removedNeighouringDuplicateValues(source, x => x);
        }

        public static double computeStandardDeviation(this IEnumerable<int> values)
        {
            double result = 0;
            int count = values.Count();
            if (count > 1)
            {
                double avg = values.Average();
                double sum = values.Sum(d => (d - avg) * (d - avg));
                result = Math.Sqrt(sum / count);
            }
            return result;
        }
    }
}
