using System;
using System.Linq;
using System.Security.Cryptography;
using at.mschwaig.mped.definitions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace at.mschwaig.mped.definitions
{

    [Table("GENERATED_PROBLEMS")]
    public class Problem
    {
        private char[] a_converted;
        private char[] b_converted;

        [Key]
        public int Id { get; private set; }

        public Experiment Experiement { get; private set; }

        public int GeneratingRunId { get; private set; }

        public string aString { get; private set; }
        public string bString { get; private set; }

        public string s1 { get; private set; }
        public string s2 { get; private set; }

        public double SubstituteProb { get; private set; }
        public double InsertProb { get; private set; }
        public double DeleteProb { get; private set; }
        
        
        public string PermutationString { get; private set; }


        public virtual ICollection<Result> Results { get; set; }
        

        public LengthCorrectionPolicy length_correction_policy { get; private set; }

        public char[] a
        {
            get
            {
                if (a_converted != null)
                {
                    return a_converted;
                }
                else
                {
                    a_converted = aString.ToCharArray().Distinct().OrderBy(x => x).ToArray();
                    return a_converted;
                }
            }
        }

        public char[] b
        {
            get
            {
                if (b_converted != null)
                {
                    return b_converted;
                }
                else
                {
                    b_converted = bString.ToCharArray().Distinct().OrderBy(x => x).ToArray();
                    return b_converted;
                }
            }
        }

        // used by EnitiyFramework
        private Problem() {
        }

        private Problem(Experiment experiment, char[] a, char[] b, string s1, string s2, int[] permutation, double substitute_prob, double insert_prob, double delete_prob, LengthCorrectionPolicy length_correction_policy, int generatring_run_id)
        {
            this.Experiement = experiment;
            this.aString = String.Concat(a.OrderBy(x => x));
            this.bString = String.Concat(b.OrderBy(x => x));

            if (s1.Distinct().Except(this.a).Count() != 0)
                throw new ArgumentException("given string contains characters which are not part of the given alphabet");

            if (s2.Distinct().Except(this.b).Count() != 0)
                throw new ArgumentException("given string contains characters which are not part of the given alphabet");

            if (this.a.Length > 52 || this.b.Length > 52)
                throw new ArgumentException("only alphabets with up to 52 characters are supported");

            this.s1 = s1;
            this.s2 = s2;
            this.PermutationString = String.Join(",", permutation);
            this.SubstituteProb = substitute_prob;
            this.InsertProb = insert_prob;
            this.DeleteProb = delete_prob;
            this.length_correction_policy = length_correction_policy;
            GeneratingRunId = generatring_run_id;
        }

        public static Problem generateProblem(Experiment experiment, int alphabet_size, int string1_length, double substitute_prob, double insert_prob, double delete_prob, LengthCorrectionPolicy length_correction, RandomNumberGenerator r, int runId)
        {
            // checking probability parameters

            if (substitute_prob < 0.0d || insert_prob < 0.0d || delete_prob < 0.0d)
                throw new ArgumentException("probability value must be greater than 0.0d");

            if (substitute_prob + insert_prob + delete_prob > 1.0d)
                throw new ArgumentException("probability values must add up to something smaller than 1.0d");

            if (insert_prob > 0.999d)
                throw new ArgumentException("ridiculously high insertion probabiblities are forbidden to ensure termination");

            // create alphabet

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

            double substitute_prob_upper_limit = substitute_prob;
            double insert_prob_upper_limit = substitute_prob_upper_limit + insert_prob;
            double delete_prob_upper_limit = insert_prob_upper_limit + delete_prob;

            int string2_length = string1_length;
            List<char> operations = new List<char>();
            for (int i = 0, j = 0; i < string1_length && (j < string1_length || length_correction == LengthCorrectionPolicy.NO_CORRECTION); )
            {
                double action = r.NextDouble<RandomNumberGenerator>();
                if (action < substitute_prob_upper_limit)
                {
                    operations.Add('s');
                    i++;
                    j++;
                }
                else if (action < insert_prob_upper_limit)
                {
                    operations.Add('i');
                    string2_length++;
                    j++;
                }
                else if (action < delete_prob_upper_limit)
                {
                    operations.Add('d');
                    string2_length--;
                    i++;
                }
                else
                {
                    operations.Add('n');
                    i++;
                    j++;
                }
            }

            int[] alphabet_p = generate_index_permutation(r, alphabet_size);

            // correct the length of s2 according to the set policy
            switch (length_correction)
            {
                case LengthCorrectionPolicy.PREPEND_CORRECTION:
                    operations.InsertRange(0, generate_correction_elems(string1_length, string2_length));
                    string2_length = string1_length;
                    break;
                case LengthCorrectionPolicy.APPEND_CORRECTION:
                    operations.AddRange(generate_correction_elems(string1_length, string2_length));
                    string2_length = string1_length;
                    break;
                case LengthCorrectionPolicy.DISTRIBUTE_CORRECTION:
                    foreach (var e in generate_correction_elems(string1_length, string2_length))
                    {
                        operations.Insert(r.Next<RandomNumberGenerator>(operations.Count() + 1), e);
                    }
                    string2_length = string1_length;
                    break;
                case LengthCorrectionPolicy.NO_CORRECTION:
                    // nothing to do here
                    break;
                default:
                    throw new ArgumentException("unknown correction policy");
            }

            var match_count = operations.Count(x => x == 'n');
            var mismatch_count = operations.Count(x => x == 's');
            var insert_count = operations.Count(x => x == 'i');
            var delete_count = operations.Count(x => x == 'd');

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
                        rnd =r.Next<RandomNumberGenerator>(alphabet_size);
                        s2[s2_index] = alphabet[rnd];
                        s2_index++;
                        break;
                    case 'd':
                        s1_index++;
                        break;
                }

            }

            return new Problem(experiment, alphabet, alphabet, new String(s1), new String(s2), alphabet_p, substitute_prob, insert_prob, delete_prob, length_correction, runId);
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

        private static List<char> generate_correction_elems(int string1_length, int string2_length)
        {
            int length_diff = string2_length - string1_length;
            int diff_element_count = Math.Abs(length_diff);

            List<char> correction_elems = new List<char>();

            for (int i = 0; i < diff_element_count; i++)
            {
                if (length_diff < 0)
                {
                    correction_elems.Add('i');
                }
                else
                {
                    correction_elems.Add('d');
                }
            }

            return correction_elems;
        }

        public AString s1ToAString() {
            return AString.create(new Alphabet(a, 1), s1);
        }

        public AString s2ToAString()
        {
            return AString.create(new Alphabet(b, 1), s2);
        }
    }
}
