using System;
using System.Linq;

using HeuristicLab.Optimization;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System.Collections.Generic;

using at.mschwaig.mped.definitions;

namespace at.mschwaig.mped.hl.plugin
{
    [Item("Multi-Parameterized Edit-Distance computation", "colorful description")]
    [Creatable(CreatableAttribute.Categories.CombinatorialProblems, Priority = 100)]
    [StorableClass]
    public class MpedBasicProblem : SingleObjectiveBasicProblem<PermutationEncoding>
    {
        #region Parameter Properties
        private IValueParameter<StringValue> AlphabetAParameter
        {
            get { return (IValueParameter<StringValue>)Parameters["AlphabetA"]; }
        }
        private IValueParameter<StringValue> AlphabetBParameter
        {
            get { return (IValueParameter<StringValue>)Parameters["AlphabetB"]; }
        }
        private IValueParameter<StringValue> StringAParameter
        {
            get { return (IValueParameter<StringValue>)Parameters["StringA"]; }
        }
        private IValueParameter<StringValue> StringBParameter
        {
            get { return (IValueParameter<StringValue>)Parameters["StringB"]; }
        }
        #endregion

        #region Properties
        public StringValue AlphabetA
        {
            get { return AlphabetAParameter.Value; }
            set { AlphabetAParameter.Value = value; }
        }
        public StringValue AlphabetB
        {
            get { return AlphabetBParameter.Value; }
            set { AlphabetBParameter.Value = value; }
        }
        public StringValue StringA
        {
            get { return StringAParameter.Value; }
            set { StringAParameter.Value = value; }
        }
        public StringValue StringB
        {
            get { return StringBParameter.Value; }
            set { StringBParameter.Value = value; }
        }
        #endregion

        private Func<AlphabetMapping, int> alphabetMappingEval;
        private Alphabet a_alphabet;
        private Alphabet b_alphabet;

        [StorableConstructor]
        private MpedBasicProblem(bool deserializing) : base(deserializing) { }

        public override bool Maximization
        {
            get { return false; }
        }

        private MpedBasicProblem(MpedBasicProblem original, Cloner cloner)
      : base(original, cloner) {
        }

        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new MpedBasicProblem(this, cloner);
        }

        public MpedBasicProblem()
      : this(new Alphabet("ABCD".ToCharArray(), 1).create("AAABCCDDCAC"), new Alphabet("0123".ToCharArray(), 1).create("1102322033")) {
        }

        [StorableHook(HookType.AfterDeserialization)]
        private void AfterDeserialization()
        {
            setupEventHandlers();
            Initialize();
        }

        private void onParameterChanged(object sender, EventArgs e)
        {
            Initialize();
        }

        public MpedBasicProblem(AString a, AString b)
      : base()
        {
            Parameters.Add(new ValueParameter<StringValue>("AlphabetA", "desc", new StringValue(a.getAlphabet().ToString())));
            Parameters.Add(new ValueParameter<StringValue>("AlphabetB", "desc", new StringValue(b.getAlphabet().ToString())));
            Parameters.Add(new ValueParameter<StringValue>("StringA", "desc", new StringValue(a.getString())));
            Parameters.Add(new ValueParameter<StringValue>("StringB", "desc", new StringValue(b.getString())));

            setupEventHandlers();

            Initialize();
        }

        private void setupEventHandlers()
        {
            AlphabetA.ValueChanged += onParameterChanged;
            AlphabetB.ValueChanged += onParameterChanged;
            StringA.ValueChanged += onParameterChanged;
            StringB.ValueChanged += onParameterChanged;
        }

        private void Initialize()
        {
            a_alphabet = new Alphabet(AlphabetA.Value.ToCharArray().Distinct().OrderBy(x => x).ToArray(), 1);
            b_alphabet = new Alphabet(AlphabetB.Value.ToCharArray().Distinct().OrderBy(x => x).ToArray(), 1);

            AString a = a_alphabet.create(StringA.Value);
            AString b = b_alphabet.create(StringB.Value);

            int max_alphabet_length = Math.Max(a_alphabet.getCount(), b_alphabet.getCount()); // longer one of two alphabet lengths

            this.Encoding.LengthParameter = new FixedValueParameter<IntValue>("Length", "The length of the permutation.", new IntValue(max_alphabet_length));
            alphabetMappingEval = Distance.getAlphabetMappingEvaluationFunction(a, b);
        }

        public override double Evaluate(Individual individual, IRandom random)
        {
            var permutation = individual.Permutation().Select(x => Convert.ToByte(x)).ToArray();
            var mapping = AlphabetMapping.getMapping(a_alphabet, b_alphabet, permutation);
            var quality = alphabetMappingEval(mapping);
            return quality;
        }

        public override void Analyze(Individual[] individuals, double[] qualities, ResultCollection results, IRandom random)
        {
        }

        public override IEnumerable<Individual> GetNeighbors(Individual individual, IRandom random)
        {
            while (true)
            {
                // Algorithm will draw only a finite amount of samples
                var neighbor = individual.Copy();
                var permutation = neighbor.Permutation();
                int length = permutation.Length;
                int first = random.Next(length);
                int second = (random.Next(length - 1) + first + 1) % length;
                var tmp = permutation[first];
                permutation[first] = permutation[second];
                permutation[second] = tmp;
                yield return neighbor;
            }
        }
    }
}
