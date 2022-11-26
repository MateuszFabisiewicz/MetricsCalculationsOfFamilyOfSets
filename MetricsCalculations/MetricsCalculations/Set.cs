using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetricsCalculations
{
    public class Set
    {
        // We assume that 0 can't be an element
        const int maxNumberInSet = 15;
        private BitArray members;
        public BitArray Members
        {
            get { return members; }
            set { members = value; }
        }
        public Set()
        {
            members = new BitArray(maxNumberInSet);
        }
        public Set(string numbers)
        {
            string[] words = numbers.Split(' ');
            members = new BitArray(maxNumberInSet);
            int setSize = int.Parse(words[0]);
            for (int i = 1; i <= setSize; i++)
            {
                int number = int.Parse(words[i]);
                //if (members[number - 1]) dosth;
                members[number - 1] = true;
            }
        }
    }
}
