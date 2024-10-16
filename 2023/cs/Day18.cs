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
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Xml.Schema;
using System.Xml.XPath;
using AdventOfCode;

namespace Day18
{
    public enum Direction
    {
        Right = 0,
        Down = 1,
        Left = 2,
        Up = 3,
    }

    public record Instruction(Direction D, long L);

    public class ParserPartOne
    {
        public Instruction[] Parse(string[] lines, string content)
        {
            Direction GetDirection(char d)
            {
                switch (d)
                {
                    case 'L': return Direction.Left;
                    case 'U': return Direction.Up;
                    case 'R': return Direction.Right;
                    default: return Direction.Down;
                }
            }

            return lines.Select(line =>
            {
                var parts = line.Split(" ");
                var direction = GetDirection(parts[0][0]);
                var length = int.Parse(parts[1]);

                return new Instruction(direction, length);
            }).ToArray();
        }
    }

    class Area
    {
        public static long Calculate(Instruction[] instructions)
        {
            Dictionary<Direction, int[]> offsets = new Dictionary<Direction, int[]> {
                { Direction.Right, [ 0, 1 ] },
                { Direction.Left, [ 0, -1 ] },
                { Direction.Up, [ -1, 0 ] },
                { Direction.Down, [ 1, 0 ] },
            };

            long size = instructions.Sum(i => i.L) * 2;
            long x = size / 2;
            long y = size / 2;
            long s = 1;

            foreach (var instr in instructions)
            {
                if (instr.D == Direction.Right)
                    s -= instr.L * x;
                
                if (instr.D == Direction.Down)
                    s += instr.L;

                if (instr.D == Direction.Left)
                    s += instr.L * (x + 1);

                var offset = offsets[instr.D];

                x += offset[0] * instr.L;
                y += offset[1] * instr.L;
            }

            return s;
        }
    }

    public class PartOne
    {
        public ParserPartOne parser = new();

        public long Solve(Instruction[] instructions)
        {
            return Area.Calculate(instructions);
        }
    }

    public class ParserPartTwo
    {
        public Instruction[] Parse(string[] lines, string content)
        {
            Direction GetDirection(char d)
            {
                switch (d)
                {
                    case '0': return Direction.Right;
                    case '1': return Direction.Down;
                    case '2': return Direction.Left;
                    default: return Direction.Up;
                }
            }

            return lines.Select(line =>
            {
                var parts = line.Split(" ");
                var color = parts[2].Substring(2, parts[2].Length - 3);
                var direction = GetDirection(color.Last());
                var length = (int) Convert.ToUInt32(color.Substring(0, 5), 16);

                return new Instruction(direction, length);
            }).ToArray();
        }
    }

    public class PartTwo
    {
        public ParserPartTwo parser = new();

        public long Solve(Instruction[] instructions)
        {
            return Area.Calculate(instructions);
        }
    }
}
