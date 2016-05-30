using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mped_cs;

namespace mped_cs_tests
{
    class Program
    {
        static void Main(string[] args)
        {
            edTest();

            mpedTest();
        }

        /// <summary>
        /// Tests for the ed method.
        /// </summary>
        static void edTest()
        {
            Debug.Assert(Distance.ed("a", "a") == 0);
            Debug.Assert(Distance.ed("a", "b") == 1);
            Debug.Assert(Distance.ed("a", "") == 1);
            Debug.Assert(Distance.ed("", "a") == 1);
            Debug.Assert(Distance.ed("ab", "b") == 1);
            Debug.Assert(Distance.ed("ab", "a") == 1);
            Debug.Assert(Distance.ed("a", "ab") == 1);
            Debug.Assert(Distance.ed("b", "ab") == 1);
            Debug.Assert(Distance.ed("ab", "ab") == 0);
            Debug.Assert(Distance.ed("aa", "ab") == 1);
        }

        /// <summary>
        /// Tests for MPED method.
        /// </summary>
        static void mpedTest()
        {
            Debug.Assert(Distance.mped("a", "x") == 0);
            Debug.Assert(Distance.mped("ab", "xy") == 0);
            Debug.Assert(Distance.mped("aa", "xy") == 1);
            Debug.Assert(Distance.mped("ab", "xx") == 1);
            Debug.Assert(Distance.mped("abc", "abc") == 0);
            Debug.Assert(Distance.mped("AAABCCDDCAC", "1102322033") == 4);
            Debug.Assert(Distance.mped("AAABCCDDCAA", "2210500155") == 5);
            // Debug.Assert(mped("AAABCCDCADD", "BABAEFEAFAD", true) == 5); // todo: find source? check this manually
            Debug.Assert(Distance.mped(AString.create("AAABCCDDCAA", 2), AString.create("2210500155", 2)) == 3);
            Debug.Assert(Distance.mped(AString.create("ABABD966GDBDA", 2), AString.create("1312X1XX122KK", 1)) == 7);
        }
    }
}
