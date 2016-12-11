using at.mschwaig.mped.definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Algorithms.OffspringSelectionEvolutionStrategy;
using HeuristicLab.Problems.MultiParameterizedEditDistance;
using HeuristicLab.Optimization;
using HeuristicLab.Core;
using System.Threading;
using HeuristicLab.SequentialEngine;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Data;
using at.mschwaig.mped.persistence;
using HeuristicLab.Analysis;

namespace at.mschwaig.mped.heuristiclab.heuristic
{
    public class OffspringSelectionEvolutionStrategyHeuristic : Heuristic
    {
        public OffspringSelectionEvolutionStrategyHeuristic() : base(AlgorithmType.HL_OSES) {

        }

        public override persistence.Result applyTo(persistence.Problem p, int max_evaluation_number)
        {
            var trigger = new ManualResetEvent(false);

            Exception ex = null;
            var alg = new OffspringSelectionEvolutionStrategy();
            if (max_evaluation_number > 0)
            {
                int pop_size_suggestion = p.a.Length + p.b.Length;

                if (max_evaluation_number / pop_size_suggestion > 0)
                {
                    alg.PopulationSize = new IntValue(pop_size_suggestion);
                    alg.MaximumGenerations = new IntValue(max_evaluation_number / pop_size_suggestion);

                }
                else
                {
                    alg.PopulationSize = new IntValue(1);
                    alg.MaximumGenerations = new IntValue(max_evaluation_number);
                }
                alg.MaximumEvaluatedSolutions = new IntValue(max_evaluation_number);
                alg.MaximumSelectionPressure = new DoubleValue(100);
            }

            alg.Problem = new MpedBasicProblem(p.s1ToAString(), p.s2ToAString());

            var mutator = alg.MutatorParameter.ValidValues.OfType<MultiPermutationManipulator>().Single();

            foreach (var manipulation_op in mutator.Operators)
            {
                mutator.Operators.SetItemCheckedState(manipulation_op, manipulation_op is Swap2Manipulator || manipulation_op is Swap3Manipulator);
            }

            alg.Mutator = mutator;

            alg.Engine = new SequentialEngine();
            alg.Stopped += (sender, args) => { trigger.Set(); };
            alg.ExceptionOccurred += (sender, args) => { ex = args.Value; trigger.Set(); };

            alg.Analyzer.Operators.Add(EvaluationCountAnalyerBuilder.createForParameterName("EvaluatedSolutions"));

            try
            {
                alg.Prepare();
                alg.Start();
                trigger.WaitOne();
                if (ex != null) throw ex;

                persistence.Result r = new persistence.Result(p, run);

                var qualities = ((DataTable)alg.Results["Qualities"].Value).Rows["BestQuality"].Values.ToArray();
                var evaluations = ((DataTable)alg.Results["EvaluationCount Chart"].Value).Rows["EvaluatedSolutions"].Values.ToArray();

                for (int g = 0; g < qualities.Length; g++)
                {
                    r.Solutions.Add(new BestSolution(r, (int)evaluations[g], (int)qualities[g]));
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
