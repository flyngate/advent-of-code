namespace AdventOfCode.Day11;

public class Solution
{
    public object PartOne(string input) => Solve(input, 25);

    public object PartTwo(string input) => Solve(input, 75);

    long Solve(string input, int iterations)
    {
        var nums = input.Split(" ").Select(long.Parse);
        var stones = new Dictionary<long, long>();
        
        foreach (var x in nums)
            stones[x] = stones.GetValueOrDefault(x, 0) + 1;

        for (int i = 0; i < iterations; i++)
            stones = SimulateOnce(stones);

        return stones.Values.Sum();
    }

    Dictionary<long, long> SimulateOnce(Dictionary<long, long> stones)
    {
        var next = new Dictionary<long, long>();
        
        long add(long value, long count) =>
            next[value] = next.GetValueOrDefault(value, 0) + count;

        foreach (var (value, count) in stones)
        {
            var digits = value.ToString();

            if (value == 0)
            {
                add(1, count);
            }
            else if (digits.Length % 2 == 0)
            {
                var a = long.Parse(digits[..(digits.Length / 2)]);
                var b = long.Parse(digits[(digits.Length / 2)..]);

                add(a, count);
                add(b, count);
            } else
            {
                add(value * 2024, count);
            }
        }

        return next;
    }
}