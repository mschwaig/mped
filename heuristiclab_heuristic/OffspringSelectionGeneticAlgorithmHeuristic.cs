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

namespace at.mschwaig.mped.heuristiclab.heuristic
{
    public class OffspringSelectionGeneticAlgorithmHeuristic : Heuristic
    {

        public OffspringSelectionGeneticAlgorithmHeuristic() : base(AlgorithmType.HL_OSGA) {

        }

        public override persistence.Result applyTo(persistence.Problem p, int max_evaluation_number)
        {
            var trigger = new ManualResetEvent(false);

            Exception ex = null;
            var alg = new OffspringSelectionGeneticAlgorithm();
            if (max_evaluation_number > 0)
            {
                int pop_size_suggestion = p.a.Length + p.b.Length;

                if (max_evaluation_number / pop_size_suggestion > 0) {
                    alg.PopulationSize = new IntValue(pop_size_suggestion);
                    alg.MaximumGenerations = new IntValue(max_evaluation_number / pop_size_suggestion);
                    alg.MaximumEvaluatedSolutions = new IntValue(max_evaluation_number);
                } else {
                    alg.PopulationSize = new IntValue(1);
                    alg.MaximumGenerations = new IntValue(max_evaluation_number);
                    alg.MaximumEvaluatedSolutions = new IntValue(max_evaluation_number);
                }
                alg.MaximumSelectionPressure = new DoubleValue(1000);
            }

            alg.Problem = new MpedBasicProblem(p.s1ToAString(), p.s2ToAString());

            var crossover = alg.CrossoverParameter.ValidValues.OfType<MultiPermutationCrossover>().Single();

            foreach (var crossover_op in crossover.Operators)
            {
                crossover.Operators.SetItemCheckedState(crossover_op,
                    crossover_op is CyclicCrossover ||
                    crossover_op is CyclicCrossover2 ||
                    crossover_op is PartiallyMatchedCrossover);
            }

            alg.Crossover = crossover;

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
