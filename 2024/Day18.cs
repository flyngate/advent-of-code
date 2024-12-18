namespace AdventOfCode.Day18;

public class Solution
{
    static Vec2 Up = (-1, 0);
    static Vec2 Right = (0, 1);
    static Vec2 Down = (1, 0);
    static Vec2 Left = (0, -1);
    readonly Vec2[] Dirs = [Up, Down, Left, Right];

    public object PartOne(string input)
    {
        var w = 71;
        var h = 71;
        var amount = 1024;
        var corrupted = input.Split("\n")
            .Select(line => line.Split(",").Select(int.Parse).ToArray())
            .Select(coord => new Vec2(coord[0], coord[1]))
            .Take(amount)
            .ToHashSet();
        var map = (
            from x in Enumerable.Range(0, w)
            from y in Enumerable.Range(0, h)
            let value = corrupted.Contains((x, y)) ? '#' : '.'
            select new KeyValuePair<Vec2, char>((x, y), value)
        ).ToDictionary();

        // Debug.PrintMatrix(
        //     (x, y) => map.GetValueOrDefault((y, x), '.'),
        //     7,
        //     7
        // );

        Vec2 start = (0, 0);
        Vec2 end = (w - 1, h - 1);

        return Bfs(map, start, end);
    }

    public object PartTwo(string input)
    {
        var w = 71;
        var h = 71;
        Vec2 start = (0, 0);
        Vec2 end = (w - 1, h - 1);
        var corruptedLst = input.Split("\n")
            .Select(line => line.Split(",").Select(int.Parse).ToArray())
            .Select(coord => new Vec2(coord[0], coord[1]))
            .ToList();

        for (int amount = 1024; amount < 1000000; amount++)
        {
            var corrupted = corruptedLst.Take(amount).ToHashSet();
            var map = (
                from x in Enumerable.Range(0, w)
                from y in Enumerable.Range(0, h)
                let value = corrupted.Contains((x, y)) ? '#' : '.'
                select new KeyValuePair<Vec2, char>((x, y), value)
            ).ToDictionary();
            var steps = Bfs(map, start, end);

            if (steps == -1)
            {
                var pos = corruptedLst.Take(amount).Last();

                return $"{pos.X},{pos.Y}";
            }
        }

        throw new Exception("not found");
    }

    int Bfs(Dictionary<Vec2, char> map, Vec2 start, Vec2 end)
    {
        int steps = 0;
        var queue = new Queue<Vec2>();
        var visited = new HashSet<Vec2>();

        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            for (var count = queue.Count; count > 0; count--)
            {
                var pos = queue.Dequeue();

                if (pos == end)
                    return steps;

                foreach (var dir in Dirs)
                {
                    var next = pos + dir;

                    if (!visited.Contains(next) && map.GetValueOrDefault(next, '?') == '.')
                    {
                        queue.Enqueue(next);
                        visited.Add(next);
                    }
                }
            }

            steps++;
        }

        return -1;
    }
}