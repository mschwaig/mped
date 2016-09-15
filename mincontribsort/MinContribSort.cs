using System;
using at.mschwaig.mped.definitions;
using System.Collections.Generic;

using System.Linq;

namespace at.mschwaig.mped.mincontribsort
{
    public class MinContribSort
    {

        public static CharacterMapping[,] generateMatrixOfOneToOneMappings(Problem p)
        {
            char[] a = p.a, b = p.b;
            string s1 = p.s1, s2 = p.s2;

            CharacterMapping[,] one_to_one_mappings = new CharacterMapping[a.Length, b.Length];

            // generate all possible 1:1 mappings of characters
            for (int i = 0; i < a.Length; i++)
            {
                for (int j = 0; j < b.Length; j++)
                {
                    Dictionary<char, char> dict = new Dictionary<char, char>();
                    dict.Add(a[i], b[j]);
                    CharacterMapping cm = new CharacterMapping(a, b, s1, s2, dict);
                    one_to_one_mappings[i, j] = cm;
                }
            }

            return one_to_one_mappings;
        }
    }
}
