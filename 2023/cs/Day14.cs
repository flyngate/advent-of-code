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

namespace Day14
{
    public class Platform
    {
        public char[,] Rocks;

        public void TiltNorth()
        {
            for (int i = 0; i < Rocks.GetLength(0); i++)
                for (int j = 0; j < Rocks.GetLength(1); j++)
                    if (Rocks[i, j] == 'O')
                    {
                        for (int k = i; k >= 0; k--)
                        {
                            if (k == 0 || Rocks[k - 1, j] != '.')
                            {
                                Rocks[i, j] = '.';
                                Rocks[k, j] = 'O';
                                break;
                            }
                        }
                    }
        }

        public void TiltWest()
        {
            for (int i = 0; i < Rocks.GetLength(0); i++)
                for (int j = 0; j < Rocks.GetLength(1); j++)
                    if (Rocks[i, j] == 'O')
                    {
                        for (int k = j; k >= 0; k--)
                        {
                            if (k == 0 || Rocks[i, k - 1] != '.')
                            {
                                Rocks[i, j] = '.';
                                Rocks[i, k] = 'O';
                                break;
                            }
                        }
                    }
        }

        public void TiltSouth()
        {
            for (int i = Rocks.GetLength(0) - 1; i >= 0; i--)
                for (int j = Rocks.GetLength(1) - 1; j >= 0; j--)
                    if (Rocks[i, j] == 'O')
                    {
                        for (int k = i; k < Rocks.GetLength(0); k++)
                        {
                            if (k == Rocks.GetLength(0) - 1 || Rocks[k + 1, j] != '.')
                            {
                                Rocks[i, j] = '.';
                                Rocks[k, j] = 'O';
                                break;
                            }
                        }
                    }
        }

        public void TiltEast()
        {
            for (int i = Rocks.GetLength(0) - 1; i >= 0; i--)
                for (int j = Rocks.GetLength(1) - 1; j >= 0; j--)
                    if (Rocks[i, j] == 'O')
                    {
                        for (int k = j; k < Rocks.GetLength(1); k++)
                        {
                            if (k == Rocks.GetLength(1) - 1 || Rocks[i, k + 1] != '.')
                            {
                                Rocks[i, j] = '.';
                                Rocks[i, k] = 'O';
                                break;
                            }
                        }
                    }
        }

        public void TiltCycle()
        {
            TiltNorth();
            TiltWest();
            TiltSouth();
            TiltEast();
        }

        public int GetLoadOnNorthSupportBeam()
        {
            int result = 0;

            for (int i = 0; i < Rocks.GetLength(0); i++)
                for (int j = 0; j < Rocks.GetLength(1); j++)
                    if (Rocks[i, j] == 'O')
                        result += Rocks.GetLength(0) - i;

            return result;
        }

        public override int GetHashCode()
        {
            var result = 1;

            for (int i = 0; i < Rocks.GetLength(0); i++)
                for (int j = 0; j < Rocks.GetLength(1); j++)
                    result = HashCode.Combine(result, Rocks[i, j], i, j);

            return result;
        }
    }
    public class Parser
    {
        public Platform Parse(string[] lines, string content)
        {

            var rocks = new char[lines.Length, lines[0].Length];

            for (int i = 0; i < lines.Length; i++)
                for (int j = 0; j < lines[0].Length; j++)
                    rocks[i, j] = lines[i][j];

            return new Platform() { Rocks = rocks };
        }
    }

    public class PartOne
    {
        public Parser parser = new();

        public int Solve(Platform platform)
        {
            platform.TiltNorth();

            MatrixUtils.Print(platform.Rocks);

            return platform.GetLoadOnNorthSupportBeam();
        }
    }

    public class PartTwo
    {
        public Parser parser = new();

        public int Solve(Platform platform)
        {
            int totalCycles = 1_000_000_000;
            int cycles = 10_000_000;
            var cacheRef = new Dictionary<int, int>();
            var cache = new List<int>();

            for (int i = 0; i < cycles; i++)
            {
                platform.TiltCycle();

                var load = platform.GetLoadOnNorthSupportBeam();
                var key = platform.GetHashCode();

                if (cacheRef.ContainsKey(key))
                {
                    var entryIndex = cacheRef[key];

                    return cache[
                        entryIndex + (totalCycles - entryIndex) % (i - entryIndex) - 1
                    ];
                }

                cache.Add(load);
                cacheRef.Add(key, i);
            }

            return platform.GetLoadOnNorthSupportBeam();
        }
    }
}
