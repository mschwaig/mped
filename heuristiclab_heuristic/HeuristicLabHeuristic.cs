using at.mschwaig.mped.persistence;
using HeuristicLab.Optimization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using at.mschwaig.mped.definitions;
using HeuristicLab.Analysis;
using HeuristicLab.SequentialEngine;
using System.Threading;
using HeuristicLab.Problems.MultiParameterizedEditDistance;
using System.Reflection;
using HeuristicLab.Core;

namespace at.mschwaig.mped.heuristiclab.heuristic
{
    public abstract class HeuristicLabHeuristic<T> : Heuristic where T : HeuristicOptimizationEngineAlgorithm
    {
        public HeuristicLabHeuristic(AlgorithmType type) : base(type)
        {
        }

        protected abstract void parameterizeAlgorithm(T alg, int albhabet_size_a, int albhabet_size_b, int max_eval_number);

        protected abstract T instantiateAlgorithm();

        protected abstract void attachEvalCountAnalzyer(T alg, IntValueAnalyzer eval_count_analyzer);

        protected abstract string getEvalCountParameterName();

        public override persistence.Result applyTo(persistence.Problem p)
        {
            var trigger = new ManualResetEvent(false);

            Exception ex = null;
            var alg = instantiateAlgorithm();
            alg.Problem = new MpedBasicProblem(p.s1ToAString(), p.s2ToAString());

            parameterizeAlgorithm(alg, p.a.Length, p.b.Length, getMaxEvalNumber(p.a.Length, p.b.Length));

            alg.Engine = new SequentialEngine();
            alg.Stopped += (sender, args) => { trigger.Set(); };
            alg.ExceptionOccurred += (sender, args) => { ex = args.Value; trigger.Set(); };

            var eval_count_analyzer = EvaluationCountAnalyzerBuilder.createForParameterName(getEvalCountParameterName());
            attachEvalCountAnalzyer(alg, eval_count_analyzer);

            try
            {
                alg.Prepare();
                alg.Start();
                trigger.WaitOne();
                if (ex != null) throw ex;

                persistence.Result r = new persistence.Result(p, run);

                var qualities = ((DataTable)alg.Results["Qualities"].Value).Rows["BestQuality"].Values.ToArray();
                var evaluations = ((DataTable)alg.Results["EvaluationCount Chart"].Value).Rows[getEvalCountParameterName()].Values.ToArray();

                for (int g = 0; g < qualities.Length; g++)
                {
                    r.Solutions.Add(new BestSolution(r, (int)evaluations[g], (int)qualities[g]));
                }

                r.Parameters = ((IParameterizedNamedItem)alg).Parameters.ToArray().ToString();

                return r;
            }
            finally
            {
                trigger.Reset();
            }
        }

        internal static object GetInstanceField(Type type, object instance, string fieldName)
        {
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                | BindingFlags.Static;
            FieldInfo field = type.GetField(fieldName, bindFlags);
            return field.GetValue(instance);
        }
    }
}
