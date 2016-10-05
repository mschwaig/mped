using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using at.mschwaig.mped.definitions;
using at.mschwaig.mped.persistence;

namespace at.mschwaig.mped.mincontribsort
{
    public class MinContribSortBasedGuess : Heuristic
    {
        SolutionSpaceSortingMethod solution_space_sorting_method = new LinearExactMaxiumumAssignmentBasedSorting();

        public MinContribSortBasedGuess() : base(AlgorithmType.MINCONTRIBSORT_FIRSTGUESS){}

        public override Result applyTo(Problem p)
        {
            CharacterMapping[,] one_to_one_mappings = MinContribSort.generateMatrixOfOneToOneMappings(p);

            Solution best = null;
            int min_dist = Int32.MaxValue;
            int eval_counter = 0;

            var sorted_solution_space = solution_space_sorting_method.sortByMinMaxContrib(one_to_one_mappings);
            var max_contrib_section = sorted_solution_space.First();
            foreach (var s in max_contrib_section.Item2.Take(5))
            {
                int dist = DistanceUtil.mped(p, s);
                eval_counter++;
                if (dist < min_dist)
                {
                    min_dist = dist;
                    best = s;
                }
            }


            return Result.create(p, best, run, p.a.Length * p.b.Length + eval_counter);
        }
    }
}
