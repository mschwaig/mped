using System;
using System.Linq;

using HeuristicLab.Optimization;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Encodings.PermutationEncoding;

namespace mped_cs
{
    [Item("MPED computation", "colorful description")]
    [Creatable(CreatableAttribute.Categories.CombinatorialProblems, Priority = 100)]
    public class MpedBasicProblem : SingleObjectiveBasicProblem<PermutationEncoding>
    {
        #region Parameter Properties
        public IValueParameter<StringValue> AlphabetAParameter
        {
            get { return (IValueParameter<StringValue>)Parameters["AlphabetA"]; }
        }
        public IValueParameter<StringValue> AlphabetBParameter
        {
            get { return (IValueParameter<StringValue>)Parameters["AlphabetB"]; }
        }
        public IValueParameter<StringValue> StringAParameter
        {
            get { return (IValueParameter<StringValue>)Parameters["StringA"]; }
        }
        public IValueParameter<StringValue> StringBParameter
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
      : base() {
            Parameters.Add(new ValueParameter<StringValue>("AlphabetA", "desc", new StringValue("")));
            Parameters.Add(new ValueParameter<StringValue>("AlphabetB", "desc", new StringValue("")));
            Parameters.Add(new ValueParameter<StringValue>("StringA", "desc", new StringValue("")));
            Parameters.Add(new ValueParameter<StringValue>("StringB", "desc", new StringValue("")));

            Initialize();
        }

        private void AfterDeserialization()
        {
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

            Initialize();
        }

        private void Initialize() {
            a_alphabet = new Alphabet(AlphabetA.Value.ToCharArray(), 1);
            b_alphabet = new Alphabet(AlphabetB.Value.ToCharArray(), 1);

            AString a = a_alphabet.create(StringA.Value);
            AString b = b_alphabet.create(StringB.Value);

            int max_alphabet_length = Math.Max(a.getLength(), b.getLength()); // longer one of two alphabet lengths

            Encoding = new PermutationEncoding("permutation", max_alphabet_length, PermutationTypes.Absolute);
            alphabetMappingEval = Distance.getAlphabetMappingEvaluationFunction(a, b);

            AlphabetAParameter.ValueChanged += onParameterChanged;
            AlphabetBParameter.ValueChanged += onParameterChanged;
            StringAParameter.ValueChanged += onParameterChanged;
            StringBParameter.ValueChanged += onParameterChanged;
        }

        public override double Evaluate(Individual individual, IRandom random)
        {                   
            var permutation = individual.Permutation("permutation").Select(x => Convert.ToByte(x)).ToArray();
            var quality = alphabetMappingEval(AlphabetMapping.getMapping(a_alphabet, b_alphabet, permutation));
            return quality;
        }

        public override void Analyze(Individual[] individuals, double[] qualities, ResultCollection results, IRandom random)
        {
        }
    }
}
