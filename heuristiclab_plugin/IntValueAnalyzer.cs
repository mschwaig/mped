using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Analysis
{
    /// <summary>
    /// An operator which analyzes a single int value in the scope tree (current scope and parents).
    /// </summary>
    [Item("IntValueAnalyzer", "An operator which analyzes a single int value in the scope tree (current scope and parents).")]
    [StorableClass]
    public sealed class IntValueAnalyzer : AlgorithmOperator, IAnalyzer
    {
        #region Parameter properties
        public ILookupParameter<IntValue> ValueParameter
        {
            get { return (ILookupParameter<IntValue>)Parameters["Value"]; }
        }
        public IValueLookupParameter<DataTable> ValuesParameter
        {
            get { return (IValueLookupParameter<DataTable>)Parameters["Values"]; }
        }
        public IValueLookupParameter<VariableCollection> ResultsParameter
        {
            get { return (IValueLookupParameter<VariableCollection>)Parameters["Results"]; }
        }
        #endregion

        #region Properties
        public bool EnabledByDefault
        {
            get { return true; }
        }

        private DataTableValuesCollector DataTableValuesCollector
        {
            get { return (DataTableValuesCollector)OperatorGraph.InitialOperator; }
        }
        private ExpressionCalculator IntToDoubleConverter
        {
            get { return (ExpressionCalculator)DataTableValuesCollector.Successor; }
        }
        private ResultsCollector ResultsCollector
        {
            get { return (ResultsCollector)IntToDoubleConverter.Successor; }
        }
        #endregion

        #region Storing & Cloning
        [StorableConstructor]
        private IntValueAnalyzer(bool deserializing) : base(deserializing) { }
        private IntValueAnalyzer(IntValueAnalyzer original, Cloner cloner) : base(original, cloner) { }
        public override IDeepCloneable Clone(Cloner cloner)
        {
            return new IntValueAnalyzer(this, cloner);
        }
        #endregion
        public IntValueAnalyzer()
          : base()
        {
            #region Create parameters
            Parameters.Add(new LookupParameter<IntValue>("Value", "The int value contained in the scope tree which should be analyzed."));
            Parameters.Add(new ValueLookupParameter<DataTable>("Values", "The data table to store the values."));
            Parameters.Add(new ValueLookupParameter<VariableCollection>("Results", "The results collection where the analysis values should be stored."));
            #endregion

            #region Create operators
            DataTableValuesCollector dataTableValuesCollector = new DataTableValuesCollector();
            ResultsCollector resultsCollector = new ResultsCollector();

            dataTableValuesCollector.CollectedValues.Add(new LookupParameter<IntValue>("Value", null, ValueParameter.Name));
            dataTableValuesCollector.DataTableParameter.ActualName = ValuesParameter.Name;

            var intValueToDoubleValueConverter = new ExpressionCalculator() { Name = "Value 1.0 *" };

            intValueToDoubleValueConverter.CollectedValues.Add(new LookupParameter<IntValue>("Value", null, ValueParameter.Name));
            intValueToDoubleValueConverter.ExpressionResultParameter.ActualName = "DoubleValue";
            intValueToDoubleValueConverter.ExpressionParameter.Value = new StringValue("Value 1.0 *");

            resultsCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("DoubleValue", null, "DoubleValue"));
            resultsCollector.CollectedValues.Add(new LookupParameter<DataTable>(ValuesParameter.Name));
            resultsCollector.ResultsParameter.ActualName = ResultsParameter.Name;
            #endregion

            #region Create operator graph
            OperatorGraph.InitialOperator = dataTableValuesCollector;
            dataTableValuesCollector.Successor = intValueToDoubleValueConverter;
            intValueToDoubleValueConverter.Successor = resultsCollector;
            resultsCollector.Successor = null;
            #endregion
        }
    }
}
