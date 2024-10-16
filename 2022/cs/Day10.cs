namespace Day10
{
    class Op
    {
        public string name = "";
        public int arg = 0;
        public int cycles = 0;
    }

    class Parser
    {
        public Op[] Parse(string[] lines)
        {
            return lines.Select(line =>
            {
                var parts = line.Split(" ");

                if (parts.Length > 1)
                {
                    return new Op()
                    {
                        name = "addx",
                        arg = int.Parse(parts[1]),
                        cycles = 2
                    };
                }

                return new Op() { name = "noop", cycles = 1 };
            }).ToArray();
        }
    }

    class PartOne
    {
        public int Solve(Op[] input)
        {
            int x = 1;
            int cycle = 0;
            int targetCycle = 20;
            int result = 0;

            foreach (var op in input)
            {
                if (cycle + op.cycles >= targetCycle)
                {
                    var signalStrength = x * targetCycle;
                    result += signalStrength;
                    targetCycle += 40;
                }

                if (op.name == "addx")
                {
                    x += op.arg;
                }

                cycle += op.cycles;
            }

            return result;
        }
    }

    class PartTwo
    {
        const int width = 40;
        const int height = 6;
        string screen = "";
        int x = 1;
        int cycle = 0;

        void Draw(int cycle, int x)
        {
            var lit = Math.Abs(cycle % width - x) <= 1;

            screen += lit ? "#" : ".";

            if ((cycle + 1) % width == 0)
            {
                screen += "\n";
            }
        }

        public string Solve(Op[] input)
        {
            foreach (var op in input)
            {
                if (op.name == "addx")
                {
                    Draw(cycle, x);
                    Draw(cycle + 1, x);
                    x += op.arg;
                }
                else
                {
                    Draw(cycle, x);
                }

                cycle += op.cycles;
            }

            return screen;
        }
    }
}
