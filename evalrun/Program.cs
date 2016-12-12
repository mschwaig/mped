﻿using at.mschwaig.mped.cpp_heuristics;
using at.mschwaig.mped.contribution_sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using at.mschwaig.mped.heuristiclab.heuristic;
using at.mschwaig.mped.persistence;


namespace at.mschwaig.mped.evalrun
{
    class Program
    {
        private static Object dblock = new Object();

        static void Main(string[] args)
        {

            // insert experiment
            var heuristics = new List<Heuristic>();
            heuristics.Add(new MinContribSortBasedGuess());
            // heuristics.Add(new SimulatedAnnealingHeuristic());
            // heuristics.Add(new OffspringSelectionGeneticAlgorithmHeuristic());
            heuristics.Add(new CPPHillClimbingHeuristic());
            // heuristics.Add(new CPPSimulatedAnnealingHeuristic());
            //runExperiment("Insert", heuristics);


            // SA vs CPP SA experiment
            var heuristics1 = new List<Heuristic>();
            //heuristics1.Add(new MinContribSortBasedGuess());
            heuristics1.Add(new SimulatedAnnealingHeuristic());
            heuristics1.Add(new OffspringSelectionGeneticAlgorithmHeuristic());
            heuristics1.Add(new CPPHillClimbingHeuristic());
            heuristics1.Add(new CPPSimulatedAnnealingHeuristic());
            runExperiment("Compare", heuristics1);
        }

        static void runExperiment(string experiment_name, List<Heuristic> heuristics)
        {
            foreach (var heuristic in heuristics)
            {
                long evals = 0;

                List<Problem> problemList;
                Experiment ex;
                using (var ctx = new ThesisDbContext())
                {
                    ex = ctx.Experiments.Include("Problems.Results.HeuristicRun").Where(x => x.Name == experiment_name).First();
                    problemList = ex.Problems.Where(x => !x.Results.Where(r => r.HeuristicRun.Algorithm == heuristic.run.Algorithm).Any()).ToList();

                    Parallel.ForEach(problemList, (problem) =>
                    {
                        var watch = System.Diagnostics.Stopwatch.StartNew();

                        var lenghth_product = problem.a.Length * problem.b.Length;

                        var res = heuristic.applyTo(problem);

                        lock (dblock)
                        {
                            ctx.Results.Add(res);
                            ctx.SaveChanges();
                        }

                        Console.WriteLine(String.Format("{0:HH:mm:ss}: Problem {1} took {2} miliseconds.", DateTime.Now, problem.ProblemId, watch.ElapsedMilliseconds));
                        watch.Stop();
                        evals += res.Solutions.Select(x => x.EvalCount).Max();
                    });

                }
            }
        }
    }
}
