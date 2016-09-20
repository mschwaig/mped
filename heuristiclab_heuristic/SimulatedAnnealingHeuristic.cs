using at.mschwaig.mped.definitions;
using System;
using System.Linq;
using HeuristicLab.Algorithms.SimulatedAnnealing;
using HeuristicLab.Problems.MultiParameterizedEditDistance;
using System.Threading;
using HeuristicLab.SequentialEngine;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Data;


namespace at.mschwaig.mped.heuristiclab.heuristic
{
    public class SimulatedAnnealingHeuristic : Heuristic
    {

        public SimulatedAnnealingHeuristic() : base(HeuristicRun.AlgorithmType.HL_SA) {

        }

        public override definitions.Result applyTo(definitions.Problem p)
        {
            var trigger = new ManualResetEvent(false);

            Exception ex = null;
            var alg = new SimulatedAnnealing();
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
