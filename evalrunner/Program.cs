using at.mschwaig.mped.definitions;
using at.mschwaig.mped.cpp_heuristics;
using at.mschwaig.mped.mincontribsort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using at.mschwaig.mped.heuristiclab.heuristic;
using System.Threading;
using at.mschwaig.mped.persistence;

namespace at.mschwaig.mped.evalrunner
{
    class Program
    {
        static void Main(string[] args)
        {
            var heuristics = new List<Heuristic>();
            heuristics.Add(new MinContribSortBasedGuess());
            // heuristics.Add(new SimulatedAnnealingHeuristic());
            // heuristics.Add(new OffspringSelectionGeneticAlgorithmHeuristic());
            heuristics.Add(new CPPHillClimbingHeuristic());
            // heuristics.Add(new CPPSimulatedAnnealingHeuristic());

            foreach (var heuristic in heuristics)
            {
                int sum = 0;
                long evals = 0;

                List<Problem> problemList;
                Experiment ex;
                using (var ctx = new ThesisDbContext())
                {
                    ex = ctx.Experiments.Include("Problems.Results.HeuristicRun").Where(x => x.Name == "Insert").First();
                    problemList = ex.Problems.Where(x => !x.Results.Where(r => r.HeuristicRun.Algorithm == heuristic.run.Algorithm).Any()).ToList();
                }


                Parallel.ForEach(problemList, (problem) =>
                    {
                        var watch = System.Diagnostics.Stopwatch.StartNew();

                        var res = heuristic.applyTo(problem);

                        using (var ctx = new ThesisDbContext())
                        {
                            ctx.Problems.Attach(res.Problem);
                            ctx.HeuristicRun.Attach(res.HeuristicRun);
                            ctx.Results.Add(res);
                            ctx.SaveChanges();
                        }

                        Console.WriteLine(String.Format("{0:HH:mm:ss}: Problem {1} took {2} miliseconds.", DateTime.Now, problem.ProblemId, watch.ElapsedMilliseconds));
                        watch.Stop();
                        sum += DistanceUtil.mped(res.Problem, res.Solution);
                        evals += res.NumberOfEvalsToObtainSolution;
                    });

            }

        }
    }
}
