using at.mschwaig.mped.definitions;
using at.mschwaig.mped.contribution_sorting;
using at.mschwaig.mped.persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace at.mschwaig.mped.solution_space_visualization
{
    class Program
    {
        static void Main(string[] args)
        {
            int alphabet_size = 8;
            int string_length = 128;

            Problem p = Problem.generateProblem(null, alphabet_size, string_length, 0.0d, 0.1d, 0.1d, LengthCorrectionPolicy.PREPEND_CORRECTION, new RNGCryptoServiceProvider(), 0);

            CharacterMapping[,] one_to_one_mappings = ContributionSorting.generateMatrixOfOneToOneMappings(p);

            var sorter = new ListBasedSorting();
            var result_list = sorter.sortByMinMaxContrib(one_to_one_mappings);
            var sorted_solution_space = result_list.Select(x => x.Item2).SelectMany(x => x).Select(x => Tuple.Create(x, DistanceUtil.mped(p, x))).OrderBy(x => x.Item2);
            var best = sorted_solution_space.First().Item1;

            var path = Path.getPath(best);

            int[,] matrix = new int[string_length, alphabet_size];

            foreach (var t in sorted_solution_space)
            {
                matrix[t.Item2, best.getDistanceTo(t.Item1)] += 1;
            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"results.txt"))
            {
                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    for (int j = 0; j < matrix.GetLength(1); j++)
                    {
                        file.Write(matrix[i, j] + " ");
                    }

                    file.WriteLine();
                }
            }


        }


        private static IEnumerable<Solution> getAdditionalStepRemovedSolutions(Solution best, Solution target)
        {
            int target_distance = target.getDistanceTo(best);

            var neighbours = target.getAllNeighbours();

            var list = neighbours.Where(x => x.getDistanceTo(best) == target_distance + 1);
            return list;
        }

        class Path
        {
            public Solution Solution { get; private set; }

            public IEnumerable<Path> Successors { get; set; }

            public Path(Solution solution, IEnumerable<Path> successors)
            {
                Solution = solution;
                Successors = successors;
            }

            public static IEnumerable<Tuple<int, int>> getMpedChanges(Problem problem, Solution best, Path path)
            {

                foreach (var succ in path.Successors)
                {
                    yield return Tuple.Create(succ.Solution.getDistanceTo(best), DistanceUtil.mped(problem, succ.Solution) - DistanceUtil.mped(problem, path.Solution));
                    var list = getMpedChanges(problem, best, succ);
                    foreach (var elem in list)
                    {
                       yield return elem;
                    }
                }

            }

            public static Path getPath(Solution root)
            {
                return buildPath(root, root);
            }

            private static Path buildPath(Solution root, Solution target)
            {
                return new Path(target, getAdditionalStepRemovedSolutions(root, target).Select(x => buildPath(root, x)));
            }
        }
    }
}
