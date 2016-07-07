using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mped_hackery
{
    interface Heuristic
    {
        Solution applyTo(Problem p);
    }
}
