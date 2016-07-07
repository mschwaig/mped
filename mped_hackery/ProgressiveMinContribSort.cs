using System;
using at.mschwaig.mped.definitions;
using System.Collections.Generic;

using at.mschwaig.mped.core;

namespace at.mschwaig.mped.mincontribsort
{
    class ProgressiveMinContribSort : Heuristic
    {
        readonly Mode mode;

        public enum Mode { FIRST_GUESS, PROVE };

        public ProgressiveMinContribSort(Mode mode)
        {
            this.mode = mode;
        }

        public Result applyTo(Problem p)
        {
            CharacterMapping[,] one_to_one_mappings = generateMatrixOfOneToOneMappings(p);

            Solution best = null;
            int min_dist = Int32.MaxValue;
            int eval_counter = 0;

            for (int i = 0; ; i++)
            {

                // TODO: make sure this also works for a.Length != b.Length
                var solutions = solve_linear_exact_maxiumum_assignment_problem(one_to_one_mappings, new int[Math.Max(p.a.Length, p.b.Length)], 0, i, false);
                foreach (var s in solutions)
                {
                    int dist = Distance.mped(p, s);
                    eval_counter++;
                    if (dist < min_dist)
                    {
                        min_dist = dist;
                        best = s;
                    }
                }

                switch (mode)
                {
                    case Mode.FIRST_GUESS:
                        if (best != null)
                        {
                            return Result.create(p, best, p.a.Length * p.b.Length + eval_counter);
                        }
                        break;
                    case Mode.PROVE:
                        if (min_dist < i)
                        {
                            return Result.create(p, best, p.a.Length * p.b.Length + eval_counter);
                        }
                        break;
                }


            }
        }



        private static CharacterMapping[,] generateMatrixOfOneToOneMappings(Problem p)
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

        // specifics to progrissive algorithm
        // variation of the linear bottleneck assigment problem
        private static IEnumerable<Solution> solve_linear_exact_maxiumum_assignment_problem(CharacterMapping[,] mappings, int[] permutation, int i, int exact_maximum, bool contains_max)
        {
            for (int j = 0; j < mappings.GetLength(1); j++)
            {
                permutation[i] = j;

                bool breakflag = false;
                for (int k = 0; k < i; k++)
                {
                    if (permutation[k] == j)
                    {
                        breakflag = true;
                        break;
                    }
                }
                if (breakflag)
                {
                    continue;
                }

                if (mappings[i, j].min_ed <= exact_maximum)
                {
                    if (i == 0)
                    {
                        contains_max = false;
                    }

                    if (mappings[i, j].min_ed == exact_maximum)
                    {
                        contains_max = true;
                    }



                    if (i < mappings.GetLength(0) - 1)
                    {
                        var subproblem_result = solve_linear_exact_maxiumum_assignment_problem(mappings, permutation, i + 1, exact_maximum, contains_max);
                        foreach (var x in subproblem_result)
                        {
                            yield return x;
                        }
                    }
                    else
                    {
                        yield return new Solution(permutation);
                    }
                }
            }
        }
    }
}
