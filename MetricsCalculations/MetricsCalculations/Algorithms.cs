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
            FamilyOfSets firstFamily = list[0];
            FamilyOfSets secondFamily = list[1];
            int additions = AddEmptySets(firstFamily, secondFamily);
            int difference = HungarianAlgorithm(firstFamily, secondFamily);
            return additions + difference;
        }

        public static float ExactEuclides(List<FamilyOfSets> list)
        {
            Set[] firstFamily = list[0].Family.ToArray();
            Set[] secondFamily = list[1].Family.ToArray();

            // sortujemy elementy zbiorów w rodzinach
            SortSet(firstFamily);
            SortSet(secondFamily);

            // tworzymy ciągi z rodzin 
            List<int> series1 = MakeSeries(firstFamily);
            List<int> series2 = MakeSeries(secondFamily);
            
            // wyrównujemy ciągi
            MakeSeriesSameLength(series1, series2);
            
            return EuklidesDistance(series1,series2);
        }

        public static int HeuristicEuclides(List<FamilyOfSets> list)
        {
            return 4;
        }

        // Adds empty sets to the smaller family so that they have equal amount of sets, counts numbers of additions and returns it
        private static int AddEmptySets(FamilyOfSets familyA, FamilyOfSets familyB)
        {
            int additions = 0;
            while(familyA.Family.Count < familyB.Family.Count)
            {
                familyA.Family.Add(new Set());
                additions++;
            }
            while (familyA.Family.Count > familyB.Family.Count)
            {
                familyB.Family.Add(new Set());
                additions++;
            }
            return additions;
        }

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
            BitArray tempA = new BitArray(arrayA);
            return tempA.Xor(arrayB).Sum();
        }

        private static int HungarianAlgorithm(FamilyOfSets familyA, FamilyOfSets familyB)
        {
            if (familyA.Family.Count != familyB.Family.Count)
                throw new ArgumentException("Nieprawidłowe wymiary rodzin!");
            int[] minimumsOfRows;
            int[] minimumsOfColumns;
            int[,] tableSetDistances = CalculateSetDistancesInTable(familyA, familyB);
            int[] matchA = InitializeArrayWithValue(familyA.Family.Count, -1);
            int[] matchB = InitializeArrayWithValue(familyB.Family.Count, -1);
            bool[] s;
            bool[] t;
            int[] slack = new int[familyA.Family.Count];
            int[] slackX = new int[familyA.Family.Count];
            int[] prev = new int[familyA.Family.Count];
            Queue<int> queue = new Queue<int>();
            int maxMatch = 0;

            GetMinimumsOfRowsAndColumns(tableSetDistances, out minimumsOfRows, out minimumsOfColumns);
            InitialMatching(tableSetDistances, minimumsOfRows, minimumsOfColumns, ref matchA, ref matchB, ref maxMatch);

            while (maxMatch != familyA.Family.Count)
            {
                int root = 0;
                int x = 0;
                int y = 0;



                queue.Clear();

                s = new bool[familyA.Family.Count];
                t = new bool[familyB.Family.Count];

                FindTreeRoot(matchA, queue, ref root, ref prev, ref s);
                InitializeSlack(tableSetDistances, minimumsOfRows, minimumsOfColumns, root, ref slack, ref slackX);

                while (true)
                {
                    while(queue.Count != 0)
                    {
                        x = queue.Dequeue();
                        int tempMinimumOfRow = minimumsOfRows[x];
                        for (y = 0; y < tableSetDistances.GetLength(0); y++)
                        {
                            if (tableSetDistances[x, y] != tempMinimumOfRow + minimumsOfColumns[y] || t[y]) continue;
                            if (matchB[y] == -1) break;
                            t[y] = true;
                            queue.Enqueue(matchB[y]);

                            AddToTree(tableSetDistances, minimumsOfRows, minimumsOfColumns, matchB[y], x, ref s, ref prev, ref slack, ref slackX);
                        }
                        if (y < tableSetDistances.GetLength(1)) break;
                    }
                    if (y < tableSetDistances.GetLength(1)) break;
                    UpdateMinimumsOfRowsAndColumns(s, t, ref minimumsOfRows, ref minimumsOfColumns, ref slack);

                    for(y = 0; y < tableSetDistances.GetLength(1); y++)
                    {
                        if (t[y] || slack[y] != 0) continue;
                        if(matchB[y] == -1)
                        {
                            x = slackX[y];
                            break;
                        }
                        t[y] = true;
                        if (s[matchB[y]]) continue;
                        queue.Enqueue(matchB[y]);
                        AddToTree(tableSetDistances, minimumsOfRows, minimumsOfColumns, matchB[y], slackX[y], ref s, ref prev, ref slack, ref slackX);
                    }
                    if(y < tableSetDistances.GetLength(1)) break;
                }
                maxMatch++;

                int ty;
                for (int i = x, j = y; i != -2; i = prev[i], j = ty)
                {
                    ty = matchA[i];
                    matchB[j] = i;
                    matchA[i] = j;
                }
            }

            int result = 0;
            for (int i = 0; i < matchA.Length; i++)
                result += tableSetDistances[i, matchA[i]];
            return result;
        }

        private static void UpdateMinimumsOfRowsAndColumns(bool[] s, bool[] t, ref int[] minimumsOfRows, ref int[] minimumsOfColumns, ref int[] slack)
        {
            int delta = int.MaxValue;
            for(int i = 0;i< minimumsOfRows.Length; i++)
            {
                if(!t[i])
                    if(delta > slack[i])
                        delta = slack[i];
            }
            for(int i = 0;i< minimumsOfColumns.Length;i++)
            {
                if (s[i])
                    minimumsOfRows[i] += delta;
                if (t[i])
                    minimumsOfColumns[i] -= delta;
                else
                    slack[i] -= delta;
            }
        }

        private static void AddToTree(int[,] tableSetDistances, int[] minimumsOfRows, int[] minimumsOfColumns, int x, int prevx, ref bool[] s, ref int[] prev, ref int[] slack, ref int[] slackX)
        {
            s[x] = true;
            prev[x] = prevx;

            int temMinimumOfRow = minimumsOfRows[x];
            
            for(int y = 0; y < tableSetDistances.GetLength(1); y++)
            {
                if (tableSetDistances[x, y] - temMinimumOfRow - minimumsOfColumns[y] >= slack[y]) continue;
                slack[y] = tableSetDistances[x, y] - temMinimumOfRow - minimumsOfColumns[y];
                slackX[y] = x;
            }
        }

        private static void InitializeSlack(int[,] tableSetDistances, int[] minimumsOfRows, int[] minimumsOfColumns, int root, ref int[] slack, ref int[] slackX)
        {
            for (int i = 0; i < tableSetDistances.GetLength(0); i++)
            {
                slack[i] = tableSetDistances[root, i] - minimumsOfRows[root] - minimumsOfColumns[i];
                slackX[i] = root;
            }
        }

        private static void FindTreeRoot(int[] matchA, Queue<int> queue, ref int root, ref int[] prev, ref bool[] s)
        {
            for(int i = 0; i < matchA.Length; i++)
            {
                if (matchA[i] != -1) continue;
                queue.Enqueue(i);
                root = i;
                prev[i] = -2;
                s[i] = true;
                break;
            }
        }

        private static void InitialMatching(int[,] tableSetDistance, int[] minimumsOfRows, int[] minimumsOfColumns, ref int[] matchA, ref int[] matchB, ref int maxMatch)
        {
            for (int i = 0; i < tableSetDistance.GetLength(0); i++)
            {
                for (int j = 0; j < tableSetDistance.GetLength(1); j++)
                {
                    if (tableSetDistance[i, j] != minimumsOfRows[i] + minimumsOfColumns[j] || matchB[j] != -1) continue;
                    matchA[i] = j;
                    matchB[j] = i;
                    maxMatch++;
                    break;
                }
            }
        }

        private static int[,] CalculateSetDistancesInTable(FamilyOfSets familyA, FamilyOfSets familyB)
        {
            if (familyA.Family.Count != familyB.Family.Count)
                throw new ArgumentException("Nieprawidłowe wymiary rodzin!");

            int[,] table = new int[familyA.Family.Count, familyB.Family.Count];

            for (int row = 0; row < familyA.Family.Count; row++)
            {
                for (int column = 0; column < familyB.Family.Count; column++)
                {
                    table[row, column] = familyA.Family[row].Members.HammingDistance(familyB.Family[column].Members);                }
            }

            return table;
        }

        private static void GetMinimumsOfRowsAndColumns(int[,] tableSetDistance, out int[] minimumsOfRows, out int[] minimumsOfColumns)
        {
            minimumsOfRows = new int[tableSetDistance.GetLength(0)];
            minimumsOfColumns = new int[tableSetDistance.GetLength(1)];
            for(int i = 0; i < tableSetDistance.GetLength(0); i++)
            {
                int minimumOfRow = tableSetDistance[i, 0];
                for (int j = 0; j < tableSetDistance.GetLength(1); j++)
                {
                    if (tableSetDistance[i, j] < minimumOfRow) minimumOfRow = tableSetDistance[i, j];
                    if (minimumOfRow == 0) break;
                }
                minimumsOfRows[i] = minimumOfRow;
            }
            for (int j = 0; j < tableSetDistance.GetLength(1); j++)
            {
                int minimumOfColumn = tableSetDistance[0, j] - minimumsOfRows[0];
                for(int i = 0;i< tableSetDistance.GetLength(0); i++)
                {
                    if (tableSetDistance[i, j] - minimumsOfRows[i] < minimumOfColumn) minimumOfColumn = tableSetDistance[i, j] - minimumsOfRows[i];
                    if (minimumOfColumn == 0) break;
                }
                minimumsOfColumns[j] = minimumOfColumn;
            }
        }

        private static T[] InitializeArrayWithValue<T>(int length, T value)
        {
            T[] result = new T[length];
            for(int i = 0; i < length; i++)
                result[i] = value;
            return result;
        }

        private static void SortSet(Set[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
                for (int j = 0; j < arr[i].Elements.Length; j++)
                    MergeSortArray(arr[i].Elements, 0, arr[i].Elements.Length - 1);
            MergeSortSet(arr, 0, arr.Length - 1);
        }

        private static void MergeSet(Set[] arr, int p, int q, int r)
        {
            int i, j, k;
            int n1 = q - p + 1;
            int n2 = r - q;
            Set[] L = new Set[n1];
            Set[] R = new Set[n2];
            for (i = 0; i < n1; i++)
            {
                L[i] = arr[p + i];
            }
            for (j = 0; j < n2; j++)
            {
                R[j] = arr[q + 1 + j];
            }
            i = 0;
            j = 0;
            k = p;
            while (i < n1 && j < n2)
            {
                if (CompareSets(L[i], R[j]) <= 0)
                {
                    arr[k] = L[i];
                    i++;
                }
                else
                {
                    arr[k] = R[j];
                    j++;
                }
                k++;
            }
            while (i < n1)
            {
                arr[k] = L[i];
                i++;
                k++;
            }
            while (j < n2)
            {
                arr[k] = R[j];
                j++;
                k++;
            }
        }
        private static void MergeSortSet(Set[] arr, int p, int r)
        {
            if (p < r)
            {
                int q = (p + r) / 2;
                MergeSortSet(arr, p, q);
                MergeSortSet(arr, q + 1, r);
                MergeSet(arr, p, q, r);
            }
        }
        private static void MergeArray(int[] arr, int p, int q, int r)
        {
            int i, j, k;
            int n1 = q - p + 1;
            int n2 = r - q;
            int[] L = new int[n1];
            int[] R = new int[n2];
            for (i = 0; i < n1; i++)
            {
                L[i] = arr[p + i];
            }
            for (j = 0; j < n2; j++)
            {
                R[j] = arr[q + 1 + j];
            }
            i = 0;
            j = 0;
            k = p;
            while (i < n1 && j < n2)
            {
                if (L[i] <= R[j])
                {
                    arr[k] = L[i];
                    i++;
                }
                else
                {
                    arr[k] = R[j];
                    j++;
                }
                k++;
            }
            while (i < n1)
            {
                arr[k] = L[i];
                i++;
                k++;
            }
            while (j < n2)
            {
                arr[k] = R[j];
                j++;
                k++;
            }
        }
        private static void MergeSortArray(int[] arr, int p, int r)
        {
            if (p < r)
            {
                int q = (p + r) / 2;
                MergeSortArray(arr, p, q);
                MergeSortArray(arr, q + 1, r);
                MergeArray(arr, p, q, r);
            }
        }
        private static int CompareSets(Set s1, Set s2)
        {
            if (s1.Elements.Length > s2.Elements.Length)
                return 1;
            else if (s1.Elements.Length == s2.Elements.Length)
            {
                for (int i = 0; i < s1.Elements.Length; i++)
                {
                    if (s1.Elements[i] > s2.Elements[i])
                        return 1;
                    else if (s1.Elements[i] < s2.Elements[i])
                        return -1;
                }
                return 0;
            }
            return -1;
        }

        private static List<int> MakeSeries(Set[] family)
        {
            List<int> series = new List<int>();
            for(int i=0; i<family.Length; i++)
            {
                for(int j = 0; j < family[i].Elements.Length; j++)
                {
                    series.Add(family[i].Elements[j]);
                }
                if(i != family.Length-1)
                    series.Add(-2);
            }
            return series;
        }

        private static void MakeSeriesSameLength(List<int> series1, List<int> series2)
        {
            if(series1.Count == series2.Count) 
                return;
            while(series1.Count < series2.Count) 
            {
                series1.Add(0);
            }
            while (series2.Count < series1.Count)
            {
                series2.Add(0);
            }
        }

        private static float EuklidesDistance(List<int> series1,List<int> series2)
        {
            double wynik = 0;
            for(int i=0;i<series1.Count;i++)
            {
                wynik += Math.Pow(series1[i] - series2[i],2);
            }
            return (float)Math.Sqrt(wynik);
        }
    }
}
