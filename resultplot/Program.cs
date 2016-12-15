using at.mschwaig.mped.definitions;
using System;
using System.IO;
using System.Linq;
using System.Globalization;
using at.mschwaig.mped.persistence;
using System.Reflection;
using System.ComponentModel;
using at.mschwaig.mped.resultplotter;
using at.mschwaig.mped.resultplot;
using System.Collections.Generic;

namespace at.mschwaig.mped.resultplotter
{
    class Program
    {
        static void Main(string[] args)
        {
            evaluateInsertExperiment();
            evaluateCompareExperimentLineGraph(0);
            evaluateCompareExperimentTable();
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

        static void evaluateCompareExperimentLineGraph(int index)
        {
            using (var ctx = new ThesisDbContext())
            {
                Experiment ex = ctx.Experiments.Include("Problems.Results.Solutions").Include("Problems.Results.HeuristicRun").Where(x => x.Name == "FullComparison").Single();

                var problem_param_groups = ex.Problems.GroupBy(x => new { Correlation = 1 - x.SubstituteProb, x.a.Length });

                foreach (var group in problem_param_groups)
                {
                    using (StreamWriter file = new StreamWriter(
                        String.Format(CultureInfo.InvariantCulture, "linecomp_{0}_{1}.dat", group.Key.Length, group.Key.Correlation)))
                    {
                        var res = group.SelectMany(x => x.Results).GroupBy(x => x.HeuristicRun.Algorithm).OrderBy(x => x.Key);

                        foreach (var alg in res)
                        {
                            file.WriteLine("\"" + alg.Key.GetDescription() + "\"");

                            var resultset = alg.ElementAt(index);

                            var filtered = resultset.Solutions.OrderBy(x => x.EvalCount).removedNeighouringDuplicateValues(x => x.Mped);

                            // fix to limit evaluation count for evolution stategy
                            // which does not terminate properly
                            int es_max_eval_count = 0;
                            int es_min_mped = 0;
                            if (alg.Key == AlgorithmType.HL_ES) {
                                es_max_eval_count = resultset.Problem.a.Length * resultset.Problem.b.Length;
                                es_max_eval_count = es_max_eval_count * es_max_eval_count;
                                filtered = filtered.Where(x => x.EvalCount <= es_max_eval_count);
                                var terminating_entry = filtered.Last(x => x.EvalCount <= es_max_eval_count);
                                es_min_mped = terminating_entry.Mped;
                            }

                            foreach (var entry in filtered) {
                                file.WriteLine(entry.EvalCount + " " + entry.Mped);
                            }
                            if (alg.Key == AlgorithmType.HL_ES)
                            {
                                file.WriteLine(es_max_eval_count + " " + es_min_mped);
                            }
                            file.WriteLine();
                            file.WriteLine();
                        }
                    }
                }
            }
        }

        static void evaluateCompareExperimentTable()
        {
            using (var ctx = new ThesisDbContext())
            {
                Experiment ex = ctx.Experiments.Include("Problems.Results.Solutions").Include("Problems.Results.HeuristicRun").Where(x => x.Name == "FullComparison").Single();

                var problem_param_groups = ex.Problems.GroupBy(x => new { Correlation = 1 - x.SubstituteProb, x.a.Length });

                foreach (var group in problem_param_groups)
                {
                    using (StreamWriter file = new StreamWriter(
                        String.Format(CultureInfo.InvariantCulture, "tblcomp_{0}_{1}.dat", group.Key.Length, group.Key.Correlation)))
                    {
                        var problem_results = group.SelectMany(x => x.Results).Select(x => x.Solutions.OrderBy(y => y.Mped).ThenBy(y => y.EvalCount).First());

                        var results = new List<TableRow>();

                        foreach (var alg_res in problem_results.GroupBy(x => x.Result.HeuristicRun.Algorithm))
                        {
                            // solution for problem - best solution for problem
                            var differences = alg_res.Select(s => s.Mped - s.Result.Problem.Results.Select(y => y.Solutions.Min(z => z.Mped)).Min());

                            double avg = alg_res.Average(x => x.Mped);
                            double avg_diff = differences.Average();
                            double stddev_diff = differences.computeStandardDeviation();

                            results.Add(new TableRow(alg_res.Key, avg, avg_diff, stddev_diff));


                        }

                        foreach (var r in results.OrderBy(x => x.AverageDiff).ThenBy(x => x.Algorithm.GetDescription()))
                        {
                            file.WriteLine(String.Format(CultureInfo.InvariantCulture, "{0,-25} {1,16} {2,16:0.0} {3,20:0.0000}",
                                                                        r.Algorithm.GetDescription(), r.Average, r.AverageDiff, r.StdDevDiff));
                            file.WriteLine();
                        }
                    }
                }
            }
        }

        static void evaluateCompareExperimentFullTable()
        {
            using (var ctx = new ThesisDbContext())
            {
                Experiment ex = ctx.Experiments.Include("Problems.Results.Solutions").Include("Problems.Results.HeuristicRun").Where(x => x.Name == "FullComparison").Single();

                var problem_param_groups = ex.Problems.GroupBy(x => new { Correlation = 1 - x.SubstituteProb, x.a.Length });

                foreach (var group in problem_param_groups)
                {
                    using (StreamWriter file = new StreamWriter(
                        String.Format(CultureInfo.InvariantCulture, "tblcomp_{0}_{1}.dat", group.Key.Length, group.Key.Correlation)))
                    {

                        var problem_results = group.Select(x => new
                        {
                            problem = x,
                            lowestMped = x.Results.Select(y => y.Solutions.Min(z => z.Mped)).Min(),
                            algorithm_results = x.Results.Select(y => new { alg = y.HeuristicRun.Algorithm, mped = y.Solutions.Min(z => z.Mped) })
                        });

                        int problem_index = 1;

                        foreach (var result in problem_results)
                        {
                            foreach (var alg_res in result.algorithm_results)
                            {
                                file.WriteLine("{0}, {1}, {2}, {3}", "Problem " + problem_index, alg_res.alg, alg_res.mped, result.lowestMped);
                            }

                            problem_index++;
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
                            var mincontribsort_mped = problem.Results.Where(x => x.HeuristicRun.Algorithm == AlgorithmType.CONTRIBUTION_SORTING_GUESS).Min(x => x.Solutions.Single().Mped);
                            var min_mped = problem.Results.Where(x => x.HeuristicRun.Algorithm != AlgorithmType.CONTRIBUTION_SORTING_GUESS).Min(x => x.Solutions.Single().Mped);
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

        private class TableRow
        {
            public double Average { get; }
            public double AverageDiff { get; }
            public double StdDevDiff { get; }

            public AlgorithmType Algorithm { get; }

            public TableRow(AlgorithmType alg, double average, double average_diff, double stddev_diff)
            {
                this.Algorithm = alg;
                this.Average = average;
                this.AverageDiff = average_diff;
                this.StdDevDiff = stddev_diff;
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
