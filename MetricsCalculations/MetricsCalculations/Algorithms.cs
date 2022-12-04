using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetricsCalculations
{
    public static class Algorithms
    {
        public static int ExactHamming(List<FamilyOfSets> list)
        {
            /*
            FamilyOfSets firstFamily = list[0];
            FamilyOfSets secondFamily = list[1];
            int additions;
            if (firstFamily.Family.Count < secondFamily.Family.Count)
                additions = AddEmptySets(firstFamily, secondFamily.Family.Count);
            else
                additions = AddEmptySets(secondFamily, firstFamily.Family.Count);
            */
            return 1;
        }

        public static int HeuristicHamming(List<FamilyOfSets> list)
        {
            return 2;
        }

        public static int ExactEuclides(List<FamilyOfSets> list)
        {
            return 3;
        }

        public static int HeuristicEuclides(List<FamilyOfSets> list)
        {
            return 4;
        }

        /*private static int AddEmptySets(FamilyOfSets familyA, FamilyOfSets familyB)
        {
            int additions = 0;
            if(familyA.Family.Count < familyB.Family.Count)
            while(smallerFamily.Family.Count < biggerFamilyCount)
            {
                smallerFamily.Family.Add(new Set());
                additions++;
            }
            return additions;
        }*/

        // BitArray extension method to return number of trues in given array
        private static int Sum(this BitArray array)
        {
            int result = 0;
            foreach (bool value in array)
                result += value ? 1 : 0;
            return result;
        }

        // BitArray extension method to return Hamming distance between two bit arrays
        private static int HammingDistance(this BitArray arrayA, BitArray arrayB)
        {
            return arrayA.Xor(arrayB).Sum();
        }
    }
}
