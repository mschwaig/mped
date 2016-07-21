using System;
using System.Linq;
using System.Security.Cryptography;
using at.mschwaig.mped.definitions;
using System.Collections.Generic;

namespace at.mschwaig.mped.problemgen
{
    public class ProblemData : Problem
    {
        public string a, b;
        public string s1, s2;

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

        public static ProblemData generateProblem(int alphabet_size, int string1_length, double substitute_prob, RandomNumberGenerator r)
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

            // generate random numbers for first string

            int[] s1_rnd = new int[string1_length];
            for (int i = 0; i < string1_length; i++)
            {
                int rnd = r.Next<RandomNumberGenerator>(alphabet_size);
                s1_rnd[i] = rnd;

            }

            // generate first string

            char[] s1 = s1_rnd.Select(x => alphabet[x]).ToArray();

            // generate operation sequence that leads to second string

            int string2_length = string1_length;
            List<char> operations = new List<char>();
            for (int i = 0; i < string1_length; i++)
            {
                double action = substitute_prob;
                if (action < substitute_prob)
                {
                    operations.Add('s');
                }
                else
                {
                    operations.Add('n');
                }
            }

            int[] alphabet_p = generate_index_permutation(r, alphabet_size);

            // TODO: correct s2 length


            // generate s2 from operation sequence

            char[] s2 = new char[string2_length];

            int s1_index = 0;
            int s2_index = 0;

            foreach (var operation in operations)
            {
                switch (operation)
                {
                    case 's':
                        int rnd = r.Next<RandomNumberGenerator>(alphabet_size);
                        // TODO: evaluate preventing accidental matches
                        s2[s2_index] = alphabet[rnd];
                        s1_index++;
                        s2_index++;
                        break;
                    case 'n':
                        s2[s2_index] = alphabet[alphabet_p[s1_rnd[s1_index]]];
                        s1_index++;
                        s2_index++;
                        break;
                    case 'i':
                        rnd = r.Next<RandomNumberGenerator>(alphabet_size - 1);
                        s2[s2_index] = alphabet[rnd];
                        s2_index++;
                        break;
                    case 'd':
                        s1_index++;
                        break;
                }

            }

            ProblemData p = new ProblemData(alphabet, alphabet, new String(s1), new String(s2));
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
