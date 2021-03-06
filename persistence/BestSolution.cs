﻿using at.mschwaig.mped.definitions;
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

        public int EvalCount { get; private set; }

        public int Mped { get; private set; }

        // Used by Entity Framework
        private BestSolution() { }


        public BestSolution(Result result, Solution solution, int eval_count) : this(
            result,
            solution.Permutation,
            eval_count
            )
        { }

        public BestSolution(Result result, int[] permutation, int eval_count) : this (
            result,
            eval_count,
            DistanceUtil.mped(result.Problem, permutation)
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

        public BestSolution(Result result, int eval_count, int mped)
        {
            this.Result = result;
            this.Mped = mped;
            this.EvalCount = eval_count;

        }
    }
}
