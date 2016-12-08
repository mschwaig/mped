using at.mschwaig.mped.definitions;
using System;
using System.IO;
using System.Linq;
using System.Globalization;
using at.mschwaig.mped.persistence;
using System.Reflection;
using System.ComponentModel;
using at.mschwaig.mped.resultplotter;

namespace at.mschwaig.mped.resultplotter
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
                            file.WriteLine("{0} {1} {2}", result.HeuristicRun.Algorithm.ToString(), result.Solutions.Single().Mped, result.Solutions.Single().EvalCount);
                        }
                    }
                }
            }
        }

        static void evaluateCompareExperiment()
        {
            using (var ctx = new ThesisDbContext())
            {
                Experiment ex = ctx.Experiments.Include("Problems.Results.Solutions").Include("Problems.Results.HeuristicRun").Where(x => x.Name == "Compare").First();

                var problem_param_groups = ex.Problems.GroupBy(x => new { Correlation = 1 - x.SubstituteProb, x.a.Length });

                foreach (var group in problem_param_groups)
                {
                    using (StreamWriter file = new StreamWriter("comp_" + group.Key.Length + "_" + group.Key.Correlation + ".dat"))
                    {
                        var res = group.SelectMany(x => x.Results).GroupBy(x => x.HeuristicRun.Algorithm).Select(grp => new {
                            Enum = grp.Key,
                            Name = grp.Key.GetDescription(),
                            MinMped = grp.Min(x => x.Solutions.Single().Mped),
                            MaxMped = grp.Max(x => x.Solutions.Single().Mped),
                            AvgMped = grp.Average(x => x.Solutions.Single().Mped),
                            MinEvals = grp.Min(x => x.Solutions.Single().EvalCount),
                            MaxEvals = grp.Max(x => x.Solutions.Single().EvalCount),
                            AvgEvals = grp.Average(x => x.Solutions.Single().EvalCount)
                        }).OrderBy(x => x.Name);

                        foreach (var alg in res)
                        {
                            file.WriteLine(String.Format(CultureInfo.InvariantCulture, "{0,-25} {1,11} {2,11} {3,11} {4,11} {5,11} {6,11}",
                                alg.Name, alg.MinMped, alg.AvgMped, alg.MaxMped, alg.MinEvals, alg.AvgEvals, alg.MaxEvals));
                            file.WriteLine();
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
                    var grouped = ex.Problems.GroupBy(x => new { x.SubstituteProb, x.InsertProb });
                    foreach (var group in grouped)
                    {
                        int mincontrib_good = 0;
                        int mincontrib_bad = 0;

                        foreach (var problem in group)
                        {
                            var mincontribsort_mped = problem.Results.Where(x => x.HeuristicRun.Algorithm == AlgorithmType.MINCONTRIBSORT_FIRSTGUESS).Min(x => x.Solutions.Single().Mped);
                            var min_mped = problem.Results.Where(x => x.HeuristicRun.Algorithm != AlgorithmType.MINCONTRIBSORT_FIRSTGUESS).Min(x => x.Solutions.Single().Mped);
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

    static class Extensions {
        public static string GetDescription(this AlgorithmType value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute description = ((DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false)).FirstOrDefault();

            return description != null ? description.Description : value.ToString();
        }
    }
}
