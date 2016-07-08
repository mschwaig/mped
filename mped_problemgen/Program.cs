using at.mschwaig.mped.definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace at.mschwaig.mped.problemgen
{
    class Program
    {
        static List<Problem> generateComparisonData()
        {

            List<Problem> problems = new List<Problem>();
            RandomNumberGenerator r = new RNGCryptoServiceProvider();

            for (int alphabet_pow = 2; alphabet_pow <= 5; alphabet_pow++)
                for (int length_pow = 4; length_pow <= 4; length_pow++)
                    for (int similarity = 0; similarity <= 8; similarity++)
                    {
                        int alphabet_size = 1 << alphabet_pow;
                        int string_length = 1 << (length_pow * 2);
                        int related_char_count = string_length * similarity / 8;
                        problems.Add(ProblemData.generateProblem(alphabet_size, string_length, related_char_count, r));
                    }

            return problems;
        }

        static void Main(string[] args)
        {
            var res = generateComparisonData();

            var asdf = Newtonsoft.Json.JsonConvert.SerializeObject(res);
            System.IO.File.WriteAllText("test.txt", asdf);
        }
    }
}
