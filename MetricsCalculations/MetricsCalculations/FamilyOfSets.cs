using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetricsCalculations
{
    public class FamilyOfSets
    {
        private List<Set> family;
        public List<Set> Family
        {
            get { return family; }
            set { family = value; }
        }
        public FamilyOfSets() { 
            family = new List<Set>();
        }     
        public FamilyOfSets(List<Set> sets)
        {
            family = sets;
        }
    }
}
