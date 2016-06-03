using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace mped_cs
{
    public class Distance
    {
        public static int ed(string a, string b) {
            int[,] distance_matrix = ed(a, b, (x, y) => x == y);
            return distance_matrix[distance_matrix.GetLength(0) - 1, distance_matrix.GetLength(1) - 1];
        }

        private static int[,] ed(string a, string b, Func<char, char, bool> mapping)
        {
            int[,] distance = new int[a.Length + 1, b.Length + 1];

            for (int i = 0; i < a.Length + 1; i++)
            {
                distance[i, 0] = i;
            }

            for (int j = 0; j < b.Length + 1; j++)
            {
                distance[0, j] = j;
            }

            for (int j = 0; j < b.Length; j++)
            {
                for (int i = 0; i < a.Length; i++)
                {
                    if (mapping(a[i],b[j]))
                    {
                        distance[i + 1, j + 1] = distance[i, j];
                    }
                    else
                    {
                        int delete = distance[i, j + 1] + 1;
                        int insert = distance[i + 1, j] + 1;
                        int substitute = distance[i, j] + 1;
                        distance[i + 1, j + 1] = Math.Min(delete, Math.Min(insert, substitute));
                    }
                }
            }

            return distance;
        }

        public static string visualizeMatch(string a, string b, int[,] distance) {
            // calculate transformation sequence
            int pos_a = a.Length;
            int pos_b = b.Length;

            string c = "";

            do
            {
                int move_left = pos_b - 1 >= 0 ? distance[pos_a, pos_b - 1] - distance[pos_a, pos_b] : Int32.MaxValue;
                int move_up = pos_a - 1 >= 0 ? distance[pos_a - 1, pos_b] - distance[pos_a, pos_b] : Int32.MaxValue;
                int move_diagonal = pos_a - 1 >= 0 && pos_b - 1 >= 0 ? distance[pos_a - 1, pos_b - 1] - distance[pos_a, pos_b] : Int32.MaxValue;

                if (move_left < move_up && move_left < move_diagonal)
                {
                    a = a.Insert(pos_a, "-");
                    c = " " + c;
                    pos_b--;
                }
                else if (move_up < move_left && move_up < move_diagonal)
                {
                    b = b.Insert(pos_b, "-");
                    c = " " + c;
                    pos_a--;
                }
                else
                {
                    if (move_diagonal == 0) c = "*" + c;
                    else c = " " + c;
                    pos_a--;
                    pos_b--;
                }
            } while (pos_a > 0 || pos_b > 0);

            return a + Environment.NewLine + b + Environment.NewLine + c;
        }


        public static int mped(string a, string b, bool verbose = false)
        {
            return mped(AString.create(a), AString.create(b), verbose);
        }

        public static int mped(AString a, AString b, bool verbose = false)
        {
            int minimal_ed = Int32.MaxValue;
            AlphabetMapping minimal_mapping = null;
            Func<AlphabetMapping, int> mtd = getAlphabetMappingEvaluationFunction(a, b);

            foreach (AlphabetMapping m in AlphabetMapping.getAllPossibleMappings(a.getAlphabet(), b.getAlphabet()))
            {
                int edit_distance_for_mapping = mtd(m);
                if (edit_distance_for_mapping < minimal_ed)
                {
                    minimal_ed = edit_distance_for_mapping;
                    minimal_mapping = m;
                }
            }

            if (verbose)
            {
                Console.WriteLine(minimal_mapping);
                int[,] minimal_dist_matrix = ed(a.getString(), b.getString(), minimal_mapping.getMappingFunction());
                Console.WriteLine(visualizeMatch(a.getString(), b.getString(), minimal_dist_matrix));
            }

            return minimal_ed;
        }

        public static Func<AlphabetMapping, int> getAlphabetMappingEvaluationFunction(AString a, AString b)
        {
            return (m) => {
                Func<char, char, bool> mapping = m.getMappingFunction();

                int[,] distance_matrix = ed(a.getString(), b.getString(), mapping);
                return distance_matrix[distance_matrix.GetLength(0) - 1, distance_matrix.GetLength(1) - 1];
            };
        }
    }
}