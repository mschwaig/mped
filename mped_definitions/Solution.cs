using System;
using System.Linq;

namespace at.mschwaig.mped.definitions
{
    public class Solution
    {
        public int[] Permutation { get; }

        public Solution(int [] permutation)
        {
            this.Permutation = permutation;
        }

        public Solution(int[] a_permutation, int[] permutation_b)
        {
            this.Permutation = a_permutation
                .Zip(permutation_b, (x, y) => Tuple.Create(x, y))
                .OrderBy(x => x.Item1)
                .Select(x => x.Item2)
                .ToArray();
        }
    }
}
