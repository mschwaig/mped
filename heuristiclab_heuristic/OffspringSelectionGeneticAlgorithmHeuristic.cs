﻿using at.mschwaig.mped.definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Algorithms.OffspringSelectionGeneticAlgorithm;
using at.mschwaig.mped.heuristiclab.plugin;
using HeuristicLab.Optimization;
using HeuristicLab.Core;
using System.Threading;
using HeuristicLab.SequentialEngine;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Data;

namespace at.mschwaig.mped.heuristiclab.heuristic
{
    public class OffspringSelectionGeneticAlgorithmHeuristic : Heuristic
    {

        public OffspringSelectionGeneticAlgorithmHeuristic() : base(HeuristicRun.AlgorithmType.HL_OSGA) {

        }

        public override definitions.Result applyTo(definitions.Problem p)
        {
            var trigger = new ManualResetEvent(false);

            Exception ex = null;
            var alg = new OffspringSelectionGeneticAlgorithm();
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
                return definitions.Result.create(p, solution, run, number_of_evals);
            }
            finally
            {
                trigger.Reset();
            }
        }
    }
}
