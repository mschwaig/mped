using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

using at.mschwaig.mped.definitions;

namespace at.mschwaig.mped.mincontribsort
{
    class Program
    {

        static ExhaustiveExperimentResult processResultOfExhaustiveTest(Problem p, IEnumerable<Tuple<int, IEnumerable<Solution>>> result, int alphabet_size)
        {
            int minimum_ed = Int32.MaxValue;
            Solution minimum_mapping = null;
            int min_ed_contribution = Int32.MaxValue;
            int max_eval_counter = alphabet_size* alphabet_size;
            int max_evals_to_find_min = Int32.MaxValue;
            int evals_to_prove_min = Int32.MaxValue;

            foreach (var group in result)
            {
                max_eval_counter += group.Item2.Count();

                if (group.Item1 <= minimum_ed)
                {
                    evals_to_prove_min = max_eval_counter;
                }

                foreach (var r in group.Item2)
                {
                    int dist = Distance.mped(p, r);
                    if (dist < minimum_ed)
                    {
                        minimum_ed = dist;
                        minimum_mapping = r;
                        min_ed_contribution = group.Item1;
                        max_evals_to_find_min = max_eval_counter;
                    }
                }
            }

            return new ExhaustiveExperimentResult(max_evals_to_find_min, evals_to_prove_min, minimum_ed);
        }

        static void Main(string[] args)
        {
            int alphabet_size = 6;
            int length = 100;
                 
            using (System.IO.StreamWriter file = new System.IO.StreamWriter("results.txt"))
            {

                file.WriteLine(String.Format("# correlation, max evals to find min, evals to prove min, mped. alphabet size: {0}, string length: {1}", alphabet_size, length));

                for (int correlation_step = 0; correlation_step <= 10; correlation_step++)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        double correlation = 0.1d * correlation_step;

                        Problem p = Problem.generateProblem(null, alphabet_size, length, correlation, 0.0d, 0.0d, LengthCorrectionPolicy.NO_CORRECTION, new RNGCryptoServiceProvider(), 0);

                        CharacterMapping[,] one_to_one_mappings = MinContribSort.generateMatrixOfOneToOneMappings(p);

                        var sorter = new ListBasedSorting();
                        var result_list = sorter.sortByMinMaxContrib(one_to_one_mappings);

                        var result = processResultOfExhaustiveTest(p, result_list, alphabet_size);

                        file.WriteLine(String.Format("{0} {1} {2} {3}", correlation,
                            result.max_evals_to_find_min, result.evals_to_prove_min, result.mped));
                    }
                }
            }
        }


        public static void printMatrix(char[]a, char[]b, CharacterMapping[,] one_to_one_mappings)
        {
            Console.WriteLine("   " + String.Join("   ", b));

            for (int i = 0; i < a.Length; i++)
            {
                Console.Write(a[i] + " ");
                for (int j = 0; j < b.Length; j++)
                {
                     Console.Write(String.Format("{0,3} ", one_to_one_mappings[i, j].min_ed));
                }
                Console.WriteLine();
            }
        }



        public class ExhaustiveExperimentResult
        {
            public int max_evals_to_find_min, evals_to_prove_min, mped;

            public ExhaustiveExperimentResult(int max_evals_to_find_min, int evals_to_prove_min, int mped)
            {
                this.max_evals_to_find_min = max_evals_to_find_min;
                this.evals_to_prove_min = evals_to_prove_min;
                this.mped = mped;
            }
        }
    }
}
