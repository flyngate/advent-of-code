namespace AdventOfCode.Day25;

public class Solution
{
    public object PartOne(string input)
    {
        var blocks = input.Split("\n\n");
        var locks = blocks.Where(block => block[0] == '#').Select(ParseBlock);
        var keys = blocks.Where(block => block[0] == '.').Select(ParseBlock);
        var result = 0;

        foreach (var _lock in locks)
            foreach (var key in keys)
                if (Fits(_lock, key))
                    result += 1;

        return result;
    }

    bool Fits(int[] _lock, int[] key) =>
        _lock.Zip(key).Select(pair => pair.First + pair.Second).All(value => value <= 5);

    int[] ParseBlock(string block)
    {
        var lines = block.Split("\n");

        return (
            from i in Enumerable.Range(0, 5)
            select (
                from j in Enumerable.Range(0, 7)
                where lines[j][i] == '#'
                select 1
            ).Count() - 1
        ).ToArray();
    }
}
