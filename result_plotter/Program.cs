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
            // evaluateInsertExperiment();
            evaluateCompareExperiment();
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

        static void evaluateCompareExperiment()
        {
            using (var ctx = new ThesisDbContext())
            {
                Experiment ex = ctx.Experiments.Include("Problems.Results.HeuristicRun").Where(x => x.Name == "Compare").First();

                var problem_param_groups = ex.Problems.GroupBy(x => new { x.SubstituteProb, x.a.Length });

                foreach (var group in problem_param_groups)
                {
                    using (StreamWriter file = new StreamWriter("comp-" + group.Key.Length + "-" + group.Key.SubstituteProb + ".txt"))
                    {
                        var res = group.SelectMany(x => x.Results).GroupBy(x => x.HeuristicRun.Algorithm).Select(grp => new {
                            Name = grp.Key,
                            MinMped = grp.Min(x => x.Mped),
                            MaxMped = grp.Max(x => x.Mped),
                            AvgMped = grp.Average(x => x.Mped),
                            MinEvals = grp.Min(x => x.NumberOfEvalsToObtainSolution),
                            MaxEvals = grp.Max(x => x.NumberOfEvalsToObtainSolution),
                            AvgEvals = grp.Average(x => x.NumberOfEvalsToObtainSolution)
                        });

                        foreach (var alg in res)
                        {
                            file.WriteLine(String.Format(CultureInfo.InvariantCulture, "{0} {1} {2} {3} {4} {5} {6}",
                                alg.Name, alg.MinMped, alg.AvgMped, alg.MaxMped, alg.MinEvals, alg.AvgEvals, alg.MaxEvals));
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

                using (StreamWriter file = new StreamWriter("insert_rate_impact.dat"))
                {
                    var grouped = ex.Problems.GroupBy(x => new {  x.SubstituteProb, x.InsertProb });
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
                        
                        file.WriteLine(String.Format(CultureInfo.InvariantCulture, "{0} {1} {2} {3} {4}", group.Key.SubstituteProb, group.Key.InsertProb, mincontrib_good, mincontrib_bad, mincontrib_good + mincontrib_bad));
                    }
                }
            }
        }
    }
}
