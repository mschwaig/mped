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
using HeuristicLab.Analysis;

namespace at.mschwaig.mped.heuristiclab.heuristic
{
    public class SimulatedAnnealingHeuristic : HeuristicLabHeuristic<SimulatedAnnealing>
    {

        public SimulatedAnnealingHeuristic() : base(AlgorithmType.HL_SA) {}

        protected override void attachEvalCountAnalzyer(SimulatedAnnealing alg, IntValueAnalyzer eval_count_analyzer)
        {
            alg.Analyzer.Operators.Add(eval_count_analyzer);
        }

        protected override string getEvalCountParameterName()
        {
            return "EvaluatedMoves";
        }

        protected override SimulatedAnnealing instantiateAlgorithm()
        {
            return new SimulatedAnnealing();
        }

        protected override void parameterizeAlgorithm(SimulatedAnnealing alg, int albhabet_size_a, int albhabet_size_b, int max_eval_number)
        {
            int inner_iteration_suggestion = albhabet_size_a + albhabet_size_b;

            if (max_eval_number / inner_iteration_suggestion <= 0)
                throw new ArgumentException();

            alg.InnerIterations = new IntValue(inner_iteration_suggestion);
            alg.MaximumIterations = new IntValue(max_eval_number / inner_iteration_suggestion);
        }
    }
}
