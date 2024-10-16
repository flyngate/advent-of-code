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

namespace Day22
{
    public record Brick
    {
        public int x1 { get; set; }
        public int x2 { get; set; }
        public int y1 { get; set; }
        public int y2 { get; set; }
        public int z1 { get; set; }
        public int z2 { get; set; }
    }

    public class Parser
    {
        public Brick[] Parse(string[] lines, string content)
        {
            return lines.Select(line =>
            {
                var parts = line.Split(new char[] { ',', '~', });
                var numbers = parts.Select(int.Parse).ToArray();

                return new Brick()
                {
                    x1 = numbers[0],
                    x2 = numbers[1],
                    y1 = numbers[2],
                    y2 = numbers[3],
                    z1 = numbers[4],
                    z2 = numbers[5],
                };
            }).ToArray();
        }
    }

    public class PartOne
    {
        public Parser parser = new();

        public int Solve(Brick[] bricks)
        {
            // Array.Sort(bricks, (a, b) => a.MinZ.CompareTo(b.MinZ));

            return 0;
        }
    }
}
