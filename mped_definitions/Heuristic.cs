using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace at.mschwaig.mped.definitions
{
    public interface Heuristic
    {
        Solution applyTo(Problem p);
    }
}
