// See https://aka.ms/new-console-template for more information
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MetricsCalculations
{
    public class Program
    {
        public static void Main()
        {
            bool fileLoaded = false;
            List<FamilyOfSets> list = new List<FamilyOfSets>();
            while(!fileLoaded)
            {
                fileLoaded = LoadFile(out list);
            }
            
            ChooseAlgorithm(in list);
        }
        
        public static void ChooseAlgorithm(in List<FamilyOfSets> list)
        {
            Console.WriteLine("************* Wyznaczanie odległości pomiędzy rodzinami zbiorów *************");
            Console.WriteLine("Wybierz metrykę:");
            Console.WriteLine("1. Metryka korzystająca z odległości Hamminga.");
            Console.WriteLine("2. Metryka korzystająca z odległości Euklidesa.");
            Console.WriteLine("3. Zakończ działanie programu");
            while (true)
            {
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.D1:
                        Console.WriteLine("\n\nOdległość pomiędzy rodzinami: {0}", Algorithms.ExactHamming(in list));
                        break;
                    case ConsoleKey.D2:
                        Console.WriteLine("\n\nOdległość pomiędzy rodzinami: {0}", Algorithms.ExactEuclides(list));
                        break;
                    case ConsoleKey.D3:
                        return;
                    default:
                        Console.WriteLine("\nNieprawidłowy symbol!");
                        break;
                }
                Console.WriteLine();
            }
        }
        public static bool LoadFile(out List<FamilyOfSets> list)
        {
            //BitArray members = new BitArray(10);
            //members[3] = true;
            //PrintValues(members, members.Count);
            //string path = Directory.GetCurrentDirectory();
            list = new List<FamilyOfSets>();
            Console.WriteLine("Podaj ścieżkę do pliku.");
            string zmienna = Console.ReadLine();
            try
            {
                list = Loader.Load(zmienna);
            }
            catch(Exception)
            {
                return false;
            }
            int i = 1;
            foreach (FamilyOfSets family in list)
            {
                Console.WriteLine("Rodzina {0}:", i++);
                foreach (Set set in family.Family)
                {
                    PrintValues(set.Members);
                }
                Console.Write('\n');
            }
            return true;
        }

        public static void PrintValues(BitArray myList)
        {
            int numberOfWritten = 0;
            for (int i = 0; i < myList.Count; i++)
            {
                if (myList[i])
                {
                    Console.Write("{0} ", i+1);
                    numberOfWritten++;
                }
            }
            if(numberOfWritten == 0) Console.Write("Zbiór pusty");
            Console.Write('\n');
        }
        public static void PrintValues(int[] array)
        {
            for(int i=0;i<array.Length;i++)
            {
                Console.WriteLine(array[i]);
            }
            Console.WriteLine("\n");
        }
    }
}
