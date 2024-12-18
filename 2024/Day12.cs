namespace AdventOfCode.Day12;

public class Solution
{
    static Vec2 Up = (-1, 0);
    static Vec2 Right = (0, 1);
    static Vec2 Down = (1, 0);
    static Vec2 Left = (0, -1);
    readonly Vec2[] Directions = [Up, Right, Down, Left];

    public object PartOne(string input)
    {
        var map = Parse(input);
        HashSet<Vec2> visited = [];
        var result = 0;

        foreach (var pos in map.Keys)
            if (!visited.Contains(pos))
            {
                var region = GetRegion(map, pos);
                var area = region.Count;
                var perimeter = GetPerimeter(region);

                result += area * perimeter;

                foreach (var p in region)
                    visited.Add(p);
            }

        return result;
    }

    public object PartTwo(string input)
    {
        var map = Parse(input);
        HashSet<Vec2> visited = [];
        var result = 0;

        foreach (var pos in map.Keys)
            if (!visited.Contains(pos))
            {
                var region = GetRegion(map, pos);
                var area = region.Count;
                var corners = GetCorners(region);

                result += area * corners;

                foreach (var p in region)
                    visited.Add(p);
            }

        return result;
    }

    HashSet<Vec2> GetRegion(Dictionary<Vec2, char> map, Vec2 start)
    {
        var kind = map[start];
        Queue<Vec2> queue = new([start]);
        HashSet<Vec2> region = [start];

        while (queue.Count > 0)
        {
            var pos = queue.Dequeue();

            foreach (var adj in Directions.Select(dir => pos + dir))
            {
                if (region.Contains(adj))
                    continue;

                if (map.GetValueOrDefault(adj, '.') == kind)
                {
                    region.Add(adj);
                    queue.Enqueue(adj);
                }
            }
        }

        return region;
    }

    int GetPerimeter(HashSet<Vec2> region) =>
    (
        from pos in region
        from dir in Directions
        where !region.Contains(pos + dir)
        select dir
    ).Count();

    int GetCorners(HashSet<Vec2> region)
    {
        (Vec2, Vec2)[] types = [(Up, Right), (Up, Left), (Down, Right), (Down, Left)];
        var result = 0;

        foreach (var pos in region)
        {
            foreach (var (u, v) in types)
            {
                if (!region.Contains(pos + u) && !region.Contains(pos + v))
                    result++;

                if (region.Contains(pos + u) && region.Contains(pos + v) && !region.Contains(pos + u + v))
                    result++;
            }
        }
        
        return result;
    }

    Dictionary<Vec2, char> Parse(string input)
    {
        var lines = input.Split("\n");

        return (
            from x in Enumerable.Range(0, lines.Length)
            from y in Enumerable.Range(0, lines[0].Length)
            select new KeyValuePair<Vec2, char>((x, y), lines[x][y])
        ).ToDictionary();
    }
}