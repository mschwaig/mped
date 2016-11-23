using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using at.mschwaig.mped.definitions;
using at.mschwaig.mped.persistence;

namespace at.mschwaig.mped.contribution_sorting
{
    public class ContributionSortingHeuristic : Heuristic
    {
        SolutionSpaceSortingMethod solution_space_sorting_method = new LinearExactMaxiumumAssignmentBasedSorting();

        public ContributionSortingHeuristic() : base(AlgorithmType.MINCONTRIBSORT_FIRSTGUESS){}

        public override Result applyTo(Problem p, int max_evaluation_number)
        {
            CharacterMapping[,] one_to_one_mappings = ContributionSorting.generateMatrixOfOneToOneMappings(p);

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

            int eval_number = p.a.Length * p.b.Length + eval_counter;
            if (max_evaluation_number != 0 && max_evaluation_number < eval_number)
                throw new ArgumentException("cannot make a guess with so few evaluations");


            return Result.create(p, best, run, p.a.Length * p.b.Length + eval_counter);
        }
    }
}
