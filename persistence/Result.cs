using System.ComponentModel.DataAnnotations.Schema;
using at.mschwaig.mped.definitions;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace at.mschwaig.mped.persistence
{
    public class Result
    {
        public int ResultId { get; private set; }

        public int ProblemId { get; set; }

        [Required]
        public virtual Problem Problem { get; set; }


        public virtual List<BestSolution> Solutions { get; set; }

        [NotMapped]

        public int HeuristicRunId { get; set; }

        [Required]
        public virtual HeuristicRun HeuristicRun { get; set; }

        // Used by Entity Framework
        private Result(){ }

        public Result(Problem problem, HeuristicRun run)
        {
            this.Problem = problem;
            this.HeuristicRun = run;
            this.Solutions = new List<BestSolution>();
        }
    }
}
