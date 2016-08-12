namespace at.mschwaig.mped.definitions
{
    public class Result
    {
        public static Result create(Problem p, Solution solution, int number_of_evals)
        {
            return new Result(p, solution, number_of_evals);
        }

        private Result(Problem problem, Solution solution, int number_of_evals)
        {
            this.Problem = problem;
            this.Solution = solution;
            this.NumberOfEvalsToObtainSolution = number_of_evals;
        }

       public  Problem Problem { get; }

        public Solution Solution { get; }

        public int NumberOfEvalsToObtainSolution { get; }
    }
}
