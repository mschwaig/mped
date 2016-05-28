using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mped_cs
{
    class Alphabet
    {
        int subset_cardinality;
        char[] characters;
        SortedDictionary<char, byte> reverse_mapping;

        public AString create(string s) {
            return AString.create(this, s);
        }

        public Alphabet(char[] characters, int subset_cardinality)
        {
            char[] sorted = new char[characters.Length];
            Array.Copy(characters, sorted, characters.Length);
            Array.Sort(sorted);
            this.characters = sorted;
            this.subset_cardinality = subset_cardinality;
            this.reverse_mapping = create_reverse_mapping(characters);
        }

        public SortedDictionary<char, byte> getReverseMapping() {
            return reverse_mapping;
        }

        public int getSubsetCardinality() {
            return subset_cardinality;
        }

        public int getCount() {
            return characters.Length;
        }

        public char[] getCharacters()
        {
            return characters;
        }

        public IEnumerable<IEnumerable<IEnumerable<char>>> disjointSetOfSubsets()
        {
            int[] markings = new int[characters.Length];
            // if you want a maximum cardinality instead of a target cardinality, then
            // replace
            // (characters.Length + subset_cardinality - 1) / subset_cardinality + 1
            // with
            // (characters.Length + subset_cardinality) / subset_cardinality + 1
            int[] marker_counts = new int[(characters.Length + subset_cardinality - 1) / subset_cardinality + 1];
            for (int i = 1; i < marker_counts.Length; i++)
            {
                marker_counts[i] = subset_cardinality;
            }
            return mark_subset(markings, marker_counts, 0, 0);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            Alphabet a = obj as Alphabet;

            return Equals(a);
        }

        public bool Equals(Alphabet a)
        {
            if ((object)a == null)
            {
                return false;
            }

            return (subset_cardinality == a.subset_cardinality) && (Enumerable.SequenceEqual(characters, a.characters));
        }

        private IEnumerable<IEnumerable<IEnumerable<char>>> mark_subset(int[] positions, int[] marker_counts, int pos, int highest_used_marker)
        {
            if (pos == positions.Length)
            {
                yield return create_enumerable(positions, highest_used_marker);

            }
            else for (int marker = 1; marker <= highest_used_marker + 1 && marker < marker_counts.Length; marker++)
                {
                    if (marker_counts[marker] > 0)
                    {
                        positions[pos] = marker;
                        marker_counts[marker]--;
                        if (marker == highest_used_marker + 1)
                        {
                            var subset = mark_subset(positions, marker_counts, pos + 1, highest_used_marker + 1);
                            foreach (var v in subset)
                            {
                                yield return v;
                            }
                        }
                        else {
                            var subset = mark_subset(positions, marker_counts, pos + 1, highest_used_marker);
                            foreach (var v in subset)
                            {
                                yield return v;
                            }
                        }
                        marker_counts[marker]++;
                    }
                }
        }
        private IEnumerable<IEnumerable<char>> create_enumerable(int[] positions, int highest_used_marker)
        {
            List<List<char>> ret = new List<List<char>>();
            for (int i = 1; i <= highest_used_marker; i++)
            {
                ret.Add(new List<char>());
            }

            for (int i = 0; i < positions.Length; i++)
            {
                ret[positions[i] - 1].Add(characters[i]);
            }

            return ret;
        }

        private static SortedDictionary<char, byte> create_reverse_mapping(char[] characters)
        {
            SortedDictionary<char, byte> set_reverse_mapping = new SortedDictionary<char, byte>();

            byte i = 0;
            foreach (char c in characters)
            {
                set_reverse_mapping.Add(c, i);
                i++;
            }

            return set_reverse_mapping;
        }
    }
}
