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
