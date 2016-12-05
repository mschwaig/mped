using at.mschwaig.mped.definitions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace at.mschwaig.mped.persistence
{

    public class BestSolution
    {
        public int BestSolutionId { get; private set; }

        public int ResultId { get; private set; }

        [Required]
        public virtual Result Result { get; set; }


        [NotMapped]
        public int[] Permutation
        {
            get
            {
                if (permutation_converted != null)
                {
                    return permutation_converted;
                }
                else
                {
                    permutation_converted = PermutationString
                        .Split(',')
                        .Select(x => Int32.Parse(x))
                        .ToArray();
                    return permutation_converted;
                }
            }
        }

        private string PermutationString { get; set; }

        public int EvalCount { get; private set; }

        public int Mped { get; private set; }


        private int[] permutation_converted;

        // Used by Entity Framework
        private BestSolution() { }


        public BestSolution(Result result, Solution solution, int eval_count) : this(
            result,
            String.Join(",", solution.Permutation),
            eval_count
            )
        { }

        public BestSolution(Result result, int[] permutation, int eval_count) : this (
            result,
            String.Join(",", permutation),
            eval_count
            ) {}

        public BestSolution(Result result, int[] a_permutation, int[] permutation_b, int eval_count) : this (
            result,
            a_permutation
                .Zip(permutation_b, (x, y) => Tuple.Create(x, y))
                .OrderBy(x => x.Item1)
                .Select(x => x.Item2)
                .ToArray(),
            eval_count
            ){ }

        public BestSolution(Result result, string permutation_string, int eval_count)
        {
            this.Result = result;
            this.PermutationString = permutation_string;
            this.Mped = DistanceUtil.mped(result.Problem, this.Permutation);
            this.EvalCount = eval_count;

        }
    }
}
