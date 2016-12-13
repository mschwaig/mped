using at.mschwaig.mped.definitions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace at.mschwaig.mped.persistence
{
    public class HeuristicRun
    {
        public int HeuristicRunId { get; private set; }

        public AlgorithmType Algorithm { get; private set; }
        public DateTime StartTime { get; private set; }
        public string Parameters { get; private set; }

        public virtual ICollection<Result> Results { get; set; }

        // Used by Entity Framework
        private HeuristicRun() { }

        public HeuristicRun(AlgorithmType algorithm, DateTime startTime, string parameters) {
            this.Algorithm = algorithm;
            this.StartTime = startTime;
            this.Parameters = parameters;
        }
    }
}
