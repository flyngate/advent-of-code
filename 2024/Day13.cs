using System.Numerics;
using System.Text.RegularExpressions;

namespace AdventOfCode.Day13;

record struct Pair(long X, long Y);
record struct Machine(Pair A, Pair B, Pair Price);

public class Solution
{
    public object PartOne(string input)
    {
        var machines = Parse(input);

        return Solve(machines);
    }

    public object PartTwo(string input)
    {
        var offset = 10000000000000;
        var machines = Parse(input)
            .Select(machine => machine with
            {
                Price = new Pair(machine.Price.X + offset, machine.Price.Y + offset)
            });

        return Solve(machines);
    }

    BigInteger Solve(IEnumerable<Machine> machines)
    {
        return (
            from machine in machines
            select GetTokens(machine)
        ).Aggregate((acc, x) => acc + x);
    }

    BigInteger GetTokens(Machine machine)
    {
        var (a, b, price) = machine;
        double n = 1.0 * (price.Y * b.X - price.X * b.Y) / (b.X * a.Y - a.X * b.Y);
        double m = 1.0 * (price.X * a.Y - price.Y * a.X) / (b.X * a.Y - a.X * b.Y);

        if (Math.Truncate(n) == n && Math.Truncate(m) == m)
            return new BigInteger(n) * 3 + new BigInteger(m);

        return 0;
    }

    Machine[] Parse(string input)
    {
        Pair parsePair(string line)
        {
            var nums = Regex
                .Matches(line, @"\d+")
                .Select(match => long.Parse(match.Value));

            return new Pair(nums.First(), nums.Last());
        }

        var result = from block in input.Split("\n\n")
                     let lines = block.Split("\n")
                     select new Machine(
                         parsePair(lines[0]),
                         parsePair(lines[1]),
                         parsePair(lines[2]));

        return result.ToArray();
    }
}