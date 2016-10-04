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
                    foreach (var problem in ex.Problems)
                    {
                        var mincontribsort_mped = problem.Results.Where(x => x.HeuristicRun.Algorithm == HeuristicRun.AlgorithmType.MINCONTRIBSORT_FIRSTGUESS).Select(x => x.Mped).First();
                        var min_mped = problem.Results.Where(x => x.HeuristicRun.Algorithm != HeuristicRun.AlgorithmType.MINCONTRIBSORT_FIRSTGUESS).Min(x=> x.Mped);
                        file.WriteLine("{0} {1} {2}", problem.InsertProb, problem.SubstituteProb, mincontribsort_mped <= min_mped ? 1 : 0);
                    }
                }
            }
        }
    }
}
