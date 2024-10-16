using System.Text.RegularExpressions;
using System.Numerics;
using System.Linq;

namespace Day22
{
    struct Vector2
    {
        public int X, Y;

        public Vector2(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class Map
    {
        string[] Data;
        public int Width, Height;

        public Map(string[] data)
        {
            Data = data;
            Height = data.Length;
            Width = data.Select(line => line.Length).Max();
        }

        public char Get(int x, int y)
        {
            if (x >= 0 && x < Data.Length && y >= 0 && y < Data[x].Length)
                return Data[x][y];
            
            return ' ';
        }
    }

    enum Facing { Up = 0, Right, Down, Left }

    struct Bounds { public int start, end; }

    class Parser
    {
        public (Map, string) Parse(string[] lines)
        {
            var map = lines.TakeWhile(line => line != "").ToArray();
            var path = lines.Last();

            return (new Map(map), path);
        }
    }

    public class PartOne
    {
        Map Map;
        string Path;
        Bounds[] RowBounds;
        Bounds[] ColumnBounds;

        readonly Dictionary<Facing, Vector2> DiffByFacing = new Dictionary<Facing, Vector2>()
        {
            { Facing.Up, new Vector2(-1, 0) },
            { Facing.Right, new Vector2(0, 1) },
            { Facing.Down, new Vector2(1, 0) },
            { Facing.Left, new Vector2(0, -1) }
        };

        public void FindBounds()
        {
            var rowBounds = new Bounds[Map.Height];
            var columnBounds = new Bounds[Map.Width];

            for (var row = 0; row < rowBounds.Length; row++)
            {
                int index;

                for (index = 0; Map.Get(row, index) == ' '; index++) ;

                rowBounds[row].start = index;

                for (index = columnBounds.Length - 1; Map.Get(row, index) == ' '; index--) ;

                rowBounds[row].end = index;
            }

            for (var column = 0; column < columnBounds.Length; column++)
            {
                int index;

                for (index = 0; Map.Get(index, column) == ' '; index++) ;

                columnBounds[column].start = index;

                for (index = rowBounds.Length - 1; Map.Get(index, column) == ' '; index--) ;

                columnBounds[column].end = index;
            }

            RowBounds = rowBounds;
            ColumnBounds = columnBounds;
        }

        Facing NextFacing(Facing facing)
        {
            return (Facing)(((int)facing + 1) % 4);
        }

        Facing PrevFacing(Facing facing)
        {
            return (Facing)(((int)facing - 1 + 4) % 4);
        }

        Vector2 Move(Vector2 pos, Facing facing, int steps)
        {
            for (; steps > 0; steps--)
            {
                Vector2 nextPos;

                nextPos.X = pos.X + DiffByFacing[facing].X;
                nextPos.Y = pos.Y + DiffByFacing[facing].Y;

                var outsideOfMap = false;

                try
                {
                    outsideOfMap = Map.Get(nextPos.X, nextPos.Y) == ' ';
                } catch (IndexOutOfRangeException)
                {
                    outsideOfMap = true;
                }

                if (outsideOfMap)
                {
                    if (facing == Facing.Left)
                        nextPos.Y = RowBounds[nextPos.X].end;

                    if (facing == Facing.Right)
                        nextPos.Y = RowBounds[nextPos.X].start;

                    if (facing == Facing.Up)
                        nextPos.X = ColumnBounds[nextPos.Y].end;

                    if (facing == Facing.Down)
                        nextPos.X = ColumnBounds[nextPos.Y].start;
                }

                if (Map.Get(nextPos.X, nextPos.Y) == '#')
                    break;

                pos = nextPos;
            }

            return pos;
        }

        int GetFacingScore(Facing facing)
        {
            return (int) PrevFacing(facing);
        }

        public int Solve((Map, string) input)
        {
            Map = input.Item1;
            Path = input.Item2;

            FindBounds();

            var pos = new Vector2(0, RowBounds[0].start);
            var facing = Facing.Right;

            for (var index = 0; index < Path.Length;)
            {
                var regex = new Regex(@"^(L|R|[0-9]+)");
                var match = regex.Match(Path.Substring(index));

                if (match.Value == "L")
                    facing = PrevFacing(facing);
                else if (match.Value == "R")
                    facing = NextFacing(facing);
                else
                {
                    var steps = int.Parse(match.Value);

                    pos = Move(pos, facing, steps);
                }

                index += match.Length;
            }

            return 1000 * (pos.X + 1) + 4 * (pos.Y + 1) + GetFacingScore(facing);
        }
    }
}
