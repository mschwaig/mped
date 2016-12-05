using at.mschwaig.mped.definitions;
using System;
using System.Linq;
using HeuristicLab.Algorithms.SimulatedAnnealing;
using HeuristicLab.Problems.MultiParameterizedEditDistance;
using System.Threading;
using HeuristicLab.SequentialEngine;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Data;
using at.mschwaig.mped.persistence;

namespace at.mschwaig.mped.heuristiclab.heuristic
{
    public class SimulatedAnnealingHeuristic : Heuristic
    {

        public SimulatedAnnealingHeuristic() : base(AlgorithmType.HL_SA) {

        }

        public override persistence.Result applyTo(persistence.Problem p, int max_evaluation_number)
        {
            var trigger = new ManualResetEvent(false);

            Exception ex = null;
            var alg = new SimulatedAnnealing();
            if (max_evaluation_number > 0)
            {
                alg.InnerIterations = new IntValue(p.a.Length + p.b.Length);
                alg.MaximumIterations = new IntValue(max_evaluation_number/alg.InnerIterations.Value);
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
