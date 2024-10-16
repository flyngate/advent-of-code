namespace Day09
{
    using Input = System.Collections.Generic.List<(string, int)>;

    struct Pos
    {
        public int X;
        public int Y;
    }

    class Parser
    {
        public Input Parse(string[] lines)
        {
            return lines.Where(line => line != "").Select(line =>
            {
                var parts = line.Split(" ");
                return (parts[0], Int32.Parse(parts[1]));
            }).ToList();
        }
    }

    class PartOne
    {
        const int size = 10000;
        bool[,] grid = new bool[size, size];
        Pos head = new Pos() { X = size / 2, Y = size / 2 };
        Pos tail = new Pos() { X = size / 2, Y = size / 2 };

        int Norm(int x)
        {
            return x == 0 ? 0 : x / Math.Abs(x);
        }

        void Step(string direction)
        {
            switch (direction)
            {
                case "U":
                    head.X += 1;
                    break;
                case "D":
                    head.X -= 1;
                    break;
                case "R":
                    head.Y += 1;
                    break;
                case "L":
                    head.Y -= 1;
                    break;
            }

            var touching = Math.Abs(head.X - tail.X) <= 1
                && Math.Abs(head.Y - tail.Y) <= 1;

            if (touching)
            {
                return;
            }

            tail.X += Norm(head.X - tail.X);
            tail.Y += Norm(head.Y - tail.Y);
        }

        void PrintGrid()
        {
            for (var i = 0; i < grid.GetLength(0); i++)
            {
                for (var j = 0; j < grid.GetLength(1); j++)
                {
                    Console.Write(grid[i, j] ? '#' : '.');
                }

                Console.WriteLine();
            }
        }

        public int Solve(Input input)
        {
            grid[tail.X, tail.Y] = true;

            foreach (var move in input)
            {
                for (var i = 0; i < move.Item2; i++)
                {
                    Step(move.Item1);
                    grid[tail.X, tail.Y] = true;
                }
            }

            var result = 0;

            for (var i = 0; i < grid.GetLength(0); i++)
            {
                for (var j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[i, j])
                    {
                        result += 1;
                    }
                }
            }

            return result;
        }
    }

    class PartTwo
    {
        const int size = 10000;
        bool[,] grid = new bool[size, size];
        Pos[] knots = Enumerable
            .Repeat(new Pos() { X = size / 2, Y = size / 2 }, 10)
            .ToArray();

        int Norm(int x)
        {
            return x == 0 ? 0 : x / Math.Abs(x);
        }

        void Step(string direction)
        {
            switch (direction)
            {
                case "U":
                    knots[0].X += 1;
                    break;
                case "D":
                    knots[0].X -= 1;
                    break;
                case "R":
                    knots[0].Y += 1;
                    break;
                case "L":
                    knots[0].Y -= 1;
                    break;
            }

            for (int i = 1; i < knots.Length; i++)
            {
                ref Pos head = ref knots[i - 1];
                ref Pos tail = ref knots[i];

                var touching = Math.Abs(head.X - tail.X) <= 1
                    && Math.Abs(head.Y - tail.Y) <= 1;

                if (touching)
                    return;

                tail.X += Norm(head.X - tail.X);
                tail.Y += Norm(head.Y - tail.Y);
            }

        }

        void PrintGrid()
        {
            for (var i = 0; i < grid.GetLength(0); i++)
            {
                for (var j = 0; j < grid.GetLength(1); j++)
                {
                    Console.Write(grid[i, j] ? '#' : '.');
                }

                Console.WriteLine();
            }
        }

        public int Solve(Input input)
        {
            foreach (var move in input)
            {
                for (var i = 0; i < move.Item2; i++)
                {
                    Step(move.Item1);
                    var knot = knots.Last();
                    grid[knot.X, knot.Y] = true;
                }
            }

            var result = 0;

            for (var i = 0; i < grid.GetLength(0); i++)
            {
                for (var j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[i, j])
                    {
                        result += 1;
                    }
                }
            }

            return result;
        }
    }
}
