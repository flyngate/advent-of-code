namespace AdventOfCode.Day17;

record struct Registers(ulong A, ulong B, ulong C);

public class Solution
{
    public object PartOne(string input)
    {
        var (registers, program) = Parse(input);
        var output = Run(registers, program);

        return string.Join(",", output);
    }

    public object PartTwo(string input)
    {
        var (registers, program) = Parse(input);

        return FindMagicNumber(registers, program, 0, 0, 0);
    }

    ulong FindMagicNumber(Registers registers, int[] program, ulong current, int shift, int matchedNumbers)
    {
        if (matchedNumbers == program.Length)
            return current;

        for (ulong i = 0; i < 1024; i++)
        {
            var x = current + (i << shift);
            var output = Run(registers with { A = x }, program);

            if (output.Length > program.Length)
                continue;
            
            int j = 0;
            for (; j < output.Length && output[j] == program[j]; j++);

            if (j > matchedNumbers)
            {
                var magic = FindMagicNumber(registers, program, x, shift + 10, j);

                if (magic > 0)
                    return magic;
            }
        }

        return 0;
    }

    int[] Run(Registers registers, int[] program)
    {
        var initA = registers.A;
        List<int> output = [];

        for (int i = 0; i < program.Length - 1; i += 2)
        {
            var op = program[i];
            ulong operand = (ulong)program[i + 1];
            ulong literal = operand;
            ulong combo = operand switch
            {
                _ when operand < 4 => operand,
                4 => registers.A,
                5 => registers.B,
                6 => registers.C,
                _ => throw new Exception("unknown operand"),
            };

            registers = op switch
            {
                0 => registers with { A = registers.A >> (int)combo },
                1 => registers with { B = registers.B ^ literal },
                2 => registers with { B = combo % 8 },
                4 => registers with { B = registers.B ^ registers.C },
                6 => registers with { B = registers.A >> (int)combo },
                7 => registers with { C = registers.A >> (int)combo },
                _ => registers,
            };

            if (op == 3 && registers.A != 0)
                i = (int)literal - 2;

            if (op == 5)
            {
                output.Add((int)(combo % 8));
                
                if (output.Count > 20)
                {
                    Console.WriteLine($"max output reached when A={initA.ToString("X")} ({string.Join(",", output)})");

                    break;
                }
            }
        }

        return [.. output];
    }

    (Registers, int[]) Parse(string input)
    {
        var lines = input.Split("\n");
        var a = ulong.Parse(lines[0][12..]);
        var b = ulong.Parse(lines[1][12..]);
        var c = ulong.Parse(lines[2][12..]);
        var registers = new Registers(a, b, c);
        var program = lines[4][9..].Split(",").Select(int.Parse).ToArray();

        return (registers, program);
    }
}