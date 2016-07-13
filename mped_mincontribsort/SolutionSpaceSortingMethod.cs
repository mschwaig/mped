using at.mschwaig.mped.definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace at.mschwaig.mped.mincontribsort
{
    interface SolutionSpaceSortingMethod
    {
        IEnumerable<Tuple<int, IEnumerable<Solution>>> sortByMinMaxContrib(CharacterMapping[,] one_to_one_mappings);
    }
}
