using at.mschwaig.mped.definitions;
using at.mschwaig.mped.persistence;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Algorithms.LocalSearch;
using HeuristicLab.Algorithms.VariableNeighborhoodSearch;
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
    public class VariableNeighborhoodSearchHeuristic : HeuristicLabHeuristic<VariableNeighborhoodSearch>
    {
        public VariableNeighborhoodSearchHeuristic() : base(AlgorithmType.HL_VNS) {}

        protected override void attachEvalCountAnalzyer(VariableNeighborhoodSearch alg)
        {
            var evaluation_count_analyzer = EvaluationCountAnalyzerBuilder.createForParameterName("EvaluatedMoves");
            alg.Analyzer.Operators.Add(evaluation_count_analyzer);
        }

        protected override VariableNeighborhoodSearch instantiateAlgorithm()
        {
            return new VariableNeighborhoodSearch();
        }

        protected override void parameterizeAlgorithm(VariableNeighborhoodSearch alg, int albhabet_size_a, int albhabet_size_b, int max_eval_number)
        {
            int inner_iteration_count = albhabet_size_a + albhabet_size_b;

            if (max_eval_number / inner_iteration_count <= 0)
                throw new ArgumentException();

            alg.LocalImprovementMaximumIterations = inner_iteration_count;
            alg.MaximumIterations = max_eval_number / inner_iteration_count;

            alg.LocalImprovement = alg.LocalImprovementParameter.ValidValues.OfType<LocalSearchImprovementOperator>().Single();
        }
    }
}
