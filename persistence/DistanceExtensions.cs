using at.mschwaig.mped.definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace at.mschwaig.mped.persistence
{
    public class DistanceUtil
    {
        public static int mped(Problem p, int[] permutation)
        {
            var a = new Alphabet(p.a, 1);
            var b = new Alphabet(p.b, 1);
            var f = Distance.getAlphabetMappingEvaluationFunction(AString.create(a, p.s1), AString.create(b, p.s2));
            return f(AlphabetMapping.getMapping(a, b, permutation.Select(x => (byte)x).ToArray()));
        }

        public static int mped(Problem p, Solution s)
        {
            return mped(p, s.Permutation);
        }
    }
}
