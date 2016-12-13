using System.Collections.Generic;
using System.Security.Cryptography;

using at.mschwaig.mped.definitions;
using at.mschwaig.mped.persistence;

namespace at.mschwaig.mped.problemgen
{
    class Program
    {
        static void generateInsertExperiment()
        {
            Experiment experiment = new Experiment("Insert");

            List<Problem> problems = new List<Problem>();
            RandomNumberGenerator r = new RNGCryptoServiceProvider();

            int runId = r.Next<RandomNumberGenerator>(int.MaxValue);

            int step_count = 16;

            double[] edit_probabilities = new double[step_count];
            double max_edit_prob = 1.0;
            for (int i = 0; i < step_count; i++)
            {
                edit_probabilities[i] = max_edit_prob * i / (step_count - 1);
            }
            double max_insert_prob = 0.3;
            double[] insert_probabilities = new double[step_count];
            for (int i = 0; i < step_count; i++)
            {
                insert_probabilities[i] = max_insert_prob * i / (step_count - 1);
            }

            foreach (var edit_probability in edit_probabilities)
            foreach (var insert_probability in insert_probabilities)
            if(edit_probability + insert_probability <= 1.0d)
            {
                for (int i = 0; i < 5; i++)
                {
                    problems.Add(Problem.generateProblem(experiment, 8, 256, edit_probability, insert_probability, .0d, LengthCorrectionPolicy.APPEND_CORRECTION, r, runId));
                }
            }

            using (var ctx = new ThesisDbContext())
            {
                ctx.Problems.AddRange(problems);
                ctx.Experiments.Add(experiment);
                ctx.SaveChanges();
            }
        }



        static void generateCompareExperiment(string name, int repetitions)
        {
            Experiment experiment = new Experiment(name);

            List<Problem> problems = new List<Problem>();
            RandomNumberGenerator r = new RNGCryptoServiceProvider();

            int runId = r.Next<RandomNumberGenerator>(int.MaxValue);
            
            int[] alphabet_sizes = { 8, 12, 16 };

            foreach (var alphabet_size in alphabet_sizes)
            {
                for (int i = 0; i < repetitions; i++)
                {
                    problems.Add(Problem.generateProblem(experiment, alphabet_size, 256, 1.0d, .0d, .0d, LengthCorrectionPolicy.NO_CORRECTION, r, runId));
                }

                for (int i = 0; i < repetitions; i++)
                {
                    problems.Add(Problem.generateProblem(experiment, alphabet_size, 256, 0.5d, .0d, .0d, LengthCorrectionPolicy.NO_CORRECTION, r, runId));
                }

                for (int i = 0; i < repetitions; i++)
                {
                    problems.Add(Problem.generateProblem(experiment, alphabet_size, 256, 0.0d, .0d, .0d, LengthCorrectionPolicy.NO_CORRECTION, r, runId));
                }
            }




            using (var ctx = new ThesisDbContext())
            {
                ctx.Problems.AddRange(problems);
                ctx.Experiments.Add(experiment);
                ctx.SaveChanges();
            }
        }

        static void generateFullComparisonExperiment()
        {
            generateCompareExperiment("FullComparison", 1);
        }


        static void Main(string[] args)
        {
            generateInsertExperiment();
            generateFullComparisonExperiment();
        }
    }
}
 