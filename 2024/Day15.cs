using System.Data;
using System.Numerics;

namespace AdventOfCode.Day15;

public class Solution
{
    Complex Up = new(-1, 0);
    Complex Right = new(0, 1);
    Complex Down = new(1, 0);
    Complex Left = new(0, -1);

    public object PartOne(string input)
    {
        var (map, moves, pos) = Parse(input);

        foreach (var move in moves)
            pos = Move(ref map, pos, move);

        return SumCoords(map, 'O');
    }

    public object PartTwo(string input)
    {
        var (map, moves, pos) = Parse(input);
        var expanded = new Dictionary<Complex, char>();

        foreach (var p in map.Keys)
        {
            var value = map[p];

            if (value == '#')
            {
                expanded.Add(new Complex(p.Real, p.Imaginary * 2), '#');
                expanded.Add(new Complex(p.Real, p.Imaginary * 2 + 1), '#');
            }

            if (value == 'O')
            {
                expanded.Add(new Complex(p.Real, p.Imaginary * 2), '[');
                expanded.Add(new Complex(p.Real, p.Imaginary * 2 + 1), ']');
            }

            if (value == '@')
            {
                expanded.Add(new Complex(p.Real, p.Imaginary * 2), '@');
            }
        }

        map = expanded;

        pos = (from p in map.Keys
            where map[p] == '@'
            select p).First();

        foreach (var move in moves)
        {
            pos = Move(ref map, pos, move);
        }

        return SumCoords(map, '[');
    }

    Complex Move(ref Dictionary<Complex, char> map, Complex pos, Complex move)
    {
        var snapshot = new Dictionary<Complex, char>(map);

        if (!TryMove(ref map, pos, move))
        {
            map = snapshot;

            return pos;
        }

        return pos + move;
    }

    bool TryMove(ref Dictionary<Complex, char> map, Complex pos, Complex move)
    {
        var nextPos = pos + move;
        var nextBlock = map.GetValueOrDefault(nextPos, '.');

        if (nextBlock == '#')
            return false;

        List<Complex> blocksToMove = nextBlock switch
        {
            'O' => [nextPos],
            '[' when move == Left => [nextPos],
            '[' => [nextPos + Right, nextPos],
            ']' when move == Right => [nextPos],
            ']' => [nextPos + Left, nextPos],
            _ => [],
        };

        foreach (var p in blocksToMove)
            if (!TryMove(ref map, p, move))
                return false;

        map[nextPos] = map[pos];
        map.Remove(pos);

        return true;
    }

    int SumCoords(Dictionary<Complex, char> map, char target) => (
        from xy in map.Keys
        where map[xy] == target
        select (int)(xy.Real * 100 + xy.Imaginary)
    ).Sum();

    (Dictionary<Complex, char>, Complex[], Complex) Parse(string input)
    {
        var parts = input.Split("\n\n");
        var mapLines = parts.First().Split("\n");
        var map = (
            from i in Enumerable.Range(0, mapLines[0].Length)
            from j in Enumerable.Range(0, mapLines.Length)
            where mapLines[i][j] != '.'
            select new KeyValuePair<Complex, char>(
                new Complex(i, j),
                mapLines[i][j]
            )
        ).ToDictionary()!;
        var moves = parts.Last().Replace("\n", "").Select(m => m switch
            {
                '^' => Up,
                '>' => Right,
                'v' => Down,
                _ => Left,
            }).ToArray();
        var pos = map.First(kv => kv.Value == '@').Key;

        return (map, moves, pos);
    }
}