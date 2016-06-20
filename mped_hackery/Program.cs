using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace mped_hackery
{
    // sources http://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.217.3627&rep=rep1&type=pdf


    class Program
    {

        static char[] a, b;
        static string s1, s2;

        static SortedList<int, CharacterMapping> generateDataForCorrelatedStrings(int alphabet_size, int length, double correlation)
        {
            CorrelatedStringPair test = generatePairOfStrings(alphabet_size, length, correlation);

            s1 = test.s1;
            s2 = test.s2;
            a = test.alphabet;
            b = test.alphabet;


            CharacterMapping[,] one_to_one_mappings = new CharacterMapping[a.Length, b.Length];

            // generate all possible 1:1 mappings of characters
            for (int i = 0; i < a.Length; i++)
            {
                for (int j = 0; j < b.Length; j++)
                {
                    Dictionary<char, char> dict = new Dictionary<char, char>();
                    dict.Add(a[i], b[j]);
                    CharacterMapping cm = new CharacterMapping(a, b, s1, s2, dict);
                    one_to_one_mappings[i, j] = cm;
                }
            }

            byte[] permutation_representation = new byte[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                permutation_representation[i] = (byte)i;
            }

            SortedList<int, CharacterMapping> min_ed_list = new SortedList<int, CharacterMapping>(new DuplicateKeyComparer<int>());
            foreach (var permutation in array_permutations(permutation_representation))
            {
                CharacterMapping[] combined_one_to_ones = new CharacterMapping[a.Length];

                int max_min_ed = Int32.MinValue;
                for (int i = 0; i < a.Length; i++)
                {
                    CharacterMapping add = one_to_one_mappings[i, permutation[i]];
                    if (max_min_ed < add.min_ed)
                    {
                        max_min_ed = add.min_ed;
                    }
                    combined_one_to_ones[i] = add;
                }
                CharacterMapping combined = CharacterMapping.merge(combined_one_to_ones);
                min_ed_list.Add(max_min_ed, combined);
            }

            return min_ed_list;
        }

        static void Main(string[] args)
        {
            int alphabet_size = 6;
            int length = 100;

            using (System.IO.StreamWriter file = new System.IO.StreamWriter("results.txt"))
            {

                file.WriteLine(String.Format("correlation, min evals to find min, real evals to find min, max evals to find min, evals to prove min, mped. alphabet size: {0}, string length: {1}", alphabet_size, length));

                for (int correlation_step = 0; correlation_step <= 10; correlation_step++)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        double correlation = 0.1d * correlation_step;
                        var result = generateDataForCorrelatedStrings(alphabet_size, length, correlation);
                        int minimum_ed = Int32.MaxValue;
                        CharacterMapping minimum_mapping = null;
                        int min_ed_contribution = Int32.MaxValue;
                        foreach (var r in result)
                        {
                            if (r.Value.min_ed < minimum_ed)
                            {
                                minimum_ed = r.Value.min_ed;
                                minimum_mapping = r.Value;
                                min_ed_contribution = r.Key;
                            }
                        }

                        var real_evals_to_find_min = result.Select((value, index) => new { value, index })
                            .Where(pair => (pair.value.Value.min_ed == minimum_ed))
                            .Select(pair => pair.index)
                            .FirstOrDefault() + 1;

                        real_evals_to_find_min += alphabet_size * alphabet_size;

                        var min_evals_to_find_min = result.Select((value, index) => new { value, index })
                            .Where(pair => (pair.value.Key == min_ed_contribution))
                            .Select(pair => pair.index)
                            .FirstOrDefault() + 1;

                        min_evals_to_find_min += alphabet_size * alphabet_size;

                        var max_evals_to_find_min = result.Select((value, index) => new { value, index })
                            .Where(pair => (pair.value.Key > min_ed_contribution))
                            .Select(pair => pair.index)
                            .FirstOrDefault();

                        if (max_evals_to_find_min == 0)
                        {
                            max_evals_to_find_min = factorial[alphabet_size];
                        }
                        max_evals_to_find_min += alphabet_size * alphabet_size;

                        var evals_to_prove_min = result.Select((value, index) => new { value, index })
                            .Where(pair => (pair.value.Key > minimum_ed))
                            .Select(pair => pair.index)
                            .FirstOrDefault();

                        if (evals_to_prove_min == 0)
                        {
                            evals_to_prove_min = factorial[alphabet_size];
                        }
                        evals_to_prove_min += alphabet_size*alphabet_size;
                        file.WriteLine(String.Format("{0} {1} {2} {3} {4} {5}", correlation, min_evals_to_find_min, real_evals_to_find_min, max_evals_to_find_min, evals_to_prove_min, minimum_ed));
                    }
                }
            }
        }

        private static int[,] ed(string a, string b, Func<char, char, bool> mapping)
        {
            int[,] distance = new int[a.Length + 1, b.Length + 1];

            for (int i = 0; i < a.Length + 1; i++)
            {
                distance[i, 0] = i;
            }

            for (int j = 0; j < b.Length + 1; j++)
            {
                distance[0, j] = j;
            }

            for (int j = 0; j < b.Length; j++)
            {
                for (int i = 0; i < a.Length; i++)
                {
                    if (mapping(a[i], b[j]))
                    {
                        distance[i + 1, j + 1] = distance[i, j];
                    }
                    else
                    {
                        int delete = distance[i, j + 1] + 1;
                        int insert = distance[i + 1, j] + 1;
                        int substitute = distance[i, j] + 1;
                        distance[i + 1, j + 1] = Math.Min(delete, Math.Min(insert, substitute));
                    }
                }
            }

            return distance;
        }


        public static void printMatrix(CharacterMapping[,] one_to_one_mappings)
        {
            Console.WriteLine("   " + String.Join("   ", b));

            for (int i = 0; i < a.Length; i++)
            {
                Console.Write(a[i] + " ");
                for (int j = 0; j < b.Length; j++)
                {
                    int min = one_to_one_mappings[i, j].min_ed;
                    if (min < 12)
                    {
                        Console.Write(String.Format("{0,3} ", one_to_one_mappings[i, j].min_ed));
                    }
                    else
                    {
                        Console.Write("   ");
                    }
                }
                Console.WriteLine();
            }
        }

        private static IEnumerable<byte[]> array_permutations(byte[] a)
        {
            long length = a.LongLength;
            byte[] a_copy = new byte[a.LongLength];
            Array.Copy(a, a_copy, a.LongLength);

            foreach (byte[] p in permutate_array(length, a_copy))
            {
                byte[] p_copy = new byte[p.LongLength];
                Array.Copy(p, p_copy, p.LongLength);
                yield return p_copy;
            }
        }

        private static IEnumerable<byte[]> permutate_array(long length, byte[] a)
        {
            if (length == 1)
            {
                yield return a;
            }
            else
            {
                for (long i = 0; i < length - 1; i++)
                {
                    var sub1 = permutate_array(length - 1, a);

                    foreach (var s1 in sub1)
                    {
                        yield return a;
                    }

                    long swap_index = (length % 2 == 0 ? i : 0);

                    var temp = a[swap_index];
                    a[swap_index] = a[length - 1];
                    a[length - 1] = temp;
                }

                var sub2 = permutate_array(length - 1, a);

                foreach (var s2 in sub2)
                {
                    yield return a;
                }
            }
        }

        /// source: stackoverflow
        /// <summary>
        /// Comparer for comparing two keys, handling equality as beeing greater
        /// Use this Comparer e.g. with SortedLists or SortedDictionaries, that don't allow duplicate keys
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        public class DuplicateKeyComparer<TKey>
                        :
                     IComparer<TKey> where TKey : IComparable
        {
            #region IComparer<TKey> Members

            public int Compare(TKey x, TKey y)
            {
                int result = x.CompareTo(y);

                if (result == 0)
                    return 1;   // Handle equality as beeing greater
                else
                    return result;
            }

            #endregion
        }
        
        public static CorrelatedStringPair generatePairOfStrings(int alphabet_size, int length, double generate_same_char_probability)
        {
            char[] alphabet = new char[alphabet_size];
            char[] alphabet2 = new char[alphabet_size];
            for (int i = 0; i < alphabet_size; i++)
            {
                if (i < 26)
                {
                    alphabet[i] = (char)('A' + i);
                }
                else
                {
                    alphabet[i] = (char)('A' + (i - 26));
                }
            }

            Array.Copy(alphabet, alphabet2, alphabet_size);
            Random r = new Random();
            r.Shuffle(alphabet2);
            char[] s1 = new char[length];
            char[] s2 = new char[length];
            for (int i = 0; i < s1.Length; i++)
            {
                int r1 = r.Next(alphabet_size);
                s1[i] = alphabet[r1];
                if (r.NextDouble() < generate_same_char_probability)
                {
                    s2[i] = alphabet2[r1];
                }
                else
                {
                    s2[i] = alphabet[r.Next(alphabet_size)];
                }
            }
            return new CorrelatedStringPair(new String(s1), new String(s2), alphabet, generate_same_char_probability);

        }

        // source: stackoverflow
        private static readonly int[] factorial = new int[]{
    1,
    1,
    2,
    6,
    24,
    120,
    720,
    5040,
    40320,
    362880,
    3628800,
    39916800,
    479001600,
    1932053504,
};

    }

    class CorrelatedStringPair
    {
        public string s1;
        public string s2;
        public char[] alphabet;

        public double introduced_correlation;

        public CorrelatedStringPair(string s1, string s2, char[] alphabet, double introduced_correlation)
        {
            this.s1 = s1;
            this.s2 = s2;
            this.alphabet = alphabet;
            this.introduced_correlation = introduced_correlation;
        }
    }

    // source: stackoverflow
    static class RandomExtensions
    {
        public static void Shuffle<T>(this Random rng, T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = rng.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
    }
}
