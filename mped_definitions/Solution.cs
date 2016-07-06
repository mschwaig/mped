using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace at.mschwaig.mped.definitions
{
    public class Solution
    {
        public static Solution create(Problem p, int[] permutation, int number_of_evals)
        {
            return new Solution(p, permutation, number_of_evals);
        }

        public static Solution create(Problem p, int[] a_permutation, int[] b_permutation, int number_of_evals)
        {
            var permutation = a_permutation
                .Zip(b_permutation, (x, y) => Tuple.Create(x, y))
                .OrderBy(x => x.Item1)
                .Select(x => x.Item2)
                .ToArray();
            return new Solution(p, permutation, number_of_evals);

        }


        private Solution(Problem problem, int[] permutation, int number_of_evals)
        {
            this.Problem = problem;
            this.Permutation = permutation;
            this.NumberOfEvalsToObtainSolution = number_of_evals;
        }

        Problem Problem { get; }

        int[] Permutation { get; }

        int NumberOfEvalsToObtainSolution { get; }
    }
}
