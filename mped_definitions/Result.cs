﻿namespace at.mschwaig.mped.definitions
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

        Problem Problem { get; }

        Solution Solution { get; }

        int NumberOfEvalsToObtainSolution { get; }
    }
}