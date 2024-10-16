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
using System.Text;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Xml.Schema;
using System.Xml.XPath;
using AdventOfCode;

namespace Day16
{
    public class Grid
    {
        public char[][] grid;

        public int Trace(Beam initial)
        {
            var activeBeams = new List<Beam>() { initial };
            var beams = new HashSet<Beam>();
            var energized = new HashSet<(int, int)>();

            while (activeBeams.Count > 0)
            {
                var beam = activeBeams.First();
                activeBeams.RemoveAt(0);

                if (beams.Contains(beam))
                    continue;

                var nextBeams = beam.Next(this);

                activeBeams.AddRange(nextBeams);
                energized.Add((beam.X, beam.Y));
                beams.Add(beam);
            }

            return energized.Count;
        }
    }

    public enum Direction
    {
        Right = 0,
        Down = 1,
        Left = 2,
        Up = 3,
    }

    public class Beam
    {
        public int X;
        public int Y;
        public Direction D;

        public Beam(int x, int y, Direction d)
        {
            X = x;
            Y = y;
            D = d;
        }

        readonly Dictionary<Direction, int[]> offsets = new Dictionary<Direction, int[]> {
            { Direction.Right, [0, 1] },
            { Direction.Down, [1, 0] },
            { Direction.Left, [0, -1] },
            { Direction.Up, [-1, 0] }
        };

        public List<Beam> Next(Grid grid)
        {
            var result = new List<Beam>();

            bool IsValid(Beam beam) =>
                beam.X >= 0 && beam.Y >= 0 && beam.X < grid.grid.Length
                && beam.Y < grid.grid[0].Length;

            Beam NextBeam(Direction direction)
            {
                var offset = offsets[direction];

                return new Beam(X + offset[0], Y + offset[1], direction);
            }

            var cell = grid.grid[X][Y];

            if (cell == '.')
            {
                result.Add(NextBeam(D));
            }

            if (cell == '\\')
            {
                Direction direction;

                if (D == Direction.Right)
                    direction = Direction.Down;
                else if (D == Direction.Left)
                    direction = Direction.Up;
                else if (D == Direction.Up)
                    direction = Direction.Left;
                else
                    direction = Direction.Right;
                
                result.Add(NextBeam(direction));
            }

            if (cell == '/')
            {
                Direction direction;

                if (D == Direction.Right)
                    direction = Direction.Up;
                else if (D == Direction.Left)
                    direction = Direction.Down;
                else if (D == Direction.Up)
                    direction = Direction.Right;
                else
                    direction = Direction.Left;
                
                result.Add(NextBeam(direction));
            }

            if (cell == '|')
            {
                if (D == Direction.Right || D == Direction.Left)
                {
                    result.Add(NextBeam(Direction.Up));
                    result.Add(NextBeam(Direction.Down));
                } else
                {
                    result.Add(NextBeam(D));
                }
            }

            if (cell == '-')
            {
                if (D == Direction.Up || D == Direction.Down)
                {
                    result.Add(NextBeam(Direction.Left));
                    result.Add(NextBeam(Direction.Right));
                } else
                {
                    result.Add(NextBeam(D));
                }
            }

            return result.Where(IsValid).ToList();
        }

        public override bool Equals(object? obj)
        {
            var other = obj as Beam;

            if (obj == null)
                return false;
            
            return other.X == X && other.Y == Y && other.D == D;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, D);
        }
    }

    public class Parser
    {
        public Grid Parse(string[] lines, string content)
        {
            return new Grid()
            {
                grid = lines.Select(line => line.ToArray()).ToArray()
            };
        }
    }

    public class PartOne
    {
        public Parser parser = new();

        public int Solve(Grid grid)
        {
            return grid.Trace(new Beam(0, 0, Direction.Right));
        }
    }

    public class PartTwo
    {
        public Parser parser = new();

        public int Solve(Grid grid)
        {
            var rows = grid.grid.Length;
            var cols = grid.grid[0].Length;
            var beams = new List<Beam>();

            for (int i = 0; i < rows; i++)
            {
                beams.Add(new Beam(i, 0, Direction.Right));
                beams.Add(new Beam(i, 0, Direction.Down));
                beams.Add(new Beam(i, 0, Direction.Left));
                beams.Add(new Beam(i, 0, Direction.Up));
                beams.Add(new Beam(i, cols - 1, Direction.Right));
                beams.Add(new Beam(i, cols - 1, Direction.Down));
                beams.Add(new Beam(i, cols - 1, Direction.Left));
                beams.Add(new Beam(i, cols - 1, Direction.Up));
            }

            for (int i = 0; i < cols; i++)
            {
                beams.Add(new Beam(0, i, Direction.Right));
                beams.Add(new Beam(0, i, Direction.Down));
                beams.Add(new Beam(0, i, Direction.Left));
                beams.Add(new Beam(0, i, Direction.Up));
                beams.Add(new Beam(rows - 1, i, Direction.Right));
                beams.Add(new Beam(rows - 1, i, Direction.Down));
                beams.Add(new Beam(rows - 1, i, Direction.Left));
                beams.Add(new Beam(rows - 1, i, Direction.Up));
            }

            return beams.Select(beam => grid.Trace(beam)).Max();
        }
    }
}
