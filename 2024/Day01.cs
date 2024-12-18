namespace AdventOfCode.Day01;

public class Parser
{
    public int[][] Parse(string[] lines)
    {
        var a = new List<int>();
        var b = new List<int>();

        foreach (var line in lines)
        {
            var parts = line.Split("   ");
            var x = int.Parse(parts[0]);
            var y = int.Parse(parts[1]);

            a.Add(x);
            b.Add(y);
        }

        a.Sort();
        b.Sort();

        return new int[][] {
                a.ToArray(),
                b.ToArray(),
            };
    }
}

public class PartOne
{
    public int Solve(int[][] lists)
    {
        int result = 0;

        for (int i = 0; i < lists[0].Length; i++)
            result += Math.Abs(lists[0][i] - lists[1][i]);

        return result;
    }
}

public class PartTwo
{
    public int Solve(int[][] lists)
    {
        var rightListHist = new int[100000];
        int similarityScore = 0;
        int len = lists[0].Length;

        for (int i = 0; i < len; i++)
            rightListHist[lists[1][i]] += 1;

        foreach (var x in lists[0])
            similarityScore += x * rightListHist[x];

        return similarityScore;
    }
}