namespace AdventOfCode.Day18;

public class Solution
{
    static Vec2 Up = (-1, 0);
    static Vec2 Right = (0, 1);
    static Vec2 Down = (1, 0);
    static Vec2 Left = (0, -1);
    readonly Vec2[] Dirs = [Up, Down, Left, Right];
    static readonly int Width = 71;
    static readonly int Height = 71;
    static readonly Vec2 Start = (0, 0);
    static readonly Vec2 End = (Width - 1, Height - 1);

    public object PartOne(string input)
    {
        var corrupted = Parse(input);
        var map = MakeMap(corrupted.Take(1024));

        return Bfs(map, Start, End);
    }

    public object PartTwo(string input)
    {
        var corrupted = Parse(input);
        var amount = 1025;

        for (; amount <= corrupted.Length; amount++)
        {
            var map = MakeMap(corrupted.Take(amount));
            var steps = Bfs(map, Start, End);

            if (steps == -1)
                break;
        }

        var pos = corrupted[amount];

        return $"{pos.X},{pos.Y}";
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

    Dictionary<Vec2, char> MakeMap(IEnumerable<Vec2> corrupted)
    {
        var corruptedSet = corrupted.ToHashSet();

        return (
            from x in Enumerable.Range(0, Width)
            from y in Enumerable.Range(0, Height)
            let value = corruptedSet.Contains((x, y)) ? '#' : '.'
            select new KeyValuePair<Vec2, char>((x, y), value)
        ).ToDictionary();
    }


    Vec2[] Parse(string input) =>
        input.Split("\n")
            .Select(line => line.Split(",").Select(int.Parse).ToArray())
            .Select(nums => new Vec2(nums[0], nums[1]))
            .ToArray();
}