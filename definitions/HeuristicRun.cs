using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace at.mschwaig.mped.definitions
{
    [Table("HEURISTIC_RUN")]
    public class HeuristicRun
    {
        public enum AlgorithmType
        {
            CPP_HILLCLIMBING,
            CPP_SIMULATEDANNEALING,
            TESTING
        }

        public int Id { get; private set; }

        public AlgorithmType Algorithm { get; private set; }
        public DateTime StartTime { get; private set; }
        public string Parameters { get; private set; }
        public string GitHash { get; private set; }
        public bool UncommittedChanges { get; private set; }

        public HeuristicRun(AlgorithmType algorithm, DateTime startTime, string parameters, string git_hash, bool uncommitted_changes) {
            this.Algorithm = algorithm;
            this.StartTime = startTime;
            this.Parameters = parameters;
            this.GitHash = git_hash;
            this.UncommittedChanges = uncommitted_changes;
        }
    }
}
