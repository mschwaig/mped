using System;
using System.Diagnostics;

namespace at.mschwaig.mped.definitions
{
    public abstract class Heuristic
    {
        public HeuristicRun run;

        public Heuristic(HeuristicRun.AlgorithmType type)
        {
            run = new HeuristicRun(type, DateTime.Now, "", "", true);
            using (var ctx = new ThesisDbContext())
            {
                ctx.HeuristicRun.Add(run);
                ctx.SaveChanges();
            }
        }

        public abstract Result applyTo(Problem p);
    }
}
