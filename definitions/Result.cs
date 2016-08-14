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

        public int NumberOfEvalsToObtainSolution { get; private set; }

        public int Mped { get; private set; }

        public static Result create(Problem p, Solution solution, int number_of_evals)
        {
            return new Result(p, solution, number_of_evals);
        }

        // Used by Entity Framework
        private Result(){ }

        private Result(Problem problem, Solution solution, int number_of_evals)
        {
            this.Problem = problem;
            this.Solution = solution;
            this.NumberOfEvalsToObtainSolution = number_of_evals;
            this.Mped = Distance.mped(problem, solution);
        }
    }
}
