using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace at.mschwaig.mped.definitions
{
    [Table("RESULTS")]
    public class Result
    {
        [Key]
        public int Id { get; private set; }

        public Problem Problem { get; private set; }

        public Solution Solution { get; private set; }

        public HeuristicRun Run { get; private set; }

        public int NumberOfEvalsToObtainSolution { get; private set; }

        public int Mped { get; private set; }

        public static Result create(Problem problem, Solution solution, HeuristicRun run, int number_of_evals)
        {
            return new Result(problem, solution, run, number_of_evals);
        }

        // Used by Entity Framework
        private Result(){ }

        private Result(Problem problem, Solution solution, HeuristicRun run, int number_of_evals)
        {
            this.Problem = problem;
            this.Solution = solution;
            this.Run = run;
            this.NumberOfEvalsToObtainSolution = number_of_evals;
            this.Mped = Distance.mped(problem, solution);
        }
    }
}
