namespace AdventOfCode.Day20;

public class Solution
{
    static Vec2 Up = (-1, 0);
    static Vec2 Right = (0, 1);
    static Vec2 Down = (1, 0);
    static Vec2 Left = (0, -1);
    readonly Vec2[] Dirs = [Up, Down, Left, Right];
    LinkedList<Vec2> Path = [];
    HashSet<Vec2> PathSet = [];
    Vec2[] MinPath = [];

    public object PartOne(string input) =>
        Solve(input, 2, 2);

    public object PartTwo(string input) =>
        Solve(input, 0, 20);

    int Solve(string input, int minDistance, int maxDistance)
    {
        var (map, start, end) = Parse(input);
        var path = Dfs(map, start, end);
        int minSaved = 100;
        int result = 0;

        for (int i = 0; i < path.Length; i++)
        {
            for (int j = i + minSaved; j < path.Length; j++)
            {
                var diff = path[j] - path[i];
                var distance = Math.Abs(diff.X) + Math.Abs(diff.Y);
                var saved = j - i - distance;

                if (minDistance <= distance && distance <= maxDistance && saved >= minSaved)
                    result++;
            }
        }

        return result;
    }

    Vec2[] Dfs(Dictionary<Vec2, char> map, Vec2 start, Vec2 end)
    {
        Path.AddLast(start);
        PathSet.Add(start);

        RunDfs(map, start, end);

        return MinPath;
    }

    void RunDfs(Dictionary<Vec2, char> map, Vec2 current, Vec2 end)
    {
        if (current == end)
        {
            if (MinPath.Length == 0 || MinPath.Length > Path.Count)
                MinPath = [.. Path];
            
            return;
        }

        var nodes = Dirs.Select(dir => dir + current)
            .Where(next => !PathSet.Contains(next) && map.GetValueOrDefault(next) != '#');

        foreach (var node in nodes)
        {
            Path.AddLast(node);
            PathSet.Add(node);

            RunDfs(map, node, end);

            Path.RemoveLast();
            PathSet.Remove(node);
        }
    }

    (Dictionary<Vec2, char>, Vec2, Vec2) Parse(string input)
    {
        var lines = input.Split("\n");

        var map = (
            from x in Enumerable.Range(0, lines.Length)
            from y in Enumerable.Range(0, lines[0].Length)
            let value = lines[x][y]
            where value != '.'
            select new KeyValuePair<Vec2, char>((x, y), lines[x][y])
        ).ToDictionary();

        var start = map.First(kv => kv.Value == 'S').Key;
        var end = map.First(kv => kv.Value == 'E').Key;

        map.Remove(start);
        map.Remove(end);

        return (map, start, end);
    }
}
