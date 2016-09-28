﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace at.mschwaig.mped.definitions
{

    [Table("SOLUTIONS")]
    public class Solution
    {
        [Key]
        [ForeignKey("Result")]
        public int ResultId { get; set; }

        public virtual Result Result { get; set; }

        public string PermutationString { get; private set; }

        public int[] Permutation
        {
            get
            {
                if (permutation_converted == null)
                {
                    return permutation_converted;
                }
                else
                {
                    permutation_converted = PermutationString.Split(',').Select(x => Int32.Parse(x)).ToArray();
                    return permutation_converted;
                }
            }
        }

        int[] permutation_converted = null;

        // Used by Entity Framework
        private Solution() { }

        public Solution(int [] permutation)
        {
            PermutationString = String.Join(",", permutation);
            this.permutation_converted = permutation;
        }

        public Solution(int[] a_permutation, int[] permutation_b) : this (
            a_permutation
                .Zip(permutation_b, (x, y) => Tuple.Create(x, y))
                .OrderBy(x => x.Item1)
                .Select(x => x.Item2)
                .ToArray()){}



        // we are using the scheme described here
        // http://stackoverflow.com/a/20651773/2066744
        public int getDistanceTo(Solution other)
        {
            if (other.Permutation.Length != Permutation.Length)
                throw new ArgumentException("both solution must have the same permutation length");


            // first invert the other permutation
            int[] other_inv = new int[Permutation.Length];
            for (int i = 0; i < Permutation.Length; i++)
            {
                other_inv[other.Permutation[i]] = i;
            }

            // then compose both permutations
            int[] composite = new int[Permutation.Length];
            for (int i = 0; i < Permutation.Length; i++)
            {
                composite[i] = Permutation[other_inv[i]];
            }

            // compute the number of cycles
            int pos = composite[0];
            int cycle_count = 0;
            composite[0] = -1;
            while (true)
            {
                if (pos == -1)
                {
                    cycle_count++;
                    for (int i = 0; i < Permutation.Length; i++)
                    {
                        if (composite[i] != -1)
                        {
                            pos = composite[i];
                            break;
                        }
                    }
                    if (pos == -1)
                    {
                        break;
                    }
                }
                else
                {
                    int tmp = pos;
                    pos = composite[pos];
                    composite[tmp] = -1;
                }
            }

            // return the length of the permutation minus the number of cycles
            return Permutation.Length - cycle_count;
        }
    }
}
