﻿using at.mschwaig.mped.definitions;
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
using HeuristicLab.Operators;
using HeuristicLab.Selection;

namespace at.mschwaig.mped.heuristiclab.heuristic
{
    public class OffspringSelectionGeneticAlgorithmHeuristic : HeuristicLabHeuristic<OffspringSelectionGeneticAlgorithm>
    {

        public OffspringSelectionGeneticAlgorithmHeuristic() : base(AlgorithmType.HL_OSGA) {}

        protected override void attachEvalCountAnalzyer(OffspringSelectionGeneticAlgorithm alg, IntValueAnalyzer eval_count_analyzer)
        {
            alg.Analyzer.Operators.Add(eval_count_analyzer);
        }

        protected override string getEvalCountParameterName()
        {
            return "EvaluatedSolutions";
        }

        protected override OffspringSelectionGeneticAlgorithm instantiateAlgorithm()
        {
            var alg = new OffspringSelectionGeneticAlgorithm();
            var mainLoop = (AlgorithmOperator)alg.OperatorGraph.Operators.Single(x => x is OffspringSelectionGeneticAlgorithmMainLoop);
            var mainOp = (AlgorithmOperator)mainLoop.OperatorGraph.Operators.Single(x => x is OffspringSelectionGeneticAlgorithmMainOperator);
            var offSel = mainOp.OperatorGraph.Operators.OfType<OffspringSelector>().Single();

            var comp = new Comparator();
            comp.Comparison = new Comparison(ComparisonType.GreaterOrEqual);
            comp.LeftSideParameter.ActualName = "EvaluatedSolutions";
            comp.ResultParameter.ActualName = "TerminateEvaluatedSolutions";
            comp.RightSideParameter.ActualName = "MaximumEvaluatedSolutions";

            var cond = new ConditionalBranch();
            cond.ConditionParameter.ActualName = "TerminateEvaluatedSolutions";

            comp.Successor = cond;
            cond.FalseBranch = offSel.OffspringCreator;
            offSel.OffspringCreator = comp;

            return alg;
        }

        protected override void parameterizeAlgorithm(OffspringSelectionGeneticAlgorithm alg, int albhabet_size_a, int albhabet_size_b, int max_eval_number)
        {
            int pop_size_suggestion = (albhabet_size_a + albhabet_size_b)*10;

            if (max_eval_number / pop_size_suggestion <= 0)
                throw new ArgumentException();

            alg.PopulationSize = new IntValue(pop_size_suggestion);
            alg.MaximumGenerations = new IntValue(max_eval_number / pop_size_suggestion);
            alg.MaximumEvaluatedSolutions = new IntValue(max_eval_number);

            alg.MaximumSelectionPressure = new DoubleValue(200);

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
