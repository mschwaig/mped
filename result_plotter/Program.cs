using at.mschwaig.mped.definitions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using at.mschwaig.mped.persistence;

namespace result_plotter
{
    class Program
    {
        static void Main(string[] args)
        {
            evaluateExperimentResultsByProblem();
            evaluateInsertExperiment();
        }

        static void evaluateExperimentResultsByProblem()
        {
            using (var ctx = new ThesisDbContext())
            {
                Experiment ex = ctx.Experiments.Include("Problems.Results.HeuristicRun").Where(x => x.Name == "Insert").First();

                foreach (var problem in ex.Problems)
                {
                    using (StreamWriter file = new StreamWriter(problem.ProblemId + ".txt"))
                    {
                        foreach (var result in problem.Results)
                        {
                            file.WriteLine("{0} {1} {2}", result.HeuristicRun.Algorithm.ToString(), result.Mped, result.NumberOfEvalsToObtainSolution);
                        }
                    }
                }
            }
        }

        static void evaluateInsertExperiment()
        {
            using (var ctx = new ThesisDbContext())
            {
                Experiment ex = ctx.Experiments.Include("Problems.Results.HeuristicRun").Where(x => x.Name == "Insert").First();

                using (StreamWriter file = new StreamWriter("mincontrib.txt"))
                {
                    var grouped = ex.Problems.GroupBy(x => new { x.InsertProb, x.SubstituteProb });
                    foreach (var group in grouped)
                    {
                        int mincontrib_good = 0;
                        int mincontrib_bad = 0;

                        foreach (var problem in group)
                        {
                            var mincontribsort_mped = problem.Results.Where(x => x.HeuristicRun.Algorithm == AlgorithmType.MINCONTRIBSORT_FIRSTGUESS).Min(x => x.Mped);
                            var min_mped = problem.Results.Where(x => x.HeuristicRun.Algorithm != AlgorithmType.MINCONTRIBSORT_FIRSTGUESS).Min(x => x.Mped);
                            if (mincontribsort_mped <= min_mped)
                            {
                                mincontrib_good++;
                            }
                            else
                            {
                                mincontrib_bad++;
                            }

                        }
                        
                        file.WriteLine(String.Format(CultureInfo.InvariantCulture, "{0} {1} {2} {3} {4}", group.Key.InsertProb, group.Key.SubstituteProb, mincontrib_good, mincontrib_bad, mincontrib_good + mincontrib_bad));
                    }
                }
            }
        }
    }
}
