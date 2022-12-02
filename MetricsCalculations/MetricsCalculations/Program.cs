// See https://aka.ms/new-console-template for more information
using System.Collections;
using System.IO;

namespace MetricsCalculations
{
    public class Program
    {
        public static void Main()
        {
            ChooseAlgorithm();

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

        public static void ChooseAlgorithm()
        {
            Console.WriteLine("************* Wyznaczanie odległości pomiędzy rodzinami zbiorów *************");
            Console.WriteLine("Wybierz metrykę:");
            Console.WriteLine("1. Metryka korzystająca z odległości Hamminga.");
            Console.WriteLine("2. Metryka korzystająca z odległości Euklidesa.");
            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.D1:
                    Console.WriteLine("\n1. Algorytm dokladny.");
                    Console.WriteLine("2. Heurystyka.");
                    switch (Console.ReadKey().Key)
                    {
                        case ConsoleKey.D1:
                            // algorytm 1 dokładny
                            break;
                        case ConsoleKey.D2:
                            // algorytm 1 heurystyka
                            break;
                        default:
                            Console.WriteLine("\nNieprawidłowy symbol!");
                            break;
                    }
                    break;
                case ConsoleKey.D2:
                    Console.WriteLine("\n1. Algorytm dokladny.");
                    Console.WriteLine("2. Heurystyka.");
                    switch (Console.ReadKey().Key)
                    {
                        case ConsoleKey.D1:
                            // algorytm 2 dokładny
                            break;
                        case ConsoleKey.D2:
                            // algorytm 2 heurystyka
                            break;
                        default:
                            Console.WriteLine("\nNieprawidłowy symbol!");
                            break;
                    }
                    break;
                default:
                    Console.WriteLine("\nNieprawidłowy symbol!");
                    break;
            }
            Console.WriteLine();
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
