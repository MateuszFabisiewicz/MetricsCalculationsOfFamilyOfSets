﻿using System;
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

        public static int ExactEuclides(List<FamilyOfSets> list)
        {
            return 3;
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
            return arrayA.Xor(arrayB).Sum();
        }

        private static int HungarianAlgorithm(FamilyOfSets familyA, FamilyOfSets familyB)
        {
            if(familyA.Family.Count != familyB.Family.Count)
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

            while(maxMatch != familyA.Family.Count)
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

            return matchA.Sum();
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
                    table[row, column] = familyA.Family[row].Members.HammingDistance(familyB.Family[column].Members);
                }
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
    }
}
