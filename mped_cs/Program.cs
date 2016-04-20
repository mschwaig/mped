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
            char[] alphabet = a.Distinct().Union(b.Distinct()).OrderBy(x => x).ToArray();
            Dictionary <char, int> reverse = alphabet.ToDictionary( x => x, x => Array.IndexOf(alphabet, x));

            int[] mapped_a = a.Select(x => reverse[x]).ToArray();
            int[] mapped_b = b.Select(x => reverse[x]).ToArray();
            return ed(mapped_a, mapped_b);
        }

        public static int ed(int[] a, int[] b)
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
                    if (a[i] == b[j])
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
           
            int result = distance[a.Length, b.Length];
            return result;
        }


        public static int mped(string a, string b)
        {
            char[] alphabet_a = a.Distinct().OrderBy(x => x).ToArray();
            char[] alphabet_b = b.Distinct().OrderBy(x => x).ToArray();

            int minimal_ed = Int32.MaxValue;

            foreach (string mapping in mappings(a, alphabet_b))
            {
                int edit_distance_for_mapping = ed(mapping, b);
                if (edit_distance_for_mapping < minimal_ed)
                {
                    minimal_ed = edit_distance_for_mapping;
                }
            }

            return minimal_ed;
        }
        
        private static IEnumerable<IEnumerable<IEnumerable<char>>> mark_subset(int[] positions, int[] marker_counts, int pos, int highest_used_marker, char[] alphabet)
        {
            if (pos == positions.Length)
            {
                yield return create_enumerable(positions, highest_used_marker, alphabet);

            } else for (int marker = 1; marker <= highest_used_marker + 1 && marker < marker_counts.Length; marker++)
            {
                if (marker_counts[marker] > 0) {
                    positions[pos] = marker;
                    marker_counts[marker]--;
                    if (marker == highest_used_marker + 1)
                    {
                        var subset = mark_subset(positions, marker_counts, pos + 1, highest_used_marker + 1, alphabet);
                        foreach (var v in subset) {
                            yield return v;
                        }
                    }
                    else {
                        var subset = mark_subset(positions, marker_counts, pos + 1, highest_used_marker, alphabet);
                        foreach (var v in subset)
                        {
                            yield return v;
                        }
                    }
                    marker_counts[marker]++;
                }
            }
        }

        private static IEnumerable<IEnumerable<char>> create_enumerable(int[] positions, int highest_used_marker, char[] alphabet) {
            List<List<char>> ret = new List<List<char>>();
            for (int i = 1; i <= highest_used_marker; i++) {
                ret.Add(new List<char>());
            }

            for (int i = 0; i < positions.Length; i++) {
                ret[positions[i] - 1].Add(alphabet[i]);
            }

            return ret;
        }

        public static IEnumerable<IEnumerable<IEnumerable<char>>> findDisjointSetOfSubsetsWithTargetCardinality(char[] alphabet, int subset_cardinality) {
            int[] markings = new int[alphabet.Length];
            // if you want a maximum cardinality instead of a target cardinality, then
            // replace
            // (alphabet.Length + subset_cardinality - 1) / subset_cardinality + 1
            // with
            // (alphabet.Length + subset_cardinality) / subset_cardinality + 1
            int[] marker_counts = new int[(alphabet.Length + subset_cardinality - 1) / subset_cardinality + 1];
            for (int i = 1; i < marker_counts.Length; i++)
            {
                marker_counts[i] = subset_cardinality;
            }
            return mark_subset(markings, marker_counts, 0, 0, alphabet);
        }

        public static string setOfSetsOfCharsToString(IEnumerable<IEnumerable<char>> set)
        {
            return "{" + String.Join(",", set.ToList().Select(y => "{" + String.Join(",", y) + "}")) + "}";
        }

        static void Main(string[] args)
        {
            findDisjointSetOfSubsetsWithTargetCardinalityTest();

            mappingsTest();

            edTest();

            mpedTest();
        }

        static void findDisjointSetOfSubsetsWithTargetCardinalityTest()
        {

            var set4x1 = findDisjointSetOfSubsetsWithTargetCardinality(new char[] { 'a', 'b', 'c', 'd' }, 1).ToList();
            Debug.Assert(set4x1.Count == 1);
            var set4x1_0 = setOfSetsOfCharsToString(set4x1[0].ToList());
            Debug.Assert(set4x1_0 == "{{a},{b},{c},{d}}");

            var set4x2 = findDisjointSetOfSubsetsWithTargetCardinality(new char[] { 'a', 'b', 'c', 'd' }, 2).ToList();
            Debug.Assert(set4x2.Count == 3);
            var set4x2_0 = setOfSetsOfCharsToString(set4x2[0].ToList());
            Debug.Assert(set4x2_0 == "{{a,b},{c,d}}");
            var set4x2_1 = setOfSetsOfCharsToString(set4x2[1].ToList());
            Debug.Assert(set4x2_1 == "{{a,c},{b,d}}");
            var set4x2_2 = setOfSetsOfCharsToString(set4x2[2].ToList());
            Debug.Assert(set4x2_2 == "{{a,d},{b,c}}");

            var set4x3 = findDisjointSetOfSubsetsWithTargetCardinality(new char[] { 'a', 'b', 'c', 'd' }, 3).ToList();
            Debug.Assert(set4x3.Count == 7);
            var set4x3_0 = setOfSetsOfCharsToString(set4x3[0].ToList());
            Debug.Assert(set4x3_0 == "{{a,b,c},{d}}");
            var set4x3_1 = setOfSetsOfCharsToString(set4x3[1].ToList());
            Debug.Assert(set4x3_1 == "{{a,b,d},{c}}");
            var set4x3_2 = setOfSetsOfCharsToString(set4x3[2].ToList());
            Debug.Assert(set4x3_2 == "{{a,b},{c,d}}");
            var set4x3_3 = setOfSetsOfCharsToString(set4x3[3].ToList());
            Debug.Assert(set4x3_3 == "{{a,c,d},{b}}");
            var set4x3_4 = setOfSetsOfCharsToString(set4x3[4].ToList());
            Debug.Assert(set4x3_4 == "{{a,c},{b,d}}");
            var set4x3_5 = setOfSetsOfCharsToString(set4x3[5].ToList());
            Debug.Assert(set4x3_5 == "{{a,d},{b,c}}");
            var set4x3_6 = setOfSetsOfCharsToString(set4x3[6].ToList());
            Debug.Assert(set4x3_6 == "{{a},{b,c,d}}");

            var set4x4 = findDisjointSetOfSubsetsWithTargetCardinality(new char[] { 'a', 'b', 'c', 'd' }, 4).ToList();
            Debug.Assert(set4x4.Count == 1);
            var set4x4_0 = setOfSetsOfCharsToString(set4x4[0].ToList());
            Debug.Assert(set4x4_0 == "{{a,b,c,d}}");
        }

        /// <summary>
        /// Tests for the mapping method.
        /// </summary>
        static void mappingsTest() {
            List<String> mappings1x1 = mappings("a", new char[] { 'x'}).ToList();

            Debug.Assert(mappings1x1.Contains("x"));
            Debug.Assert(mappings1x1.Count == 1);

            List<String> mappings1x2 = mappings("a", new char[] { 'x', 'y' }).ToList();

            Debug.Assert(mappings1x2.Contains("x"));
            Debug.Assert(mappings1x2.Contains("y"));
            Debug.Assert(mappings1x2.Count == 2);

            List<String> mappings2x1 = mappings("ab", new char[] {'x'}).ToList();

            Debug.Assert(mappings2x1.Contains("xx"));
            Debug.Assert(mappings2x1.Count == 1);

            List<String> mappings2x2 = mappings("ab", new char[] { 'x', 'y' }).ToList();

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
            Debug.Assert(mped("ab", "xx") == 0);
            Debug.Assert(mped("abc", "abc") == 0);
            Debug.Assert(mped("AAABCCDDCAC", "1102322033") == 4);
            Debug.Assert(mped("AAABCCDDCAA", "2210500155") == 5);
            Debug.Assert(mped("AAABCCDCADD", "BABAEFEAFAD") == 5); // todo: check this manually
        }
    }
}