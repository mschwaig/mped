using at.mschwaig.mped.core;
using at.mschwaig.mped.cpp;
using at.mschwaig.mped.definitions;
using at.mschwaig.mped.mincontribsort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace at.mschwaig.mped.evalrunner
{
    class Program
    {
        static void Main(string[] args)
        {
            // Heuristic heuristic = new MinContribSort(MinContribSort.Mode.FIRST_GUESS, new LinearExactMaxiumumAssignmentBasedSorting());
            Heuristic heuristic = new SimulatedAnnealing();

            int i = 0;
            int sum = 0;
            long evals = 0;

            using (var ctx = new ThesisDbContext())
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();


                foreach (var problem in ctx.Problems)
                {
                    watch.Restart();

                    var res = heuristic.applyTo(problem);
                    Console.WriteLine(String.Format("{0:HH:mm:ss}: Problem {1} took {2} miliseconds.", DateTime.Now, i, watch.ElapsedMilliseconds));
                    watch.Stop();
                    i += 1;
                    sum += Distance.mped(res.Problem, res.Solution);
                    evals += res.NumberOfEvalsToObtainSolution;
                }
            }
        }
    }
}
