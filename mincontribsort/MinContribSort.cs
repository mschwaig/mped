using System;
using at.mschwaig.mped.definitions;
using System.Collections.Generic;

using System.Linq;

namespace at.mschwaig.mped.mincontribsort
{
    public class MinContribSort : Heuristic
    {
        Mode mode;
        SolutionSpaceSortingMethod solution_space_sorting_method;

        public enum Mode { FIRST_GUESS, PROVE };

        public MinContribSort(Mode mode, SolutionSpaceSortingMethod solution_space_sorting_method)
        {
            this.mode = mode;
            this.solution_space_sorting_method = solution_space_sorting_method;
        }

        public Result applyTo(Problem p)
        {
            CharacterMapping[,] one_to_one_mappings = generateMatrixOfOneToOneMappings(p);

            Solution best = null;
            int min_dist = Int32.MaxValue;
            int eval_counter = 0;

            var sorted_solution_space = solution_space_sorting_method.sortByMinMaxContrib(one_to_one_mappings);
            foreach (var max_contrib_section in sorted_solution_space)
            {
                foreach (var s in max_contrib_section.Item2)
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
                        if (min_dist <= max_contrib_section.Item1)
                        {
                            return Result.create(p, best, p.a.Length * p.b.Length + eval_counter);
                        }
                        break;
                }
            }

            throw new InvalidOperationException("this exception should never be hit, since it means we exhausted the whole search space without finding a minimal value in it");
        }

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
