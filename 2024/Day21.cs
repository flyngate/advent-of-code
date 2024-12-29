using System.Collections.Concurrent;

namespace AdventOfCode.Day21;

public class State(Vec2[] positions, string code)
{
    public readonly Vec2[] Positions = positions;
    public readonly string Code = code;
    private readonly int _hashCode = Enumerable.Aggregate(
        positions.Select(position => position.GetHashCode()).Append(code.GetHashCode()),
        HashCode.Combine
    );

    public override int GetHashCode() =>
        _hashCode;

    public override bool Equals(object? obj) =>
        obj is State;

    public bool Equals(State obj) =>
        obj != null && obj.GetHashCode() == this.GetHashCode();
}

public class Solution
{
    static Dictionary<Vec2, char> NumericKeypad = new() {
        {(0, 0), '7'},
        {(0, 1), '8'},
        {(0, 2), '9'},
        {(1, 0), '4'},
        {(1, 1), '5'},
        {(1, 2), '6'},
        {(2, 0), '1'},
        {(2, 1), '2'},
        {(2, 2), '3'},
        {(3, 0), ' '},
        {(3, 1), '0'},
        {(3, 2), 'A'}
    };

    static Dictionary<Vec2, char> DirectionalKeypad = new() {
        {(0, 0), ' '},
        {(0, 1), '^'},
        {(0, 2), 'A'},
        {(1, 0), '<'},
        {(1, 1), 'v'},
        {(1, 2), '>'}
    };

    ConcurrentDictionary<(int, char, char), long> Cache = new();

    public object PartOne(string input) =>
        Solve(input, 2);

    public object PartTwo(string input) =>
        Solve(input, 25);

    long Solve(string input, int directionalKeypads)
    {
        var codes = input.Split("\n");
        var keypads = Enumerable
            .Repeat(DirectionalKeypad, directionalKeypads)
            .Prepend(NumericKeypad)
            .ToArray();

        return codes
            .Select(code => GetComplexity(code, GetSequenceForKeys(keypads, code)))
            .Sum();
    }

    long GetSequenceForKeys(Dictionary<Vec2, char>[] keypads, string code)
    {
        if (keypads.Length == 0)
            return code.Length;

        char currentCommand = 'A';
        long result = 0;

        foreach (var command in code)
        {
            result += GetSequenceForKey(keypads, currentCommand, command);
            currentCommand = command;
        }

        return result;
    }

    long GetSequenceForKey(Dictionary<Vec2, char>[] keypads, char currentCommand, char nextCommand)
    {
        return Cache.GetOrAdd((keypads.Length, currentCommand, nextCommand), _ => {
            var keypad = keypads[0];
            var currentCommandPos = keypad.First(kv => kv.Value == currentCommand).Key;
            var nextCommandPos = keypad.First(kv => kv.Value == nextCommand).Key;
            var diff = currentCommandPos - nextCommandPos;
            var vertical = new string(diff.X > 0 ? '^' : 'v', Math.Abs(diff.X));
            var horizontal = new string(diff.Y > 0 ? '<' : '>', Math.Abs(diff.Y));
            var result = long.MaxValue;

            if (keypad.GetValueOrDefault((currentCommandPos.X, nextCommandPos.Y)) != ' ')
                result = Math.Min(
                    GetSequenceForKeys(keypads[1..], $"{horizontal}{vertical}A"),
                    result
                );

            if (keypad.GetValueOrDefault((nextCommandPos.X, currentCommandPos.Y)) != ' ')
                result = Math.Min(
                    GetSequenceForKeys(keypads[1..], $"{vertical}{horizontal}A"),
                    result
                );

            return result;
        });
    }

    long GetComplexity(string code, long sequenceLength) =>
        int.Parse(code[0..3]) * sequenceLength;
}
