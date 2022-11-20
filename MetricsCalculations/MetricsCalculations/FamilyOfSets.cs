using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetricsCalculations
{
    public class FamilyOfSets
    {
        private List<List<int>> family;
        
        public List<List<int>> Family
        {
            get { return family; }
            set { family = value; }
        }

        public FamilyOfSets() { 
            family = new List<List<int>>();
        }

        public FamilyOfSets(string path)
        {
            family = new List<List<int>>();
            Load(path);
        }

        private void Load(string path)
        {
            
        }
    }
}
