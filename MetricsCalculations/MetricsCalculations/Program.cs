// See https://aka.ms/new-console-template for more information
using System.Collections;
using System.IO;

namespace MetricsCalculations
{
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("Hello, World!");
            BitArray members = new BitArray(10);
            members[3] = true;
            PrintValues(members, members.Count);
            string path = Directory.GetCurrentDirectory();
            Console.WriteLine("The current directory is {0}", path);
            List<FamilyOfSets> list = Loader.Load("Data.txt");
            foreach(FamilyOfSets family in list)
            {
                foreach(Set set in family.Family)
                {
                    PrintValues(set.Members, set.Members.Count);
                    Console.WriteLine();
                }
            }
        }
        public static void PrintValues(IEnumerable myList, int myWidth)
        {
            int i = myWidth;
            foreach (Object obj in myList)
            {
                if (i <= 0)
                {
                    i = myWidth;
                    Console.WriteLine();
                }
                i--;
                Console.Write("{0,8}", obj);
            }
            Console.WriteLine();
        }
    }
}
