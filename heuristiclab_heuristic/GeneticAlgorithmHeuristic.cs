using at.mschwaig.mped.definitions;
using at.mschwaig.mped.persistence;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Algorithms.GeneticAlgorithm;
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
    public class GeneticAlgorithmHeuristic : HeuristicLabHeuristic<GeneticAlgorithm>
    {
        public GeneticAlgorithmHeuristic() : base(AlgorithmType.HL_GA){}

        protected override void attachEvalCountAnalzyer(GeneticAlgorithm alg, IntValueAnalyzer eval_count_analyzer)
        {
            alg.Analyzer.Operators.Add(eval_count_analyzer);
        }

        protected override string getEvalCountParameterName()
        {
            return "EvaluatedSolutions";
        }

        protected override GeneticAlgorithm instantiateAlgorithm()
        {
            return new GeneticAlgorithm();
        }

        protected override void parameterizeAlgorithm(GeneticAlgorithm alg, int albhabet_size_a, int albhabet_size_b, int max_eval_number)
        {
            int population_size = (albhabet_size_a + albhabet_size_b) * 10;

            if (max_eval_number / population_size <= 0)
                throw new ArgumentException();

            alg.PopulationSize = new IntValue(population_size);
            alg.MaximumGenerations = new IntValue(max_eval_number / population_size);

            var crossover = alg.CrossoverParameter.ValidValues.OfType<MultiPermutationCrossover>().Single();

            foreach (var crossover_op in crossover.Operators)
            {
                crossover.Operators.SetItemCheckedState(crossover_op,
                    crossover_op is CyclicCrossover ||
                    crossover_op is PartiallyMatchedCrossover);
            }

            alg.Crossover = crossover;

            var mutator = alg.MutatorParameter.ValidValues.OfType<MultiPermutationManipulator>().Single();

            foreach (var manipulation_op in mutator.Operators)
            {
                mutator.Operators.SetItemCheckedState(manipulation_op, manipulation_op is Swap2Manipulator || manipulation_op is Swap3Manipulator);
            }

            alg.Mutator = mutator;
        }
    }
}
