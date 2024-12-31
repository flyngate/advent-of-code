namespace AdventOfCode.Day24;

using Gate = (string Type, string In1, string In2, string Out);

public class Solution
{
    public object PartOne(string input)
    {
        var (gates, signals) = Parse(input);

        return Run(gates, signals);
    }

    public object PartTwo(string input)
    {
        var (gates, _) = Parse(input);

        List<(int, int)> fixes = [
            // (37, 98),
            // (139, 144),
            // (59, 172),
            // (4, 197)
        ];
        HashSet<Gate> whitelist = [];

        for (int wire = 0; wire < 46; wire++)
        {
            if (CheckWire(gates, wire))
            {
                Console.WriteLine($"{GetWireName('z', wire)}: ok");

                var (_, activated) = GetWireValue(gates, [], GetWireName('z', wire));

                foreach (var gate in activated)
                    whitelist.Add(gate);

                continue;
            }

            Console.WriteLine($"{GetWireName('z', wire)}: corrupted; trying to fix the wire...");

            if (TryFix(gates, wire, whitelist) is (int, int) fix)
            {
                var (i, j) = fix;

                Console.WriteLine($"{GetWireName('z', wire)}: fixed (swap {i} and {j})");
                fixes.Add(fix);
                SwapOutputs(ref gates, i, j);
            }
            else
            {
                Console.WriteLine($"{GetWireName('z', wire)}: can't fix the wire");
            }
        }

        return GetResult(gates, fixes);
    }

    string GetResult(Gate[] gates, IEnumerable<(int, int)> fixes)
    {
        var indexes = fixes.SelectMany(fix => new int[] { fix.Item1, fix.Item2 }).Distinct();
        var outputs = indexes.Select(i => gates[i].Out).Order();

        return string.Join(",", outputs);
    }

    (int, int)? TryFix(Gate[] gates, int wire, HashSet<Gate> whitelist)
    {
        for (int i = 0; i < gates.Length; i++)
        {
            for (int j = i + 1; j < gates.Length; j++)
            {
                if (whitelist.Contains(gates[i]) || whitelist.Contains(gates[j]))
                    continue;

                SwapOutputs(ref gates, i, j);

                try
                {
                    if (CheckWire(gates, wire))
                        return (i, j);
                }
                catch
                { }

                SwapOutputs(ref gates, i, j);
            }
        }

        return null;
    }

    void SwapOutputs(ref Gate[] gates, int i, int j)
    {
        var gate1 = gates[i];
        var gate2 = gates[j];

        gates[i] = gate1 with { Out = gate2.Out };
        gates[j] = gate2 with { Out = gate1.Out };
    }

    bool CheckWire(Gate[] gates, int bit)
    {
        var iterations = 500;
        var wire = GetWireName('z', bit);
        var wires = gates
            .SelectMany(gate => new string[] { gate.In1, gate.In2, gate.Out })
            .Distinct()
            .ToArray();
        long opMask = ~(-1L << 45);
        var random = new Random();

        for (int i = 0; i < iterations; i++)
        {
            var x = random.NextInt64() & opMask;
            var y = random.NextInt64() & opMask;
            var z = x + y;
            var signals = GetSignals(x, 'x', 45)
                .Concat(GetSignals(y, 'y', 45))
                .ToDictionary();
            var expectedBitValue = (z >> bit) & 1;
            var (gotBitValue, activated) = GetWireValue(gates, signals, wire);

            if (expectedBitValue != gotBitValue)
                return false;
        }

        return true;
    }

    string GetWireName(char prefix, int number) =>
        $"{prefix}{number.ToString().PadLeft(2, '0')}";

    IEnumerable<KeyValuePair<string, int>> GetSignals(long x, char prefix, int bits)
    {
        for (int i = 0; i < bits; i++)
        {
            var wire = GetWireName(prefix, i);
            yield return new KeyValuePair<string, int>(wire, (int)(x & 1));
            x >>= 1;
        }
    }

    long Run(Gate[] gates, Dictionary<string, int> signals)
    {
        var zWires = gates.Select(gate => gate.Out)
            .Where(wire => wire.StartsWith('z'))
            .Order()
            .Reverse()
            .ToArray();

        var bits = string.Join("", zWires.Select(wire => GetWireValue(gates, signals, wire).Item1));

        return Convert.ToInt64(bits, 2);
    }

    (int, HashSet<Gate>) GetWireValue(Gate[] gates, Dictionary<string, int> signals, string wire, int depth = 0)
    {
        var gatesByOut = (
            from gate in gates
            select new KeyValuePair<string, Gate>(gate.Out, gate)
        ).ToDictionary();

        return GetWireValue(gatesByOut, signals, wire);
    }

    (int, HashSet<Gate>) GetWireValue(Dictionary<string, Gate> gatesByOut, Dictionary<string, int> signals, string wire, int depth = 0)
    {
        if (depth > 1000)
            throw new Exception();

        if (signals.ContainsKey(wire))
            return (signals[wire], []);

        if (wire.StartsWith('x') || wire.StartsWith('y'))
            return (0, []);

        var gate = gatesByOut[wire];
        var (in1, activated1) = GetWireValue(gatesByOut, signals, gate.In1, depth + 1);
        var (in2, activated2) = GetWireValue(gatesByOut, signals, gate.In2, depth + 1);

        var output = gate.Type switch
        {
            "AND" => in1 & in2,
            "OR" => in1 | in2,
            _ => in1 ^ in2
        };

        return (output, activated1.Concat(activated2).Append(gate).ToHashSet());
    }

    (Gate[], Dictionary<string, int>) Parse(string input)
    {
        var blocks = input.Split("\n\n");
        Dictionary<string, int> signals = [];

        foreach (var line in blocks[0].Split("\n"))
            signals[line[..3]] = int.Parse(line[5..]);

        var gates = (
            from line in blocks[1].Split("\n")
            let parts = line.Split([" -> ", " "], StringSplitOptions.None)
            select (parts[1], parts[0], parts[2], parts[3])
        ).ToArray();

        return (gates, signals);
    }
}