using at.mschwaig.mped.definitions;
using System;

namespace at.mschwaig.mped.persistence
{
    public abstract class Heuristic
    {
        public HeuristicRun run;

        public Heuristic(AlgorithmType type)
        {
            run = new HeuristicRun(type, DateTime.Now, "");
        }

        public abstract Result applyTo(Problem p);

        public int getMaxEvalNumber(int albhabet_size_a, int albhabet_size_b)
        {
            return albhabet_size_a * albhabet_size_a * albhabet_size_b * albhabet_size_b;
        }
    }
}
