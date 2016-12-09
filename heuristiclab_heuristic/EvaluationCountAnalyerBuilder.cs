using HeuristicLab.Analysis;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace at.mschwaig.mped.heuristiclab.heuristic
{
    public class EvaluationCountAnalyerBuilder
    {
        public static SingleValueAnalyzer createForParameterName(String numberOfEvalsParameterName) {
            SingleValueAnalyzer analyzer = new SingleValueAnalyzer();

            analyzer.Name = "EvaluationCountAnalyzer";
            analyzer.ResultsParameter.ActualName = "Results";
            analyzer.ValueParameter.ActualName = "numberOfEvalsParameterName";
            analyzer.ValuesParameter.ActualName = "EvaluationCount Chart";

            // rewire operators to include conversion from IntValue to DoubleValue
            // TODO: do this in a sane and functioning way

            var DataTableValuesCollector = analyzer.OperatorGraph.Operators.OfType<DataTableValuesCollector>().Single();
            var resultsCollector = analyzer.OperatorGraph.Operators.OfType<ResultsCollector>().Single();

            var intValueToDoubleValueConverter = new ExpressionCalculator() { Name = numberOfEvalsParameterName + " 1.0 *" };

            DataTableValuesCollector.Successor = intValueToDoubleValueConverter;

            intValueToDoubleValueConverter.CollectedValues.Add(new LookupParameter<IntValue>(numberOfEvalsParameterName));
            intValueToDoubleValueConverter.ExpressionResultParameter.ActualName = "EvaluationCountDouble";
            intValueToDoubleValueConverter.ExpressionParameter.Value = new StringValue(numberOfEvalsParameterName + " 1.0 *");
            intValueToDoubleValueConverter.Successor = resultsCollector;

            return analyzer;
        }
    }
}
