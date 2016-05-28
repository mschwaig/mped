using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mped_cs
{
    class Program
    {

        /// <summary>
        /// This method returns an enumeration with all the possible strings that are produced
        /// if we take the alphabet of characters that the string consists of, and map every character
        /// of that alphabet to a character in a different alphabet in all possible ways.
        /// 
        /// For example if the string is "ab" and the alphabet is {'x', 'y'}, 
        /// we see, that the source alphabet is {'a', 'b'} and the following four mappings are possible:
        /// 1) a->x, b->x
        /// 2) a->x, b->y
        /// 3) a->y, b->x,
        /// 4) a->y, b->y
        /// If we apply those four possible mappings to the input string s, they produce the following four strings:
        /// 1) "xx"
        /// 2) "xy"
        /// 3) "yx"
        /// 4) "yy"
        /// This list is what the method would return as an enueration.
        /// </summary>
        /// <param name="s">The string, which is mapped to an alphabet.</param>
        /// <param name="destination_alphabet">The alphabet that the string is mapped to.</param>
        /// <returns>An enumeration with all the possible mappings of a given string to a given alphabet.</returns>
        public static IEnumerable<string> mappings(string s, char[] destination_alphabet)
        {
            char[] source_alphabet = s.Distinct().OrderBy(x => x).ToArray();

            SortedDictionary<char, int> reverse_mapping = new SortedDictionary<char, int>();

            for (int i = 0; i < source_alphabet.Length; i++)
            {
                reverse_mapping.Add(source_alphabet[i], i);
            }

            int[] mapping = new int[source_alphabet.Length];

            int pos;

            while (true)
            {
                yield return String.Concat(s.Select(x => destination_alphabet[mapping[reverse_mapping[x]]]));

                pos = source_alphabet.Length - 1;

                while (pos >= 0 && mapping[pos] + 1 >= destination_alphabet.Length) {
                    mapping[pos] = 0;
                    pos = pos - 1;
                }

                if (pos < 0) yield break;
                mapping[pos] = mapping[pos] + 1;
            }
        }



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
                int move_left = pos_b - 1 >= 0 ? distance[pos_a, pos_b - 1] - distance[pos_a, pos_b] : 999;
                int move_up = pos_a - 1 >= 0 ? distance[pos_a - 1, pos_b] - distance[pos_a, pos_b] : 999;
                int move_diagonal = pos_a - 1 >= 0 && pos_b - 1 >= 0 ? distance[pos_a - 1, pos_b - 1] - distance[pos_a, pos_b] : 999;

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
            mappingsTest();

            edTest();

            mpedTest();
        }

        /// <summary>
        /// Tests for the mapping method.
        /// </summary>
        static void mappingsTest() {
            List<string> mappings1x1 = mappings("a", new char[] { 'x'}).ToList();

            Debug.Assert(mappings1x1.Contains("x"));
            Debug.Assert(mappings1x1.Count == 1);

            List<string> mappings1x2 = mappings("a", new char[] { 'x', 'y' }).ToList();

            Debug.Assert(mappings1x2.Contains("x"));
            Debug.Assert(mappings1x2.Contains("y"));
            Debug.Assert(mappings1x2.Count == 2);

            List<string> mappings2x1 = mappings("ab", new char[] {'x'}).ToList();

            Debug.Assert(mappings2x1.Contains("xx"));
            Debug.Assert(mappings2x1.Count == 1);

            List<string> mappings2x2 = mappings("ab", new char[] { 'x', 'y' }).ToList();

            Debug.Assert(mappings2x2.Contains("xx"));
            Debug.Assert(mappings2x2.Contains("xy"));
            Debug.Assert(mappings2x2.Contains("yx"));
            Debug.Assert(mappings2x2.Contains("yy"));
            Debug.Assert(mappings2x2.Count == 4);
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