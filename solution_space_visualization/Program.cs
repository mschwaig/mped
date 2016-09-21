using at.mschwaig.mped.definitions;
using at.mschwaig.mped.mincontribsort;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace at.mschwaig.mped.solution_space_visualization
{
    class Program
    {
        static void Main(string[] args)
        {
            int alphabet_size = 8;
            int string_length = 128;

            Problem p = Problem.generateProblem(null, alphabet_size, string_length, 0.0d, 0.1d, 0.1d, LengthCorrectionPolicy.PREPEND_CORRECTION, new RNGCryptoServiceProvider(), 0);

            CharacterMapping[,] one_to_one_mappings = MinContribSort.generateMatrixOfOneToOneMappings(p);

            var sorter = new ListBasedSorting();
            var result_list = sorter.sortByMinMaxContrib(one_to_one_mappings);
            var sorted_solution_space = result_list.Select(x => x.Item2).SelectMany(x => x).Select(x => Tuple.Create(x, Distance.mped(p, x))).OrderBy(x => x.Item2);
            var best = sorted_solution_space.First().Item1;
            
            int[,] matrix = new int[string_length, alphabet_size];

            foreach (var t in sorted_solution_space)
            {
                matrix[t.Item2, best.getDistanceTo(t.Item1)] += 1;
            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"results.txt"))
            {
                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    for (int j = 0; j < matrix.GetLength(1); j++)
                    {
                        file.Write(matrix[i, j] + " ");
                    }

                    file.WriteLine();
                }
            }
        }
    }
}
