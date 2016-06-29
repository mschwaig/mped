using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mped_hackery
{
    public interface Problem
    {
        char[] a { get; }
        char[] b { get; }

        string s1 { get; }
        string s2 { get; }
    }
}
