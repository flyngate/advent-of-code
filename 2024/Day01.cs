namespace AdventOfCode.Day01;

public class Solution
{
    public int PartOne(string input)
    {
        int[][] lists = Parse(input);
        int result = 0;

        for (int i = 0; i < lists[0].Length; i++)
            result += Math.Abs(lists[0][i] - lists[1][i]);

        return result;
    }

    public int PartTwo(string input)
    {
        int[][] lists = Parse(input);
        var rightListHist = new int[100000];
        int similarityScore = 0;
        int len = lists[0].Length;

        for (int i = 0; i < len; i++)
            rightListHist[lists[1][i]] += 1;

        foreach (var x in lists[0])
            similarityScore += x * rightListHist[x];

        return similarityScore;
    }

    int[][] Parse(string input)
    {
        var a = new List<int>();
        var b = new List<int>();

        foreach (var line in input.Split("\n"))
        {
            var parts = line.Split("   ");
            var x = int.Parse(parts[0]);
            var y = int.Parse(parts[1]);

            a.Add(x);
            b.Add(y);
        }

        a.Sort();
        b.Sort();

        return [[.. a], [.. b], ];
    }
}