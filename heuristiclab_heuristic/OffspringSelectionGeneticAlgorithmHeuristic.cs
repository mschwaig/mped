using at.mschwaig.mped.definitions;
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
using HeuristicLab.Analysis;

namespace at.mschwaig.mped.heuristiclab.heuristic
{
    public class OffspringSelectionGeneticAlgorithmHeuristic : HeuristicLabHeuristic<OffspringSelectionGeneticAlgorithm>
    {

        public OffspringSelectionGeneticAlgorithmHeuristic() : base(AlgorithmType.HL_OSGA) {}

        protected override void attachEvalCountAnalzyer(OffspringSelectionGeneticAlgorithm alg)
        {
            var evaluation_count_analyzer = EvaluationCountAnalyzerBuilder.createForParameterName("EvaluatedSolutions");
            alg.Analyzer.Operators.Add(evaluation_count_analyzer);
        }

        protected override OffspringSelectionGeneticAlgorithm instantiateAlgorithm()
        {
            return new OffspringSelectionGeneticAlgorithm();
        }

        protected override void parameterizeAlgorithm(OffspringSelectionGeneticAlgorithm alg, int albhabet_size_a, int albhabet_size_b, int max_eval_number)
        {
            int pop_size_suggestion = (albhabet_size_a + albhabet_size_b)*10;
 //           int pop_size_suggestion = (albhabet_size_a + albhabet_size_b);

            if (max_eval_number / pop_size_suggestion <= 0)
                throw new ArgumentException();

            alg.PopulationSize = new IntValue(pop_size_suggestion);
            alg.MaximumGenerations = new IntValue(max_eval_number / pop_size_suggestion);
            alg.MaximumEvaluatedSolutions = new IntValue(max_eval_number);

            alg.MaximumSelectionPressure = new DoubleValue(1000);

            var crossover = alg.CrossoverParameter.ValidValues.OfType<MultiPermutationCrossover>().Single();

            foreach (var crossover_op in crossover.Operators)
            {
                crossover.Operators.SetItemCheckedState(crossover_op,
 //                   crossover_op is CyclicCrossover ||
 //                   crossover_op is CyclicCrossover2 ||
                    crossover_op is PartiallyMatchedCrossover);
            }

            alg.Crossover = crossover;
        }
    }
}
