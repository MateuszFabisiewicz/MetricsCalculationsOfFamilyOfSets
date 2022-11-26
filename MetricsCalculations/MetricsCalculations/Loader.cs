using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetricsCalculations
{
    public static class Loader
    {
        public static List<FamilyOfSets> Load(string path)
        {
            List<FamilyOfSets> list = new List<FamilyOfSets>();
            try
            {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader(path))
                {
                    string? line;
                    for(int i = 0; i < 2; i++)
                    {
                        line = sr.ReadLine();
                        if(line == null) throw new Exception();
                        int number = int.Parse(line);
                        List<Set> sets = new List<Set>();
                        for(int j = 0; j < number; j++)
                        {
                            line = sr.ReadLine();
                            if (line == null) throw new Exception();
                            sets.Add(new Set(line));
                        }
                        list.Add(new FamilyOfSets(sets));
                    }
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            return list;
        }
    }
}
