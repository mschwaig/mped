using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Algorithms.ALPS;
using HeuristicLab.Problems.MultiParameterizedEditDistance;
using HeuristicLab.SequentialEngine;
using HeuristicLab.Optimization;
using at.mschwaig.mped.persistence;
using at.mschwaig.mped.definitions;
using System.Threading;
using HeuristicLab.Parameters;

namespace at.mschwaig.mped.heuristiclab.heuristic
{
    public class AlpsGeneticAlgorithmHeuristic : Heuristic
    {
        public AlpsGeneticAlgorithmHeuristic() : base(AlgorithmType.HL_ALPS)
        {

        }

        public override persistence.Result applyTo(persistence.Problem p, int max_evaluation_number)
        {
            var trigger = new ManualResetEvent(false);

            Exception ex = null;
            var alg = new AlpsGeneticAlgorithm();
            alg.Problem = new MpedBasicProblem(p.s1ToAString(), p.s2ToAString());
            if (max_evaluation_number > 0)
            {
                int population_size = (p.a.Length + p.b.Length)*2;
                alg.PopulationSize = new IntValue(population_size);
                var evalTerminator = alg.Terminators.Operators.Where(x => x.Name == "Evaluations").Single() as ComparisonTerminator<IntValue>;
                evalTerminator.ThresholdParameter = new FixedValueParameter<IntValue>("Threshold", new IntValue(max_evaluation_number));
                alg.Terminators.Operators.SetItemCheckedState(evalTerminator, true);
                alg.Crossover = alg.CrossoverParameter.ValidValues.OfType<PartiallyMatchedCrossover>().Single();
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
                var number_of_evals = ((IntValue)alg.Results["EvaluatedSolutions"].Value).Value;
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
