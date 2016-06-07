using System;
using System.Linq;
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Encodings.LinearLinkageEncoding;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.Programmable;
using mped_cs;

namespace HeuristicLab.Problems.Programmable {
  public class CompiledSingleObjectiveProblemDefinition : CompiledProblemDefinition, ISingleObjectiveProblemDefinition {
    public bool Maximization { get { return false; } }

    private Func<AlphabetMapping, int> alphabetMappingEval;
    private Alphabet a_alphabet;
    private Alphabet b_alphabet;

    public override void Initialize() {
      // Use vars.yourVariable to access variables in the variable store i.e. yourVariable
      // Define the solution encoding which can also consist of multiple vectors, examples below

      a_alphabet = new Alphabet(vars.alphabet_a.Value.ToCharArray(), 1);
      b_alphabet = new Alphabet(vars.alphabet_b.Value.ToCharArray(), 1);

      AString a = a_alphabet.create(vars.string_a.Value);
      AString b = b_alphabet.create(vars.string_b.Value);

      int max_alphabet_length = Math.Max(a.getLength(), b.getLength()); // longer one of two alphabet lengths

      Encoding = new PermutationEncoding("permutation", max_alphabet_length, PermutationTypes.Absolute);

      // Add additional initialization code e.g. private variables that you need for evaluating
      alphabetMappingEval = Distance.getAlphabetMappingEvaluationFunction(a, b);

    }

    public double Evaluate(Individual individual, IRandom random) {
      // Use vars.yourVariable to access variables in the variable store i.e. yourVariable
      var permutation = individual.Permutation("permutation").Select(x => Convert.ToByte(x)).ToArray();
      var quality = alphabetMappingEval(AlphabetMapping.getMapping(a_alphabet, b_alphabet, permutation));
      return quality;
    }

    public void Analyze(Individual[] individuals, double[] qualities, ResultCollection results, IRandom random) {
      // Use vars.yourVariable to access variables in the variable store i.e. yourVariable
      // Write or update results given the range of vectors and resulting qualities
      // Uncomment the following lines if you want to retrieve the best individual

      //var orderedIndividuals = individuals.Zip(qualities, (i, q) => new { Individual = i, Quality = q }).OrderBy(z => z.Quality);
      //var best = Maximization ? orderedIndividuals.Last().Individual : orderedIndividuals.First().Individual;

      //if (!results.ContainsKey("Best Solution")) {
      //  results.Add(new Result("Best Solution", typeof(RealVector)));
      //}
      //results["Best Solution"].Value = (IItem)best.RealVector("r").Clone();
    }

    public IEnumerable<Individual> GetNeighbors(Individual individual, IRandom random) {
      // Use vars.yourVariable to access variables in the variable store i.e. yourVariable
      // Create new vectors, based on the given one that represent small changes
      // This method is only called from move-based algorithms (Local Search, Simulated Annealing, etc.)
      while (true) {
        // Algorithm will draw only a finite amount of samples
        // Change to a for-loop to return a concrete amount of neighbors
        var neighbor = individual.Copy();
        var permutation = neighbor.Permutation("permutation");
        int length = permutation.Length;
        int first =  random.Next(length);
        int second = (random.Next(length - 1) + first + 1) % length;
        var tmp = permutation[first];
        permutation[first] = permutation[second];
        permutation[second] = tmp;
        yield return neighbor;
      }
    }

    // Implement further classes and methods
  }
}
