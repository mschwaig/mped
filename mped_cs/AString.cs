using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mped_cs
{
    class AString
    {
        Alphabet alphabet;
        string str;

        private AString(Alphabet a, string s) {
            // TODO: check that string is definde over alphabet
            this.alphabet = a;
            this.str = s;
        }

        public static AString create(string s) {
            char[] alphabet = s.Distinct().OrderBy(x => x).ToArray();
            Alphabet a = new Alphabet(alphabet, 1);
            return new AString(a, s);
        }

        public static AString create(Alphabet a, string s)
        {
            char[] alphabet = s.Distinct().OrderBy(x => x).ToArray();
            return new AString(a, s);
        }

        public Alphabet getAlphabet() {
            return alphabet;
        }

        public string getString() {
            return str;
        }
    }
}
