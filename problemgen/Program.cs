using at.mschwaig.mped.definitions;
using System.Collections.Generic;
using System.Security.Cryptography;


namespace at.mschwaig.mped.problemgen
{
    class Program
    {
        static void generateExhaustiveComparisonExperiment()
        {
            Experiment experiment = new Experiment("ExhaustiveComparison");

            List<Problem> problems = new List<Problem>();
            RandomNumberGenerator r = new RNGCryptoServiceProvider();

            int runId = r.Next<RandomNumberGenerator>(int.MaxValue);

            int[] alphabet_sizes = { 4, 6, 8, 10, 12, 16, 24, 32, 42 };
            int[] string_lengths = { 256 };
            double[] edit_probabilities = { .0d, .2d, .4d, .6d, .8d, 1.0d };
            double[] insert_probabilities = { .0d, .2d, .4d, .6d, .8d };
            double[] delete_probabilities = { .0d, .2d, .4d, .6d, .8d };

            foreach (var alphabet_size in alphabet_sizes)
            foreach (var string_length in string_lengths)
            foreach (var edit_probability in edit_probabilities)
            foreach (var insert_probability in insert_probabilities)
            foreach (var delete_probability in delete_probabilities)
            {
                if (!(edit_probability + insert_probability + delete_probability > 1.0d))
                {
                    problems.Add(Problem.generateProblem(experiment, alphabet_size, string_length, edit_probability, insert_probability, delete_probability, LengthCorrectionPolicy.NO_CORRECTION, r, runId));
                }
                if (!(edit_probability + insert_probability + delete_probability > 1.0d) && insert_probability <= .5d && delete_probability <= .5d)
                {
                    problems.Add(Problem.generateProblem(experiment, alphabet_size, string_length, edit_probability, insert_probability, delete_probability, LengthCorrectionPolicy.PREPEND_CORRECTION, r, runId));
                    problems.Add(Problem.generateProblem(experiment, alphabet_size, string_length, edit_probability, insert_probability, delete_probability, LengthCorrectionPolicy.APPEND_CORRECTION, r, runId));
                }
                if (!(edit_probability > 1.0d) && delete_probability == .0d && insert_probability == .0d)
                {
                    // NOTE: equally distributed insertions and deletions cancel each other out
                    problems.Add(Problem.generateProblem(experiment, alphabet_size, string_length, edit_probability, insert_probability, insert_probability, LengthCorrectionPolicy.DISTRIBUTE_CORRECTION, r, runId));
                }
            }


            using (var ctx = new ThesisDbContext())
            {
                ctx.Problems.AddRange(problems);
                ctx.Experiments.Add(experiment);
                ctx.SaveChanges();
            }
        }

        static void generateInsertExperiment()
        {
            Experiment experiment = new Experiment("Insert");

            List<Problem> problems = new List<Problem>();
            RandomNumberGenerator r = new RNGCryptoServiceProvider();

            int runId = r.Next<RandomNumberGenerator>(int.MaxValue);

            double[] edit_probabilities = { .0d, .2d, .4d, .6d, .8d, 1.0d };
            double[] insert_probabilities = { .0d, .2d, .4d, .6d, .8d };

            foreach (var edit_probability in edit_probabilities)
            foreach (var insert_probability in insert_probabilities)
            if(edit_probability + insert_probability <= 1.0d)
            { 
                problems.Add(Problem.generateProblem(experiment, 10, 256, edit_probability, insert_probability, .0d, LengthCorrectionPolicy.APPEND_CORRECTION, r, runId));
            }




            using (var ctx = new ThesisDbContext())
            {
                ctx.Problems.AddRange(problems);
                ctx.Experiments.Add(experiment);
                ctx.SaveChanges();
            }
        }


        static void generateSizeScalingExperiment()
        {
            Experiment experiment = new Experiment("SizeScaling");

            List<Problem> problems = new List<Problem>();
            RandomNumberGenerator r = new RNGCryptoServiceProvider();

            int runId = r.Next<RandomNumberGenerator>(int.MaxValue);

            int[] alphabet_sizes = { 4, 6, 8, 10, 12, 16, 24, 32, 42 };

            foreach (var alphabet_size in alphabet_sizes)
            {
                problems.Add(Problem.generateProblem(experiment, alphabet_size, 256, 1.0d, .0d, .0d, LengthCorrectionPolicy.NO_CORRECTION, r, runId));
            }




            using (var ctx = new ThesisDbContext())
            {
                ctx.Problems.AddRange(problems);
                ctx.Experiments.Add(experiment);
                ctx.SaveChanges();
            }
        }


        static void Main(string[] args)
        {
            // createExhaustiveComparisonExperiment()
            generateInsertExperiment();
            // generateSizeScalingExperiment();
        }
    }
}
 