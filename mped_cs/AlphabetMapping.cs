using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mped_cs
{
    public class AlphabetMapping
    {
        byte[] permutation;
        Alphabet a;
        Alphabet b;
        Func<char, char, bool> mapping_function;

        private AlphabetMapping(Alphabet a, Alphabet b, byte[] permutation)
        {
            this.a = a;
            this.b = b;
            this.permutation = permutation;
            this.mapping_function = (a_, b_) => (permutation[a.getReverseMapping()[a_]] / a.getSubsetCardinality() == b.getReverseMapping()[b_] / b.getSubsetCardinality());
        }

        public static IEnumerable<AlphabetMapping> getAllPossibleMappings(Alphabet a, Alphabet b)
        {
            byte[] permute_mapping = new byte[Math.Max(a.getCount(), b.getCount())];

            for (byte i = 0; i < permute_mapping.Length; i++)
            {
                permute_mapping[i] = i;
            }

            foreach (var p in array_permutations(permute_mapping))
            {
                yield return new AlphabetMapping(a, b, p);
            }
        }

        public Func<char, char, bool> getMappingFunction()
        {
            return mapping_function;
        }

        public override string ToString()
        {
            char[] a_chars = a.getCharacters();
            char[] b_chars = b.getCharacters();

            StringBuilder builder = new StringBuilder();

            int min = Math.Min(a_chars.Length, b_chars.Length);

            for (int i = 0; i < min; i += a.getSubsetCardinality())
            {
                builder.Append("{");
                for (int j = 0; j < a.getSubsetCardinality(); j++)
                {
                    builder.Append(a_chars[i + j]);
                }
                builder.Append("}->{");
                for (int j = 0; j < b.getSubsetCardinality(); j++)
                {
                    builder.Append(b_chars[permutation[i + j]]);
                }
                builder.Append("} ");
            }

            if (a_chars.Length % a.getSubsetCardinality() != b_chars.Length % b.getSubsetCardinality())
            {
                builder.Append("{");
                for (int i = b_chars.Length; i < a_chars.Length; i++)
                {
                    builder.Append(a_chars[i]);
                }
                builder.Append("}->{");
                for (int i = a_chars.Length; i < b_chars.Length; i++)
                {
                    builder.Append(b_chars[permutation[i]]);
                }
                builder.Append("}");

            }

            return builder.ToString();
        }

        private static IEnumerable<byte[]> array_permutations(byte[] a)
        {
            long length = a.LongLength;
            byte[] a_copy = new byte[a.LongLength];
            Array.Copy(a, a_copy, a.LongLength);

            foreach (byte[] p in permutate_array(length, a_copy))
            {
                byte[] p_copy = new byte[p.LongLength];
                Array.Copy(p, p_copy, p.LongLength);
                yield return p_copy;
            }
        }

        private static IEnumerable<byte[]> permutate_array(long length, byte[] a)
        {
            if (length == 1)
            {
                yield return a;
            }
            else
            {
                for (long i = 0; i < length - 1; i++)
                {
                    var sub1 = permutate_array(length - 1, a);

                    foreach (var s1 in sub1)
                    {
                        yield return a;
                    }

                    long swap_index = (length % 2 == 0 ? i : 0);

                    var temp = a[swap_index];
                    a[swap_index] = a[length - 1];
                    a[length - 1] = temp;
                }

                var sub2 = permutate_array(length - 1, a);

                foreach (var s2 in sub2)
                {
                    yield return a;
                }
            }
        }

    }
}
