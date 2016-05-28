using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace mped_cs
{
    class Program
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
            int[,] minimal_dist_matrix = null;

            foreach (AlphabetMapping m in AlphabetMapping.getMappings(a.getAlphabet(), b.getAlphabet()))
            {
                Func<char, char, bool> mapping = m.getMappingFunction();

                int[,] distance_matrix = ed(a.getString(), b.getString(), mapping);
                int edit_distance_for_mapping = distance_matrix[distance_matrix.GetLength(0) - 1, distance_matrix.GetLength(1) - 1];
                if (edit_distance_for_mapping < minimal_ed)
                {
                    minimal_ed = edit_distance_for_mapping;
                    minimal_mapping = m;
                    minimal_dist_matrix = distance_matrix;
                }
            }

            if (verbose)
            {
                Console.WriteLine(minimal_mapping);
                Console.WriteLine(visualizeMatch(a.getString(), b.getString(), minimal_dist_matrix));
            }

            return minimal_ed;
        }

        static void Main(string[] args)
        {
            edTest();

            mpedTest();
        }

        /// <summary>
        /// Tests for the ed method.
        /// </summary>
        static void edTest()
        {
            Debug.Assert(ed("a","a") == 0);
            Debug.Assert(ed("a", "b") == 1);
            Debug.Assert(ed("a", "") == 1);
            Debug.Assert(ed("", "a") == 1);
            Debug.Assert(ed("ab", "b") == 1);
            Debug.Assert(ed("ab", "a") == 1);
            Debug.Assert(ed("a", "ab") == 1);
            Debug.Assert(ed("b", "ab") == 1);
            Debug.Assert(ed("ab", "ab") == 0);
            Debug.Assert(ed("aa", "ab") == 1);
        }

        /// <summary>
        /// Tests for MPED method.
        /// </summary>
        static void mpedTest() {
            Debug.Assert(mped("a", "x") == 0);
            Debug.Assert(mped("ab","xy") == 0);
            Debug.Assert(mped("aa", "xy") == 1);
            Debug.Assert(mped("ab", "xx") == 1);
            Debug.Assert(mped("abc", "abc") == 0);
            Debug.Assert(mped("AAABCCDDCAC", "1102322033") == 4);
            Debug.Assert(mped("AAABCCDDCAA", "2210500155") == 5);
            // Debug.Assert(mped("AAABCCDCADD", "BABAEFEAFAD", true) == 5); // todo: find source? check this manually
            Debug.Assert(mped(AString.create("AAABCCDDCAA", 2), AString.create("2210500155", 2)) == 3);
            Debug.Assert(mped(AString.create("ABABD966GDBDA", 2), AString.create("1312X1XX122KK", 1)) == 7);
        }
    }
}