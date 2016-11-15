using at.mschwaig.mped.definitions;
using System;

namespace at.mschwaig.mped.persistence
{
    public abstract class Heuristic
    {
        public HeuristicRun run;

        public Heuristic(AlgorithmType type)
        {
            run = new HeuristicRun(type, DateTime.Now, "", "", true);
            using (var ctx = new ThesisDbContext())
            {
                ctx.HeuristicRun.Add(run);
                ctx.SaveChanges();
            }
        }

        public abstract Result applyTo(Problem p, int max_evaluation_number = 0);
    }
}
