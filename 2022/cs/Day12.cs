namespace Day12
{
    class Area
    {
        public int[,] Heightmap = new int[0, 0];
        public int Width;
        public int Height;
        public (int, int) Start;
        public (int, int) End;
    }

    class Parser
    {
        public Area Parse(string[] lines)
        {
            var width = lines.Length;
            var height = lines[0].Length;

            var area = new Area()
            {
                Heightmap = new int[width, height],
                Width = width,
                Height = height,
            };

            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    var c = lines[i][j];

                    if (c == 'S')
                    {
                        area.Start = (i, j);
                    }
                    else if (c == 'E')
                    {
                        area.End = (i, j);
                        area.Heightmap[i, j] = 25;
                    }
                    else
                    {
                        area.Heightmap[i, j] = ((int)c) - ((int)'a');
                    }
                }
            }

            return area;
        }
    }

    class PartOne
    {
        (int, int)[] diffs = new (int, int)[] {
            (0, 1),
            (1, 0),
            (0, -1),
            (-1, 0),
        };

        public int Solve(Area area)
        {
            var a = new int[area.Width, area.Height];
            var queue = new Queue<(int, int)>();

            queue.Enqueue(area.Start);

            while (queue.Count > 0)
            {
                var (x, y) = queue.Dequeue();
                var adjacentArr = diffs
                    .Select(diff => (x + diff.Item1, y + diff.Item2));

                foreach (var (p, q) in adjacentArr)
                {
                    if (p < 0 || q < 0 || p >= area.Width || q >= area.Height)
                        continue;
                    
                    if (area.Heightmap[p, q] - area.Heightmap[x, y] > 1)
                        continue;

                    if (a[p, q] != 0)
                        continue;
                    
                    a[p, q] = a[x, y] + 1;

                    queue.Enqueue((p, q));
                }
            }

            return a[area.End.Item1, area.End.Item2];
        }
    }

    class PartTwo
    {
        (int, int)[] diffs = new (int, int)[] {
            (0, 1),
            (1, 0),
            (0, -1),
            (-1, 0),
        };

        public int Solve(Area area)
        {
            var a = new int[area.Width, area.Height];
            var queue = new Queue<(int, int)>();

            queue.Enqueue(area.End);

            while (queue.Count > 0)
            {
                var (x, y) = queue.Dequeue();
                var adjacentArr = diffs
                    .Select(diff => (x + diff.Item1, y + diff.Item2));

                foreach (var (p, q) in adjacentArr)
                {
                    if (p < 0 || q < 0 || p >= area.Width || q >= area.Height)
                        continue;
                    
                    if (area.Heightmap[x, y] - area.Heightmap[p, q] > 1)
                        continue;

                    if (a[p, q] != 0)
                        continue;
                    
                    a[p, q] = a[x, y] + 1;

                    if (area.Heightmap[p, q] == 0)
                        return a[p, q];

                    queue.Enqueue((p, q));
                }
            }

            return -1;
        }
    }
}
