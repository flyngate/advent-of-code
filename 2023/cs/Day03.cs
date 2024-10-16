using System;
using System.Data;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Xml.XPath;

namespace Day03
{
    public class Parser
    {
        public string[] Parse(string[] lines)
        {
            return lines;
        }
    }

    public class PartTwo
    {
        int[][] offsets = new int[][] {
            new int[] { 1, 1 },
            new int[] { 0, 1 },
            new int[] { 1, 0 },
            new int[] { -1, -1 },
            new int[] { 0, -1 },
            new int[] { -1, 0 },
            new int[] { -1, 1 },
            new int[] { 1, -1 },
        };

        public int Solve(string[] lines)
        {
            var partOne = 0;
            var rows = lines.Length;
            var columns = lines[0].Length;
            int[,] mask = new int[rows, columns];
            var gears = new Dictionary<string, List<int>>();

            bool isAvailable(int x, int y) => x >= 0 && y >= 0 && x < rows && y < columns;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    var isSymbol = !char.IsDigit(lines[i][j]) && lines[i][j] != '.';

                    if (isSymbol)
                    {
                        foreach (var offset in offsets)
                        {
                            var x = i + offset[0];
                            var y = j + offset[1];

                            if (char.IsDigit(lines[x][y]) && isAvailable(x, y))
                            {
                                mask[x, y] = 1;
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (char.IsDigit(lines[i][j]))
                    {
                        var isPartNumber = false;
                        int len = 0;

                        for (; j + len < columns && char.IsDigit(lines[i][j + len]);
                            len++)
                        {
                            if (mask[i, j + len] == 1)
                            {
                                isPartNumber = true;
                            }
                        }

                        if (isPartNumber)
                        {
                            var partNumber = int.Parse(lines[i].Substring(j, len));

                            partOne += partNumber;

                            var seenGears = new HashSet<string>();

                            for (int k = 0; k < len; k++)
                            {
                                foreach (var offset in offsets)
                                {
                                    var x = i + offset[0];
                                    var y = j + k + offset[1];

                                    if (isAvailable(x, y) && lines[x][y] == '*')
                                    {
                                        var key = $"{x}_{y}";

                                        if (!gears.ContainsKey(key))
                                        {
                                            gears[key] = new List<int>();
                                        }

                                        if (!seenGears.Contains(key))
                                        {
                                            gears[key].Add(partNumber);
                                            seenGears.Add(key);
                                        }
                                    }
                                }
                            }
                        }

                        j += len - 1;
                    }
                }
            }

            // Console.WriteLine(partOne);

            int partTwo = 0;

            foreach (var partNumbers in gears.Values)
            {
                if (partNumbers.Count == 2)
                {
                    partTwo += partNumbers[0] * partNumbers[1];
                }
            }

            return partTwo;
        }
    }
}
