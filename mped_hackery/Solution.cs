using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mped_hackery
{
    interface Solution
    {
        Problem Problem { get; }

        int[] Permutation { get; }

        int NumberOfEvalsToObtainSolution { get; }
    }
}
