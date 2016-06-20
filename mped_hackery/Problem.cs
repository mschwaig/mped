using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mped_hackery
{
    public class Problem
    {
        public char[] a, b;
        public string s1, s2;

        Nullable<double> correlation;

        public Problem(char[] a, char[] b, string s1, string s2)
        {
            this.a = a.OrderBy(x => x).ToArray();
            this.b = b.OrderBy(x => x).ToArray();

            if (s1.Distinct().Except(this.a).Count() != 0)
                throw new ArgumentException("given string contains characters which are not part of the given alphabet");

            if (s2.Distinct().Except(this.b).Count() != 0)
                throw new ArgumentException("given string contains characters which are not part of the given alphabet");

            this.s1 = s1;
            this.s2 = s2;
        }

        public static Problem generateProblem(int alphabet_size, int length, double generate_correlated_char_probability)
        {
            char[] alphabet = new char[alphabet_size];
            char[] alphabet2 = new char[alphabet_size];
            for (int i = 0; i < alphabet_size; i++)
            {
                if (i < 26)
                {
                    alphabet[i] = (char)('A' + i);
                }
                else
                {
                    alphabet[i] = (char)('A' + (i - 26));
                }
            }

            Array.Copy(alphabet, alphabet2, alphabet_size);
            Random r = new Random();
            r.Shuffle(alphabet2);
            char[] s1 = new char[length];
            char[] s2 = new char[length];
            for (int i = 0; i < s1.Length; i++)
            {
                int r1 = r.Next(alphabet_size);
                s1[i] = alphabet[r1];
                if (r.NextDouble() < generate_correlated_char_probability)
                {
                    s2[i] = alphabet2[r1];
                }
                else
                {
                    s2[i] = alphabet[r.Next(alphabet_size)];
                }
            }

            Problem p = new Problem(alphabet, alphabet, new String(s1), new String(s2));
            p.correlation = generate_correlated_char_probability;
            return p;
        }

    }
}
