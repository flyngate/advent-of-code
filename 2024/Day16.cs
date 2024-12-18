namespace AdventOfCode.Day16;

public class Solution
{
    static Vec2 Up = (-1, 0);
    static Vec2 Right = (0, 1);
    static Vec2 Down = (1, 0);
    static Vec2 Left = (0, -1);
    readonly Vec2[] Dirs = [Up, Down, Left, Right];

    public object PartOne(string input)
    {
        var (map, start, end) = Parse(input);
        var distances = GetDistances(map, start, end);

        return Dirs.Select(dir => distances[(end, dir)]).Min();
    }

    public object PartTwo(string input)
    {
        var (map, start, end) = Parse(input);
        var distances = GetDistances(map, start, end);
        var minNode = Dirs.Select(dir => (end, dir)).OrderBy(node => distances[node]).First();

        return GetBestPathTiles(map, distances, minNode);
    }

    Dictionary<(Vec2, Vec2), int> GetDistances(Dictionary<Vec2, char> map, Vec2 start, Vec2 end)
    {
        Dictionary<(Vec2, Vec2), int> distances = [];
        var queue = new PriorityQueue<(Vec2, Vec2), int>();

        distances[(start, Right)] = 0;
        queue.Enqueue((start, Right), 0);

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            var distance = distances[node];

            foreach (var (adjNode, score) in GetAdjacent(map, node.Item1, node.Item2))
            {
                var adjDistance = score + distance;

                if (distances.GetValueOrDefault(adjNode, int.MaxValue) > adjDistance)
                {
                    distances[adjNode] = adjDistance;
                    queue.Enqueue(adjNode, adjDistance);
                }
            }
        }

        return distances;
    }

    int GetBestPathTiles(Dictionary<Vec2, char> map, Dictionary<(Vec2, Vec2), int> distances, (Vec2, Vec2) minNode)
    {
        var bestPathTiles = new HashSet<Vec2>();
        var queue = new Queue<(Vec2, Vec2)>();

        bestPathTiles.Add(minNode.Item1);
        queue.Enqueue(minNode);

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();

            foreach (var (adjNode, score) in GetAdjacent(map, node.Item1, node.Item2, false))
            {
                if (distances[adjNode] == distances[node] - score)
                {
                    queue.Enqueue(adjNode);
                    bestPathTiles.Add(adjNode.Item1);
                }
            }
        }

        return bestPathTiles.Count;
    }

    IEnumerable<((Vec2, Vec2) Node, int Score)> GetAdjacent(Dictionary<Vec2, char> map, Vec2 pos, Vec2 dir, bool forward = true)
    {
        yield return ((pos, RotateLeft(dir)), 1000);
        yield return ((pos, RotateRight(dir)), 1000);

        var next = forward ? pos + dir : pos - dir;

        if (map[next] != '#')
            yield return ((next, dir), 1);
    }

    Vec2 RotateLeft(Vec2 x) => (x.Y, x.X);

    Vec2 RotateRight(Vec2 x) => (-x.Y, -x.X);

    (Dictionary<Vec2, char>, Vec2, Vec2) Parse(string input)
    {
        var lines = input.Split("\n");
        var map = (
            from x in Enumerable.Range(0, lines.Length)
            from y in Enumerable.Range(0, lines[0].Length)
            select new KeyValuePair<Vec2, char>(
                (x, y),
                lines[x][y]
            )
        ).ToDictionary();
        var start = map.First(kv => kv.Value == 'S').Key;
        var end = map.First(kv => kv.Value == 'E').Key;

        return (map, start, end);
    }
}