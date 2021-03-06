﻿using System;
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
using HeuristicLab.Analysis;

namespace at.mschwaig.mped.heuristiclab.heuristic
{
    public class AlpsGeneticAlgorithmHeuristic : HeuristicLabHeuristic<AlpsGeneticAlgorithm>
    {
        public AlpsGeneticAlgorithmHeuristic() : base(AlgorithmType.HL_ALPS){}

        protected override void attachEvalCountAnalzyer(AlpsGeneticAlgorithm alg, IntValueAnalyzer eval_count_analyzer)
        {
            alg.Analyzer.Operators.Add(eval_count_analyzer);
        }

        protected override string getEvalCountParameterName()
        {
            return "EvaluatedSolutions";
        }

        protected override AlpsGeneticAlgorithm instantiateAlgorithm()
        {
            return new AlpsGeneticAlgorithm();
        }

        protected override void parameterizeAlgorithm(AlpsGeneticAlgorithm alg, int albhabet_size_a, int albhabet_size_b, int max_eval_number)
        {
            int population_size = (albhabet_size_a  + albhabet_size_a) * 2;
            alg.PopulationSize = new IntValue(population_size);
            var evalTerminator = alg.Terminators.Operators.Where(x => x.Name == "Evaluations").Single() as ComparisonTerminator<IntValue>;
            evalTerminator.ThresholdParameter = new FixedValueParameter<IntValue>("Threshold", new IntValue(max_eval_number));
            alg.Terminators.Operators.SetItemCheckedState(evalTerminator, true);

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
