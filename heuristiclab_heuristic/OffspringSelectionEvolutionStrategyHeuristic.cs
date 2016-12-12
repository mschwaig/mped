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
    public class OffspringSelectionEvolutionStrategyHeuristic : HeuristicLabHeuristic<OffspringSelectionEvolutionStrategy>
    {
        public OffspringSelectionEvolutionStrategyHeuristic() : base(AlgorithmType.HL_OSES) {}

        protected override void attachEvalCountAnalzyer(OffspringSelectionEvolutionStrategy alg, IntValueAnalyzer eval_count_analyzer)
        {
            alg.Analyzer.Operators.Add(eval_count_analyzer);
        }

        protected override string getEvalCountParameterName()
        {
            return "EvaluatedSolutions";
        }

        protected override OffspringSelectionEvolutionStrategy instantiateAlgorithm()
        {
            return new OffspringSelectionEvolutionStrategy();
        }

        protected override void parameterizeAlgorithm(OffspringSelectionEvolutionStrategy alg, int albhabet_size_a, int albhabet_size_b, int max_eval_number)
        {
            int pop_size_suggestion = albhabet_size_a + albhabet_size_b;

            if (max_eval_number / pop_size_suggestion <= 0)
                throw new ArgumentException();

            alg.PopulationSize = new IntValue(pop_size_suggestion);
            alg.MaximumGenerations = new IntValue(max_eval_number / pop_size_suggestion);
            alg.MaximumEvaluatedSolutions = new IntValue(max_eval_number);
            alg.MaximumSelectionPressure = new DoubleValue(100);

            var mutator = alg.MutatorParameter.ValidValues.OfType<MultiPermutationManipulator>().Single();

            foreach (var manipulation_op in mutator.Operators)
            {
                mutator.Operators.SetItemCheckedState(manipulation_op, manipulation_op is Swap2Manipulator || manipulation_op is Swap3Manipulator);
            }

            alg.Mutator = mutator;
        }
    }
}
