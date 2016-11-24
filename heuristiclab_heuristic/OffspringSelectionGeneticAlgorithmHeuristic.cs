﻿using at.mschwaig.mped.definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Algorithms.OffspringSelectionGeneticAlgorithm;
using HeuristicLab.Problems.MultiParameterizedEditDistance;
using HeuristicLab.Optimization;
using HeuristicLab.Core;
using System.Threading;
using HeuristicLab.SequentialEngine;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Data;
using at.mschwaig.mped.persistence;

namespace at.mschwaig.mped.heuristiclab.heuristic
{
    public class OffspringSelectionGeneticAlgorithmHeuristic : Heuristic
    {

        public OffspringSelectionGeneticAlgorithmHeuristic() : base(AlgorithmType.HL_OSGA) {

        }

        public override persistence.Result applyTo(persistence.Problem p, int max_evaluation_number)
        {
            var trigger = new ManualResetEvent(false);

            Exception ex = null;
            var alg = new OffspringSelectionGeneticAlgorithm();
            if (max_evaluation_number > 0)
            {
                int pop_size_suggestion = p.a.Length + p.b.Length;

                if (max_evaluation_number / pop_size_suggestion > 0) {
                    alg.PopulationSize = new IntValue(pop_size_suggestion);
                    alg.MaximumGenerations = new IntValue(max_evaluation_number / pop_size_suggestion);
                    alg.MaximumEvaluatedSolutions = new IntValue(max_evaluation_number);
                } else {
                    alg.PopulationSize = new IntValue(1);
                    alg.MaximumGenerations = new IntValue(max_evaluation_number);
                    alg.MaximumEvaluatedSolutions = new IntValue(max_evaluation_number);
                }
                alg.MaximumSelectionPressure = new DoubleValue(1000);
            }

            alg.Problem = new MpedBasicProblem(p.s1ToAString(), p.s2ToAString());
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
                var number_of_evals = ((IntValue)alg.Results["EvaluatedSolutions"].Value).Value;
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
