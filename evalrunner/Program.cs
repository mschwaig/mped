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

namespace at.mschwaig.mped.evalrunner
{
    class Program
    {
        static void Main(string[] args)
        {

            Heuristic heuristic = new OffspringSelectionGeneticAlgorithmHeuristic();

            int sum = 0;
            long evals = 0;

            List<Problem> problemList;
            using (var ctx = new ThesisDbContext())
            {
                problemList = ctx.Problems.Where(x => !ctx.Results.Where(r => r.Problem == x).Any()).ToList();
            }


            Parallel.ForEach(problemList, (problem) =>
                {
                    var watch = System.Diagnostics.Stopwatch.StartNew();

                    var res = heuristic.applyTo(problem);
                    using (var ctx = new ThesisDbContext())
                    {
                        ctx.Results.Add(res);
                        ctx.SaveChanges();
                    }
                    Console.WriteLine(String.Format("{0:HH:mm:ss}: Problem {1} took {2} miliseconds.", DateTime.Now, problem.Id, watch.ElapsedMilliseconds));
                    watch.Stop();
                    sum += Distance.mped(res.Problem, res.Solution);
                    evals += res.NumberOfEvalsToObtainSolution;
                });
            
        }
    }
}
