﻿using at.mschwaig.mped.definitions;
using at.mschwaig.mped.persistence;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Algorithms.LocalSearch;
using HeuristicLab.Problems.MultiParameterizedEditDistance;
using HeuristicLab.SequentialEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HeuristicLab.Optimization;

namespace at.mschwaig.mped.heuristiclab.heuristic
{
    public class LocalSearchHeuristic : Heuristic
    {
        public LocalSearchHeuristic() : base(AlgorithmType.HL_LS)
        {

        }

        public override persistence.Result applyTo(persistence.Problem p, int max_evaluation_number)
        {
            var trigger = new ManualResetEvent(false);

            Exception ex = null;
            var alg = new LocalSearch();
            alg.Problem = new MpedBasicProblem(p.s1ToAString(), p.s2ToAString());
            if (max_evaluation_number > 0)
            {
                int inner_iteration_count = p.a.Length + p.b.Length;
                alg.SampleSize = new IntValue(inner_iteration_count);
                alg.MaximumIterations = new IntValue(max_evaluation_number / inner_iteration_count);
            }

            alg.Engine = new SequentialEngine();
            alg.Stopped += (sender, args) => { trigger.Set(); };
            alg.ExceptionOccurred += (sender, args) => { ex = args.Value; trigger.Set(); };

            try
            {
                alg.Prepare();
                alg.Start();
                trigger.WaitOne();
                if (ex != null) throw ex;
                var permutation = ((Permutation)alg.Results["Best Solution"].Value).ToArray();
                var number_of_evals = ((IntValue)alg.Results["EvaluatedMoves"].Value).Value;
                var solution = new Solution(permutation);
                return persistence.Result.create(p, solution, run, number_of_evals);
            }
            finally
            {
                trigger.Reset();
            }
        }
    }
}