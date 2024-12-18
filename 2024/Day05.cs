namespace AdventOfCode.Day05;

class RuleComparer : IComparer<int>
{
    int[][] Rules;

    public RuleComparer(int[][] rules)
    {
        Rules = rules;
    }

    public int Compare(int x, int y)
    {
        foreach (var rule in Rules)
        {
            if (rule[0] == x && rule[1] == y)
            {
                return -1;
            }

            if (rule[0] == y && rule[1] == x)
            {
                return 1;
            }
        }

        return 0;
    }
}

public class Update
{
    public required int[] Value;

    public bool IsCorrect(int[][] rules)
    {
        var all = new HashSet<int>(Value);
        var seen = new HashSet<int>();

        foreach (var x in Value)
        {
            foreach (var rule in rules)
            {
                var ruleApplies = all.Contains(rule[0]) && all.Contains(rule[1]) && x == rule[1];

                if (ruleApplies && !seen.Contains(rule[0]))
                {
                    return false;
                }
            }

            seen.Add(x);
        }

        return true;
    }

    public int Middle
    {
        get
        {
            return Value[Value.Length / 2];
        }
    }

    public Update Sort(IComparer<int> comparer)
    {
        var sorted = Value.ToList();

        sorted.Sort(comparer);

        Value = [.. sorted];

        return this;
    }
}

public class Input
{
    public required int[][] Rules;
    public required Update[] Updates;
}

public class Parser
{
    public Input Parse(string[] lines)
    {
        var rules = new List<int[]>();
        var updates = new List<Update>();
        int i = 0;

        for (; lines[i] != ""; i++)
        {
            var rule = lines[i].Split("|").Select(int.Parse).ToArray();

            rules.Add(rule);
        }

        i++;

        for (; i < lines.Length; i++)
        {
            var update = lines[i].Split(",").Select(int.Parse).ToArray();

            updates.Add(new Update { Value = update });
        }

        return new Input
        {
            Rules = [.. rules],
            Updates = [.. updates]
        };
    }
}

public class PartOne
{
    public int Solve(Input input)
    {
        return input
            .Updates
            .Where(update => update.IsCorrect(input.Rules))
            .Select(update => update.Middle)
            .Sum();
    }
}

public class PartTwo
{
    public int Solve(Input input)
    {
        var comparer = new RuleComparer(input.Rules);

        return input
            .Updates
            .Where(update => !update.IsCorrect(input.Rules))
            .Select(update => update.Sort(comparer))
            .Select(update => update.Middle)
            .Sum();
    }
}