using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO.Compression;
using System.IO.Pipes;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Xml.Schema;
using System.Xml.XPath;
using AdventOfCode;
using Day08;

namespace Day10
{
    public class Input
    {
        readonly char[] pipes = new char[] { '|', '-', 'L', 'J', '7', 'F' };

        public char[][] grid;

        public (int, int) GetStartingPoint()
        {
            for (int i = 0; i < grid.Length; i++)
                for (int j = 0; j < grid[0].Length; j++)
                    if (grid[i][j] == 'S')
                        return (i, j);

            return (0, 0);
        }

        public bool IsAvailable((int, int) xy)
        {
            var (x, y) = xy;

            return x >= 0 && y >= 0
                && x < grid.Length && y < grid[0].Length
                && grid[x][y] != '.';
        }

        List<(int, int)> GetAdjacent((int, int) xy)
        {
            var (x, y) = xy;
            var pipe = grid[x][y];
            var adjacent = new List<(int, int)>();

            if (pipe == '|')
            {
                adjacent.Add((x - 1, y));
                adjacent.Add((x + 1, y));
            }

            if (pipe == '-')
            {
                adjacent.Add((x, y - 1));
                adjacent.Add((x, y + 1));
            }

            if (pipe == 'L')
            {
                adjacent.Add((x - 1, y));
                adjacent.Add((x, y + 1));
            }

            if (pipe == 'J')
            {
                adjacent.Add((x - 1, y));
                adjacent.Add((x, y - 1));
            }

            if (pipe == '7')
            {
                adjacent.Add((x + 1, y));
                adjacent.Add((x, y - 1));
            }

            if (pipe == 'F')
            {
                adjacent.Add((x + 1, y));
                adjacent.Add((x, y + 1));
            }

            return adjacent.Where(IsAvailable).ToList();
        }

        List<(int, int)> GetAdjacentAndConnected((int, int) xy)
        {
            return GetAdjacent(xy)
                .Where((_xy) => GetAdjacent(_xy).Contains(xy))
                .ToList();
        }

        (int, int[,], int) FindMaxLoopAt(int sx, int sy)
        {
            var distance = new int[grid.Length, grid[0].Length];
            var queue = new Queue<(int, int)>();

            int Distance((int, int) xy) => distance[xy.Item1, xy.Item2];

            for (int i = 0; i < grid.Length; i++)
                for (int j = 0; j < grid[0].Length; j++)
                    distance[i, j] = -1;

            queue.Enqueue((sx, sy));
            distance[sx, sy] = 0;

            for (int steps = 0; queue.Count > 0; steps++)
            {
                var count = queue.Count;

                for (int i = 0; i < count; i++)
                {
                    var xy = queue.Dequeue();
                    var next = GetAdjacentAndConnected(xy);

                    distance[xy.Item1, xy.Item2] = steps;

                    if (next.Count == 2
                      && Distance(next[0]) > 0
                      && Distance(next[0]) == Distance(next[1]))
                        return (steps, distance, 0);

                    foreach (var _xy in next)
                        if (Distance(_xy) == -1)
                            queue.Enqueue(_xy);
                }
            }

            return (0, new int[0, 0], 0);

        }

        public (int, int[,], int) FindMaxLoop()
        {
            var (sx, sy) = GetStartingPoint();
            (int, int[,], int) maxResult = (0, new int[0, 0], 0);
            var maxLoopLength = 0;
            char maxStartPipe = '0';

            foreach (var pipe in pipes)
            {
                grid[sx][sy] = pipe;

                var result = FindMaxLoopAt(sx, sy);

                if (result.Item1 > maxLoopLength)
                {
                    maxResult = result;
                    maxStartPipe = pipe;
                }
            }

            grid[sx][sy] = maxStartPipe;

            return maxResult;
        }
    }

    public class Parser
    {
        public Input Parse(string[] lines, string content)
        {
            var grid = lines.Select(line => line.ToArray()).ToArray();

            return new Input() { grid = grid };
        }
    }

    public class PartOne
    {
        public Parser parser = new();

        public int Solve(Input input) => input.FindMaxLoop().Item1;
    }

    public class PartTwo
    {
        public Parser parser = new();

        public int Solve(Input input)
        {
            var (_, loop, _) = input.FindMaxLoop();
            var rows = loop.GetLength(0);
            var cols = loop.GetLength(1);
            var result = 0;

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                {
                    if (loop[i, j] >= 0)
                        continue;

                    var amount = 0;
                    char start = '\0';

                    for (int k = i - 1; k >= 0; k--)
                        if (loop[k, j] >= 0)
                        {
                            if (loop[k, j] == -1)
                                continue;

                            var curr = input.grid[k][j];
                            
                            if (curr == 'L' || curr == 'J')
                            {
                                start = curr;
                                continue;
                            }

                            if (curr == '|')
                                continue;
                            
                            if (start == 'J' && curr == '7')
                            {
                                start = '\0';
                                continue;
                            }

                            if (start == 'L' && curr == 'F')
                            {
                                start = '\0';
                                continue;
                            }

                            amount += 1;
                        }

                    if (amount % 2 > 0)
                        result += 1;
                }

            return result;
        }
    }
}
