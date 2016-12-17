using at.mschwaig.mped.cpp_heuristics;
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
            heuristics.Add(new ContributionSortingHeuristic());
            heuristics.Add(new CPPHillClimbingHeuristic());
            runExperiment("Insert", heuristics, 1);



            // SA vs CPP SA experiment
            var heuristics1 = new List<Heuristic>();
            
            heuristics1.Add(new CPPHillClimbingHeuristic());
            heuristics1.Add(new CPPSimulatedAnnealingHeuristic());
            heuristics1.Add(new GeneticAlgorithmHeuristic());
            heuristics1.Add(new LocalSearchHeuristic());
            heuristics1.Add(new OffspringSelectionEvolutionStrategyHeuristic());
            heuristics1.Add(new OffspringSelectionGeneticAlgorithmHeuristic());
            heuristics1.Add(new SimulatedAnnealingHeuristic());
            heuristics1.Add(new AlpsGeneticAlgorithmHeuristic());

            heuristics1.Add(new EvolutionStrategyHeuristic());
            // heuristics1.Add(new VariableNeighborhoodSearchHeuristic());

            runExperiment("FullComparison", heuristics1, 10);
        }

        static void runExperiment(string experiment_name, List<Heuristic> heuristics, int resultsToGenerate)
        {
            foreach (var heuristic in heuristics)
            {
                long evals = 0;

                List<Problem> problemList;
                Experiment ex;
                using (var ctx = new ThesisDbContext())
                {
                    ex = ctx.Experiments.Include("Problems.Results.HeuristicRun").Where(x => x.Name == experiment_name).First();
                    problemList = ex.Problems.Where(x => x.Results.Where(r => r.HeuristicRun.Algorithm == heuristic.run.Algorithm).Count() < resultsToGenerate).ToList();

                    Parallel.ForEach(problemList, (problem) =>
                    {
                        while (problem.Results.Where(r => r.HeuristicRun.Algorithm == heuristic.run.Algorithm).Count() < resultsToGenerate)
                        {
                            var watch = System.Diagnostics.Stopwatch.StartNew();

                            var res = heuristic.applyTo(problem);

                            lock (dblock)
                            {
                                ctx.Results.Add(res);
                                ctx.SaveChanges();
                            }

                            Console.WriteLine(String.Format("{0:HH:mm:ss}: Problem {1} took {2} miliseconds using {3}.", DateTime.Now, problem.ProblemId, watch.ElapsedMilliseconds, heuristic.run.Algorithm));
                            watch.Stop();
                            evals += res.Solutions.Select(x => x.EvalCount).Max();
                        }
                    });

                }
            }
        }
    }
}
