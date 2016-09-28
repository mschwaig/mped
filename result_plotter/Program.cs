using at.mschwaig.mped.definitions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;

namespace result_plotter
{
    class Program
    {
        static void Main(string[] args)
        {


            // for all experiments which have algorithms with complete results
            // aggregate the results over the algorithms
            // and make a scatter plot with evaluations and obtained result
            using (var ctx = new ThesisDbContext())
            {
                Experiment ex = ctx.Experiments.Where(x => x.Name == "Insert").First();
                evaluateExperimentResultsByProblem(ex);



                List<Problem> problemList = ctx.Problems.Where(x => x.Experiement.Id == ex.Id).ToList();
                var problemIds = problemList.Select(x => x.Id);
                var problemCount = problemIds.Count();
                var groupedResults = ctx.Results.Where(x => problemIds.Contains(x.Problem.Id)).GroupBy(x => new { x.Run.Algorithm, x.Run.Parameters}).Where(x => x.Count() == problemCount);
            }
        }

        static void evaluateExperimentResultsByProblem(Experiment ex)
        {
            using (var ctx = new ThesisDbContext())
            {
                var results_by_problem = ctx.Results.Include(x => x.Run).Where(x => x.Problem.Experiement.Id == ex.Id).GroupBy(x => x.Problem).ToList();

                foreach (var problem in results_by_problem)
                {
                    using (StreamWriter file = new StreamWriter(problem.Key.Id + ".txt"))
                    {
                        foreach (var result in problem)
                        {
                            ctx.Entry(result).Reference(r => r.Run).Load();
                            file.WriteLine("{0} {1} {2}", result.Run.Algorithm.ToString(), result.Mped, result.NumberOfEvalsToObtainSolution);
                        }
                    }
                }
            }
        }
    }
}
