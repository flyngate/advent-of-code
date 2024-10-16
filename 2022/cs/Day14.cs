namespace Day14
{
    struct Point
    {
        public int x;
        public int y;
    }

    class Parser
    {
        void DrawLine(char[,] a, Point p, Point q)
        {
            if (p.x > q.x || p.y > q.y)
            {
                var t = p;
                p = q;
                q = t;
            }

            while (p.x != q.x || p.y != q.y)
            {
                a[p.x, p.y] = '#';

                if (p.x != q.x)
                    p.x += 1;

                if (p.y != q.y)
                    p.y += 1;
            }

            a[q.x, q.y] = '#';
        }

        public char[,] Parse(string[] lines)
        {
            char[,] result = new char[1000, 1000];

            for (var i = 0; i < result.GetLength(0); i++)
                for (var j = 0; j < result.GetLength(1); j++)
                    result[i, j] = '.';

            var sequences = new List<List<Point>>();
            var highest = 0;

            foreach (var line in lines)
            {
                var sequence = new List<Point>();

                foreach (var pointStr in line.Split(" -> "))
                {
                    var parts = pointStr.Split(",");
                    var point = new Point()
                    {
                        x = int.Parse(parts[1]),
                        y = int.Parse(parts[0])
                    };

                    if (point.x > highest)
                        highest = point.x;

                    sequence.Add(point);
                }

                sequences.Add(sequence);
            }

            foreach (var sequence in sequences)
            {
                Point? prevPoint = null;

                foreach (var point in sequence)
                {
                    if (prevPoint != null)
                        DrawLine(result, prevPoint.Value, point);

                    prevPoint = point;
                }
            }

            for (int i = 0; i < result.GetLength(1); i++)
                result[highest + 2, i] = '#';

            return result;
        }
    }

    class PartOne
    {
        bool isOver = false;

        void Print(char[,] a, int x0, int y0, int x1, int y1)
        {
            for (var i = x0; i <= x1; i++)
            {
                for (var j = y0; j <= y1; j++)
                    Console.Write($"{a[i, j]} ");
                Console.WriteLine();
            }
        }

        public void Simulate(char[,] a)
        {
            var p = new Point() { x = 0, y = 500 };

            while (true)
            {
                if (p.x >= a.GetLength(0) - 1 || p.y < 0 || p.y >= a.GetLength(1))
                {
                    isOver = true;
                    return;
                }

                if (a[p.x + 1, p.y] == '.')
                {
                    p.x += 1;
                }
                else
                {
                    if (a[p.x + 1, p.y - 1] == '.')
                    {
                        p.x += 1;
                        p.y -= 1;
                    }
                    else if (a[p.x + 1, p.y + 1] == '.')
                    {
                        p.x += 1;
                        p.y += 1;
                    }
                    else 
                        break;
                }
            }

            a[p.x, p.y] = 'o';
        }

        public int Solve(char[,] a)
        {
            var result = 0;

            while (!isOver)
            {
                Simulate(a);
                result += 1;
            }

            Print(a, 0, 494, 9, 503);

            return result - 1;
        }
    }

    class PartTwo
    {
        public int Solve(char[,] a)
        {
            var partOne = new PartOne();
            var result = 0;

            while (a[0, 500] == '.')
            {
                partOne.Simulate(a);
                result += 1;
            }

            // Print(a, 0, 494, 9, 503);

            return result;
        }
    }
}
