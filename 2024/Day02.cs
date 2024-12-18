using System.Data;

namespace AdventOfCode.Day02;

public class Parser
{
    public int[][] Parse(string[] lines)
    {
        var reports = new List<int[]>();

        foreach (var line in lines)
        {
            var report = line.Split(" ").Select(x => int.Parse(x)).ToArray();

            reports.Add(report);
        }

        return [.. reports];
    }
}

public class PartOne
{
    const int minDiff = 1;
    const int maxDiff = 3;

    public int Solve(int[][] reports)
    {
        return reports.Select(IsSafe).Where(x => x).Count();
    }

    bool IsSafe(int[] report)
    {
        bool increasing = report[0] < report[1];

        for (int i = 1; i < report.Length; i++)
        {
            int diff = Math.Abs(report[i] - report[i - 1]);
            bool isUnsafe = increasing != report[i - 1] < report[i] ||
                diff < minDiff || diff > maxDiff;

            if (isUnsafe)
                return false;
        }

        return true;
    }
}

public class PartTwo
{
    const int minDiff = 1;
    const int maxDiff = 3;

    public int Solve(int[][] reports)
    {
        return reports.Select(IsSafe).Where(x => x).Count();
    }

    bool IsSafe(int[] report)
    {
        return IsSafe(report, true);
    }

    bool IsSafe(int[] report, bool problemDampener)
    {
        bool increasing = report[0] < report[1];

        for (int i = 1; i < report.Length; i++)
        {
            int diff = Math.Abs(report[i] - report[i - 1]);
            bool isUnsafe = increasing != report[i - 1] < report[i] ||
                diff < minDiff || diff > maxDiff;

            if (isUnsafe)
            {
                return problemDampener && IsSafeWithProblemDampener(report, i);
            }
        }

        return true;
    }

    bool IsSafeWithProblemDampener(int[] report, int index)
    {
        return TryModified(report, index) ||
            TryModified(report, index - 1) ||
            (index > 1 && TryModified(report, index - 2));
    }

    bool TryModified(int[] report, int removeAt)
    {
        var modified = report.ToList();

        modified.RemoveAt(removeAt);

        return IsSafe([.. modified], false);
    }
}