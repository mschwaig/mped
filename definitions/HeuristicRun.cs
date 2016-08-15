using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace at.mschwaig.mped.definitions
{
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
        public string gitHash { get; private set; }
        public bool uncommitted_changes { get; private set; }

        public HeuristicRun(AlgorithmType algorithm, DateTime startTime, string parameters, string git_hash, bool uncommitted_changes) {
        }
    }
}
