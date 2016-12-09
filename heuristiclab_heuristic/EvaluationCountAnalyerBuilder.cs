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
        public static IntValueAnalyzer createForParameterName(String numberOfEvalsParameterName) {
            IntValueAnalyzer intValueAnalyzer = new IntValueAnalyzer();

            intValueAnalyzer.Name = "EvaluationCountAnalyzer";
            intValueAnalyzer.ResultsParameter.ActualName = "Results";
            intValueAnalyzer.ValueParameter.ActualName = numberOfEvalsParameterName;
            intValueAnalyzer.ValuesParameter.ActualName = "EvaluationCount Chart";

            return intValueAnalyzer;
        }
    }
}
