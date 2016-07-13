using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using at.mschwaig.mped.definitions;
using System.Collections;

namespace at.mschwaig.mped.mincontribsort
{
    class ListBasedSorting : SolutionSpaceSortingMethod
    {
        public IEnumerable<Tuple<int, IEnumerable<Solution>>> sortByMinMaxContrib(CharacterMapping[,] one_to_one_mappings)
        {
            byte[] permutation_representation = new byte[one_to_one_mappings.GetLength(0)];
            for (int i = 0; i < one_to_one_mappings.GetLength(0); i++)
            {
                permutation_representation[i] = (byte)i;
            }

            var ed_list = new List<Tuple<int,Solution>>();

            foreach (var permutation in array_permutations(permutation_representation))
            {
                CharacterMapping[] combined_one_to_ones = new CharacterMapping[one_to_one_mappings.GetLength(0)];

                int max_min_ed = Int32.MinValue;
                for (int i = 0; i < one_to_one_mappings.GetLength(0); i++)
                {
                    CharacterMapping add = one_to_one_mappings[i, permutation[i]];
                    if (max_min_ed < add.min_ed)
                    {
                        max_min_ed = add.min_ed;
                    }
                    combined_one_to_ones[i] = add;
                }
                CharacterMapping combined = CharacterMapping.merge(combined_one_to_ones);

                ed_list.Add(Tuple.Create(max_min_ed, new Solution(permutation.Select(x => (int)x).ToArray())));
            }

            var ret = ed_list.AsQueryable()
                .GroupBy(x => x.Item1, x => x.Item2)
                .OrderBy(x => x.Key).Select(x => Tuple.Create(x.Key, (IEnumerable<Solution>)x)); // maybe x.ToList before cast

            return ret;
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
    }

}
