using System.ComponentModel;

namespace AdventOfCode.Day19;

public class Solution
{
    Dictionary<string, long> Cache = [];

    public object PartOne(string input)
    {
        var (patterns, towels) = Parse(input);

        bool IsPossible(string pattern) =>
            Rec(pattern, towels) > 0;

        return patterns.Count(IsPossible);
    }

    public object PartTwo(string input)
    {
        var (patterns, towels) = Parse(input);

        return patterns.Select(pattern => Rec(pattern, towels)).Sum();
    }

    long Rec(string pattern, string[] towels)
    {
        if (Cache.ContainsKey(pattern))
            return Cache[pattern];

        if (pattern == "")
            return 1;
        
        long result = (
            from towel in towels
            where pattern.StartsWith(towel)
            select Rec(pattern[towel.Length..], towels)
        ).Sum();

        Cache[pattern] = result;

        return result;
    }

    (string[] towels, string[] patterns) Parse(string input)
    {
        var lines = input.Split("\n");
        var towels = lines[0].Split(", ");
        var patterns = lines[2..].ToArray();

        return (patterns, towels);
    }
}
