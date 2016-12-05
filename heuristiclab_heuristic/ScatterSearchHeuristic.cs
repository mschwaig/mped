using at.mschwaig.mped.definitions;
using at.mschwaig.mped.persistence;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Algorithms.ScatterSearch;
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
    public class ScatterSearchHeuristic : Heuristic
    {
        public ScatterSearchHeuristic() : base(AlgorithmType.HL_SCATTER)
        {

        }

        public override persistence.Result applyTo(persistence.Problem p, int max_evaluation_number)
        {
            var trigger = new ManualResetEvent(false);

            Exception ex = null;
            var alg = new ScatterSearch();
            alg.Problem = new MpedBasicProblem(p.s1ToAString(), p.s2ToAString());
            if (max_evaluation_number > 0)
            {
                int population_size = (p.a.Length + p.b.Length) * 10;
                alg.MaximumIterations = new IntValue(max_evaluation_number / population_size);
                // TODO: change crossover operator
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
                persistence.Result r = new persistence.Result(p, run);

                r.Solutions.Add(new BestSolution(r, permutation, number_of_evals));

                return r;
            }
            finally
            {
                trigger.Reset();
            }
        }
    }
}
