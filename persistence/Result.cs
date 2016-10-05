using System.ComponentModel.DataAnnotations.Schema;
using at.mschwaig.mped.definitions;
using System.ComponentModel.DataAnnotations;

namespace at.mschwaig.mped.persistence
{
    public class Result
    {
        public int ResultId { get; private set; }

        public int ProblemId { get; set; }

        [Required]
        public virtual Problem Problem { get; set; }

        public string PermutationString { get; private set; }

        [NotMapped]
        public  Solution Solution { get
            {
                if (solution_converted != null)
                {
                    return solution_converted;
                }
                else
                {
                    solution_converted = new Solution(PermutationString);
                    return solution_converted;
                }
            }
        }


        public int HeuristicRunId { get; set; }

        [Required]
        public virtual HeuristicRun HeuristicRun { get; set; }

        public int NumberOfEvalsToObtainSolution { get; private set; }

        public int Mped { get; private set; }

        private Solution solution_converted;

        public static Result create(Problem problem, Solution solution, HeuristicRun run, int number_of_evals)
        {
            return new Result(problem, solution, run, number_of_evals);
        }


        // Used by Entity Framework
        private Result(){ }

        private Result(Problem problem, Solution solution, HeuristicRun run, int number_of_evals)
        {
            this.Problem = problem;
            this.solution_converted = solution;
            this.PermutationString = string.Join(",", solution.Permutation); 
            this.HeuristicRun = run;
            this.NumberOfEvalsToObtainSolution = number_of_evals;
            this.Mped = DistanceUtil.mped(problem, solution);
        }
    }
}
