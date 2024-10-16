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
using System.Runtime.Serialization;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Xml.Schema;
using System.Xml.XPath;
using AdventOfCode;

namespace Day21
{
    public enum Direction
    {
        Right = 0,
        Down = 1,
        Left = 2,
        Up = 3,
    }

    public class Parser
    {
        public char[][] Parse(string[] lines, string content)
        {
            return lines.Select(line => line.ToArray()).ToArray();
        }
    }

    public class PartOne
    {
        public Parser parser = new();

        public int Solve(char[][] map)
        {
            int sx = 0;
            int sy = 0;
            int rows = map.Length;
            int cols = map[0].Length;
            int steps = 64;
            var offsets = new int[][] {
                [ 0, 1 ],
                [ 0, -1 ],
                [ -1, 0 ],
                [ 1, 0 ],
            };
            var queue = new List<(int, int)>();
            var reachable = new HashSet<(int, int)>();
            var seen = new HashSet<(int, int)>();

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    if (map[i][j] == 'S')
                    {
                        sx = i;
                        sy = j;
                    }

            queue.Add((sx, sy));

            for (int i = 0; i < steps; i++)
            {
                var count = queue.Count;

                for (int j = 0; j < count; j++)
                {
                    var (x, y) = queue.First();

                    queue.RemoveAt(0);

                    if ((steps - i) % 2 == 0)
                        reachable.Add((x, y));

                    foreach (var offset in offsets)
                    {
                        var xx = x + offset[0];
                        var yy = y + offset[1];

                        if (!(xx >= 0 && yy >= 0 && xx < rows && yy < cols))
                            continue;

                        if (seen.Contains((xx, yy)) || map[xx][yy] == '#')
                            continue;

                        queue.Add((xx, yy));
                        seen.Add((xx, yy));
                    }
                }
            }

            foreach (var node in queue)
                reachable.Add(node);

            return reachable.Count;
        }
    }

    public class PartTwo
    {
        public Parser parser = new();

        public int Solve(char[][] map)
        {
            int sx = 0;
            int sy = 0;
            int rows = map.Length;
            int cols = map[0].Length;
            int steps = 100;
            var offsets = new int[][] {
                [ 0, 1 ],
                [ 0, -1 ],
                [ -1, 0 ],
                [ 1, 0 ],
            };
            var queue = new Queue<(int, int)>();
            var seen = new HashSet<(int, int)>();
            var reachable = 0;

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    if (map[i][j] == 'S')
                    {
                        sx = i;
                        sy = j;
                    }

            queue.Enqueue((sx, sy));

            for (int i = 0; i < steps + 1; i++)
            {
                var count = queue.Count;

                for (int j = 0; j < count; j++)
                {
                    var (x, y) = queue.Dequeue();

                    if ((steps - i) % 2 == 0)
                        reachable += 1;

                    foreach (var offset in offsets)
                    {
                        var xx = x + offset[0];
                        var yy = y + offset[1];
                        var cell = map[(xx + cols * 1000) % rows]
                            [(yy + cols * 1000) % cols];

                        if (seen.Contains((xx, yy)) || cell == '#')
                            continue;

                        queue.Enqueue((xx, yy));
                        seen.Add((xx, yy));
                    }
                }
            }

            return reachable;
        }
    }
}
