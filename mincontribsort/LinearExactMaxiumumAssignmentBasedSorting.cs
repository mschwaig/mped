using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using at.mschwaig.mped.definitions;

namespace at.mschwaig.mped.mincontribsort
{
    public class LinearExactMaxiumumAssignmentBasedSorting : SolutionSpaceSortingMethod
    {
        public IEnumerable<Tuple<int, IEnumerable<Solution>>> sortByMinMaxContrib(CharacterMapping[,] one_to_one_mappings)
        {
            // find highest value as termination criterion for the search
            int max_min_ed = Int32.MinValue;
            foreach (var x in one_to_one_mappings)
            {
                if (x.min_ed > max_min_ed)
                {
                    max_min_ed = x.min_ed;
                }
            }
            // TODO: make sure this also works for a.Length != b.Length
            // progressively return sorted parts of solution space
            for (int i = 0; i <= max_min_ed; i++)
            {
                var solutions = solve_linear_exact_maxiumum_assignment_problem(one_to_one_mappings, new int[Math.Max(one_to_one_mappings.GetLength(0), one_to_one_mappings.GetLength(1))], 0, i, false);
                if (solutions.Any())
                {
                    yield return Tuple.Create(i, solutions);
                }
            }

        }


        // variation of the linear bottleneck assigment problem
        // see www.opt.math.tu-graz.ac.at/~cela/papers/lap_bericht.pdf
        // or http://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.217.3627&rep=rep1&type=pdf
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
