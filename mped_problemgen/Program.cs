using at.mschwaig.mped.definitions;
using at.mschwaig.mped.problemgen;
using System.Collections.Generic;
using System.Security.Cryptography;


namespace at.mschwaig.mped.problemgen
{
    class Program
    {
        static List<GeneratedProblem> generateComparisonData()
        {

            List<GeneratedProblem> problems = new List<GeneratedProblem>();
            RandomNumberGenerator r = new RNGCryptoServiceProvider();

            int runId = r.Next<RandomNumberGenerator>(int.MaxValue);

            for (int alphabet_pow = 2; alphabet_pow <= 5; alphabet_pow++)
                for (int length_pow = 4; length_pow <= 4; length_pow++)
                    for (int similarity = 0; similarity <= 8; similarity++)
                    {
                        int alphabet_size = 1 << alphabet_pow;
                        int string_length = 1 << (length_pow * 2);
                        problems.Add(GeneratedProblem.generateProblem(alphabet_size, string_length, similarity / 8.0d, 0.0d, 0.0d, LengthCorrectionPolicy.NO_CORRECTION, r, runId));
                    }

            return problems;
        }

        static void Main(string[] args)
        {
            var res = generateComparisonData();

            using (var ctx = new ThesisDbContext())
            {
                ctx.Problems.AddRange(res);
                ctx.SaveChanges();
            }
        }
    }
}
