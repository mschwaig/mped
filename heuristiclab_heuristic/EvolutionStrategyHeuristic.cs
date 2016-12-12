using at.mschwaig.mped.definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Algorithms.EvolutionStrategy;
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
    public class EvolutionStrategyHeuristic :  HeuristicLabHeuristic<EvolutionStrategy>
    {
        public EvolutionStrategyHeuristic() : base(AlgorithmType.HL_ES) {}

        protected override void attachEvalCountAnalzyer(EvolutionStrategy alg, IntValueAnalyzer eval_count_analyzer)
        {
            alg.Analyzer.Operators.Add(eval_count_analyzer);
        }

        protected override string getEvalCountParameterName()
        {
            return "EvaluatedSolutions";
        }

        protected override EvolutionStrategy instantiateAlgorithm()
        {
            return new EvolutionStrategy();
        }

        protected override void parameterizeAlgorithm(EvolutionStrategy alg, int albhabet_size_a, int albhabet_size_b, int max_eval_number)
        {
            int pop_size_suggestion = albhabet_size_a + albhabet_size_b;

            if (max_eval_number / pop_size_suggestion <= 0)
                throw new ArgumentOutOfRangeException();

            alg.PopulationSize = new IntValue(pop_size_suggestion);
            alg.MaximumGenerations = new IntValue(max_eval_number / pop_size_suggestion);       
        }
    }
}
