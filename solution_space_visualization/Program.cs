using at.mschwaig.mped.definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solution_space_visualization
{
    class Program
    {
        static void Main(string[] args)
        {
            Solution s1 = new Solution(new int[] { 0, 1, 2, 3, 4 });
            Solution s2 = new Solution(new int[] { 1, 0, 3, 2, 4 });

            var dist = s1.getDistanceTo(s2);
        }
    }
}
