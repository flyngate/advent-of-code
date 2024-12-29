namespace AdventOfCode.Day22;

public class Solution
{
    const int N = 2000;

    public object PartOne(string input)
    {
        var secrets = input.Split("\n").Select(int.Parse).ToArray();

        return secrets.Select(secret => (long)GetSecrets(secret).Last()).Sum();
    }

    public object PartTwo(string input)
    {
        var secrets = input.Split("\n").Select(int.Parse).ToArray();

        return (
            from secret in secrets
            from sequencePrices in GetSequencePrices(secret)
            group sequencePrices by sequencePrices.Key into gr
            select gr.Sum(item => item.Value)
        ).Max();
    }

    Dictionary<(int, int, int, int), int> GetSequencePrices(int seed)
    {
        Dictionary<(int, int, int, int), int> result = [];
        var prices = GetPrices(seed);
        var diffs = GetDiffs(prices);

        for (int i = 3; i < diffs.Length; i++)
        {
            var seq = (diffs[i - 3], diffs[i - 2], diffs[i - 1], diffs[i]);

            result.TryAdd(seq, prices[i + 1]);
        }

        return result;
    }

    int[] GetDiffs(int[] values) =>
        values.Zip(values.Skip(1)).Select((pair) => pair.Second - pair.First).ToArray();

    int[] GetPrices(int seed) =>
        GetSecrets(seed).Select(secret => secret % 10).ToArray();

    IEnumerable<int> GetSecrets(int seed)
    {
        long secret = seed;

        for (int i = 0; i < N; i++)
        {
            secret = Prune(Mix(secret, secret << 6));
            secret = Prune(Mix(secret, secret >> 5));
            secret = Prune(Mix(secret, secret << 11));

            yield return (int)secret;
        }
    }

    long Mix(long a, long b) => a ^ b;

    long Prune(long a) => a % 16777216;
}
