using System.Numerics;

namespace AdventOfCode.Day10;

public class Solution
{
    delegate void Callback(Complex last);

    Complex[] Adj = [
        new Complex(0, 1),
        new Complex(1, 0),
        new Complex(0, -1),
        new Complex(-1, 0),
    ];

    public object PartOne(string input)
    {
        var map = Parse(input);
        var r = 0;
        var set = new HashSet<Complex>();

        foreach (var pos in map.Keys)
            if (map[pos] == 0)
            {
                set.Clear();
                Dfs(map, pos, (last) => set.Add(last));
                r += set.Count;
            }

        return r;
    }

    public object PartTwo(string input)
    {
        var map = Parse(input);
        var r = 0;

        foreach (var pos in map.Keys)
            if (map[pos] == 0)
                Dfs(map, pos, (last) => r++);

        return r;
    }

    Dictionary<Complex, int> Parse(string input)
    {
        var lines = input.Split("\n");
        return (
            from x in Enumerable.Range(0, lines.Length)
            from y in Enumerable.Range(0, lines[0].Length)
            select new KeyValuePair<Complex, int>(
                new Complex(x, y), lines[x][y] == '.' ? -1 : int.Parse($"{lines[x][y]}"))
        ).ToDictionary();
    }
    
    void Dfs(Dictionary<Complex, int> map, Complex current, Callback callback)
    {
        if (map[current] == 9)
            callback(current);

        foreach (var next in Adj.Select(diff => current + diff))
            if (map.GetValueOrDefault(next, int.MaxValue) - map[current] == 1)
                Dfs(map, next, callback);
    }
}