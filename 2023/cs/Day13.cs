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

namespace Day13
{
    public class Pattern
    {
        public string[] lines;

        public string GetRow(int row)
        {
            return lines[row];
        }

        public string GetColumn(int column)
        {
            string result = "";

            for (int i = 0; i < lines.Length; i++)
                result += lines[i][column];

            return result;
        }

    }
    public class Parser
    {
        public Pattern[] Parse(string[] lines, string content)
        {
            var blocks = content.Split("\n\n");

            return blocks.Select(block => new Pattern()
            {
                lines = block.Split("\n"),
            }).ToArray();
        }
    }

    public class Solver
    {
        readonly int Smudges;

        public Solver(int smudges)
        {
            Smudges = smudges;
        }

        static int GetDiff(string a, string b)
        {
            return a.Zip(b).Where((pair) => pair.First != pair.Second).Count();
        }

        int GetHorizontalLineOfReflection(Pattern pattern)
        {
            var len = pattern.lines.Length;

            for (int i = 0; i < len - 1; i++)
            {
                var rows = Math.Min(i + 1, len - i - 1);
                var smudges = 0;

                for (int j = 0; j < rows; j++)
                {
                    var a = pattern.GetRow(i - j);
                    var b = pattern.GetRow(i + j + 1);

                    smudges += GetDiff(a, b);

                    if (smudges > Smudges)
                        break;
                }

                if (smudges == Smudges)
                    return i;
            }

            return -1;
        }

        int GetVerticalLineOfReflection(Pattern pattern)
        {
            var len = pattern.lines[0].Length;

            for (int i = 0; i < len - 1; i++)
            {
                var columns = Math.Min(i + 1, len - i - 1);
                var smudges = 0;

                for (int j = 0; j < columns; j++)
                {
                    var a = pattern.GetColumn(i - j);
                    var b = pattern.GetColumn(i + j + 1);

                    smudges += GetDiff(a, b);

                    if (smudges > Smudges)
                        break;
                }

                if (smudges == Smudges)
                    return i;
            }

            return -1;
        }

        public int GetRank(Pattern pattern)
        {
            var lineIndex = GetVerticalLineOfReflection(pattern);

            if (lineIndex != -1)
                return lineIndex + 1;

            return (GetHorizontalLineOfReflection(pattern) + 1) * 100;
        }
    }

    public class PartOne
    {
        public Parser parser = new();
        public Solver solver;

        public PartOne()
        {
            solver = new Solver(0);
        }

        public int Solve(Pattern[] patterns)
        {
            var GetRank = solver.GetRank;

            return patterns.Select(GetRank).Sum();
        }
    }

    public class PartTwo : PartOne
    {
        public PartTwo()
        {
            solver = new Solver(1);
        }
    }
}
