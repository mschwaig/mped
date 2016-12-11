using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace at.mschwaig.mped.definitions
{
    public enum AlgorithmType
    {
        [Description("C++ Hill Climbing")]
        CPP_HILLCLIMBING,
        [Description("C++ Simulated Annealing")]
        CPP_SIMULATEDANNEALING,
        [Description("Contribution Sorting Guess")]
        CONTRIBUTION_SORTING_GUESS,
        [Description("HL Offspring Selection GA")]
        HL_OSGA,
        [Description("HL Simulated Annealing")]
        HL_SA,
        HL_VNS,
        [Description("HL Local Search")]
        HL_LS,
        [Description("HL Genetic Algorithm")]
        HL_GA,
        HL_SCATTER,
        [Description("HL Offspring Selection ES")]
        HL_OSES,
        [Description("HL ALPS GA")]
        HL_ALPS,
        [Description("HL Evolution Strategy")]
        HL_ES,
        TESTING
    }
}
