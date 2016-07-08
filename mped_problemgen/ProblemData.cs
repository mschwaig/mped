using System;
using System.Linq;
using System.Security.Cryptography;
using at.mschwaig.mped.definitions;

namespace at.mschwaig.mped.problemgen
{
    public class ProblemData : Problem
    {
        public string a, b;
        public string s1, s2;

        public int min_related_chars = 0;
        public string permutation;

        private char[] a_converted;
        private char[] b_converted;

        char[] Problem.a
        {
            get
            {
                if (a_converted != null)
                {
                    return a_converted;
                }
                else
                {
                    a_converted = a.ToCharArray().Distinct().OrderBy(x => x).ToArray();
                    return a_converted;
                }
            }
        }

        char[] Problem.b
        {
            get
            {
                if (b_converted != null)
                {
                    return b_converted;
                }
                else
                {
                    b_converted = b.ToCharArray().Distinct().OrderBy(x => x).ToArray();
                    return b_converted;
                }
            }
        }

        string Problem.s1
        {
            get { return s1; }
        }

        string Problem.s2
        {
            get { return s2; }
        }

        public ProblemData(char[] a, char[] b, string s1, string s2)
        {
            this.a = String.Concat(a.OrderBy(x => x));
            this.b = String.Concat(b.OrderBy(x => x));

            if (s1.Distinct().Except(this.a).Count() != 0)
                throw new ArgumentException("given string contains characters which are not part of the given alphabet");

            if (s2.Distinct().Except(this.b).Count() != 0)
                throw new ArgumentException("given string contains characters which are not part of the given alphabet");

            if (this.a.Length > 52 || this.b.Length > 52)
                throw new ArgumentException("only alphabets with up to 52 characters are supported");

            this.s1 = s1;
            this.s2 = s2;
        }

        public static ProblemData generateProblem(int alphabet_size, int string_length, double correlation, RandomNumberGenerator r)
        {
            // TODO: check this
            return generateProblem(alphabet_size, string_length, (int)alphabet_size * correlation, r);
        }

        public static ProblemData generateProblem(int alphabet_size, int string_length, int number_of_identical_characters, RandomNumberGenerator r)
        {
            char[] alphabet = new char[alphabet_size];
            for (int i = 0; i < alphabet_size; i++)
            {
                if (i < 26)
                {
                    alphabet[i] = (char)('A' + i);
                }
                else
                {
                    alphabet[i] = (char)('a' + (i - 26));
                }
            }

            int[] string_p = generate_index_permutation(r, string_length);
            int[] alphabet_p = generate_index_permutation(r, alphabet_size);




            char[] s1 = new char[string_length];
            char[] s2 = new char[string_length];

            for (int permutation_index = 0; permutation_index < string_length; permutation_index++)
            {
                int i = string_p[permutation_index];

                if (permutation_index < number_of_identical_characters)
                {
                    // chars are related by construction
                    int rnd_1 = r.Next<RandomNumberGenerator>(alphabet_size);
                    s1[i] = alphabet[rnd_1];
                    s2[i] = alphabet[alphabet_p[rnd_1]];
                }
                else
                {
                    // chars are not related by construction
                    int rnd_1 = r.Next<RandomNumberGenerator>(alphabet_size);
                    int rnd_2 = r.Next<RandomNumberGenerator>(alphabet_size); // guarantee chars a not the same
                    s1[i] = alphabet[rnd_1];
                    s2[i] = alphabet[rnd_2];
                }
            }

            ProblemData p = new ProblemData(alphabet, alphabet, new String(s1), new String(s2));
            p.min_related_chars = number_of_identical_characters;
            p.permutation = String.Join(",", alphabet_p);
            return p;
        }


        private static int[] generate_index_permutation(RandomNumberGenerator r, int size)
        {
            int[] permutation = new int[size];
            for (int i = 0; i < permutation.Length; i++)
            {
                permutation[i] = i;
            }
            r.Shuffle(permutation);

            return permutation;
        }

    }
}
