using at.mschwaig.mped.definitions;
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
using HeuristicLab.Analysis;

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

                persistence.Result r = new persistence.Result(p, run);

                var qualities = ((DataTable)alg.Results["Qualities"].Value).Rows["BestQuality"].Values.ToArray();
                var evals_per_generations = alg.SampleSize.Value;

                for (int g = 0; g < qualities.Length; g++)
                {
                    r.Solutions.Add(new BestSolution(r, g * evals_per_generations, (int)qualities[g]));
                }

                return r;
            }
            finally
            {
                trigger.Reset();
            }
        }
    }
}
