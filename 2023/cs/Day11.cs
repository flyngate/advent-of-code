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

namespace Day11
{
    public class Image
    {
        public readonly List<Point> galaxies = new();

        public void Expand(int rate)
        {
            int maxX = galaxies.Max(galaxy => galaxy.x);
            int maxY = galaxies.Max(galaxy => galaxy.y);
            int amount = rate - 1;

            for (int i = 0; i < maxX; i++)
            {
                bool empty = !galaxies.Any(galaxy => galaxy.x == i);

                if (empty)
                {
                    foreach (var galaxy in galaxies)
                        if (galaxy.x > i)
                            galaxy.x += amount;
                    i += amount;
                    maxX += amount;
                }
            }

            for (int i = 0; i < maxY; i++)
            {
                bool empty = !galaxies.Any(galaxy => galaxy.y == i);

                if (empty)
                {
                    foreach (var galaxy in galaxies)
                        if (galaxy.y > i)
                            galaxy.y += amount;
                    i += amount;
                    maxY += amount;
                }
            }
        }

        public long GetSumOfShortestPaths()
        {
            long result = 0;

            for (int i = 0; i < galaxies.Count; i++)
                for (int j = i + 1; j < galaxies.Count; j++)
                    if (i != j)
                    {
                        var a = galaxies[i];
                        var b = galaxies[j];

                        result += a.ManhattanDistance(b);;
                    }

            return result;
        }
    }

    public class Parser
    {
        public Image Parse(string[] lines, string content)
        {
            Image image = new();

            for (int i = 0; i < lines.Length; i++)
                for (int j = 0; j < lines[0].Length; j++)
                {
                    if (lines[i][j] == '#')
                        image.galaxies.Add(new Point(i, j));
                }

            return image;
        }
    }

    public class PartOne
    {
        public Parser parser = new();

        public long Solve(Image image)
        {
            image.Expand(2);

            return image.GetSumOfShortestPaths();
        }
    }

    public class PartTwo
    {
        public Parser parser = new();

        public long Solve(Image image)
        {
            image.Expand(1_000_000);

            return image.GetSumOfShortestPaths();
        }
    }
}
