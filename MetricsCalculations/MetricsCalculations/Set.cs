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
        // We assume that 0 can't be an element. Therefore, members[0] defines whether '1' is a part of the set,
        // members[1] defines whether '2' is a part of the set, and so on.
        const int maxNumberInSet = 15;
        private BitArray members;
        private int[] elements;
        public BitArray Members
        {
            get { return members; }
            set { members = value; }
        }
        public int[] Elements
        {
            get { return elements; }
            set { elements = value; }
        }
        public Set()
        {
            members = new BitArray(maxNumberInSet);
            elements = new int[maxNumberInSet];
        }
        public Set(string numbers)
        {
            string[] words = numbers.Split(' ');
            members = new BitArray(maxNumberInSet);
            int setSize = int.Parse(words[0]);
            if (setSize == 0)
            {
                elements = new int[1] { -1 };
            }
            else
            {
                elements = new int[setSize];
            }
            for (int i = 1; i <= setSize; i++)
            {
                int number = int.Parse(words[i]);
                elements[i - 1] = number;
                //if (members[number - 1]) dosth;
                members[number - 1] = true;
            }
        }
    }
}
