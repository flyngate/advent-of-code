namespace Day11
{
    enum ArgType { Self, Number };

    enum Op { Add, Mul };

    class Operation
    {
        public Op Op;
        public ArgType ArgType;
        public int RightValue;

        public long Exec(long leftValue)
        {
            var rightValue = ArgType == ArgType.Self ? leftValue : RightValue;

            return Op == Op.Add ? leftValue + rightValue : leftValue * rightValue;
        }
    }

    class Test
    {
        public int DivisibleBy;
        public int TrueMonkey;
        public int FalseMonkey;

        public int GetNextMonkey(long item)
        {
            return item % DivisibleBy == 0 ? TrueMonkey : FalseMonkey;
        }
    }

    class Monkey
    {
        public long InspectedTimes = 0;
        public Queue<long> Items = new Queue<long>();
        public Operation Operation = new Operation();
        public Test Test = new Test();
    }

    class Parser
    {
        public Monkey[] Parse(string[] lines)
        {
            var result = new List<Monkey>();
            int index = 0;

            while (index < lines.Length)
            {
                var items = new Queue<long>(
                    lines[index + 1]
                        .Substring(18)
                        .Split(", ")
                        .Select(x => long.Parse(x))
                );
                var op = lines[index + 2][23] == '+' ? Op.Add : Op.Mul;
                var arg = lines[index + 2].Substring(25);
                var argType = arg == "old" ? ArgType.Self : ArgType.Number;
                var value = argType == ArgType.Self ? 0 : int.Parse(arg);
                var divisibleBy = int.Parse(lines[index + 3].Substring(21));
                var trueMonkey = int.Parse(lines[index + 4].Substring(29));
                var falseMonkey = int.Parse(lines[index + 5].Substring(29));

                var monkey = new Monkey()
                {
                    Items = items,
                    Operation = new Operation()
                    {
                        Op = op,
                        ArgType = argType,
                        RightValue = value,
                    },
                    Test = new Test()
                    {
                        DivisibleBy = divisibleBy,
                        TrueMonkey = trueMonkey,
                        FalseMonkey = falseMonkey,
                    },
                };

                result.Add(monkey);
                index += 6;
            }

            return result.ToArray();
        }
    }

    class PartOne
    {
        public (long Item, int NextMonkey) Inspect(Monkey monkey)
        {
            monkey.InspectedTimes += 1;

            var item = monkey.Items.Dequeue();
            item = monkey.Operation.Exec(item);
            item /= 3;

            var nextMonkey = monkey.Test.GetNextMonkey(item);

            return (item, nextMonkey);
        }

        public long Solve(Monkey[] monkeys)
        {
            var rounds = 20;

            for (var i = 0; i < rounds; i++)
            {
                foreach (var monkey in monkeys)
                {
                    while (monkey.Items.Count > 0)
                    {
                        var (item, nextMonkey) = Inspect(monkey);
                        monkeys[nextMonkey].Items.Enqueue(item);
                    }
                }
            }

            var timesArr = monkeys.Select(monkey => monkey.InspectedTimes).ToArray();
            Array.Sort(timesArr, (a, b) => b.CompareTo(a));

            return timesArr[0] * timesArr[1];
        }
    }

    class PartTwo
    {
        int mod = 1;

        public (long Item, int NextMonkey) Inspect(Monkey monkey)
        {
            monkey.InspectedTimes += 1;

            var item = monkey.Items.Dequeue();
            item = monkey.Operation.Exec(item) % mod;

            var nextMonkey = monkey.Test.GetNextMonkey(item);

            return (item, nextMonkey);
        }

        public long Solve(Monkey[] monkeys)
        {
            var rounds = 10000;

            mod = monkeys
                .Select(monkey => monkey.Test.DivisibleBy)
                .Aggregate(1, (a, b) => a * b);

            for (var i = 0; i < rounds; i++)
            {
                foreach (var monkey in monkeys)
                {
                    while (monkey.Items.Count > 0)
                    {
                        var (item, nextMonkey) = Inspect(monkey);
                        monkeys[nextMonkey].Items.Enqueue(item);
                    }
                }
            }

            var timesArr = monkeys.Select(monkey => monkey.InspectedTimes).ToArray();

            Array.Sort(timesArr, (a, b) => b.CompareTo(a));

            return timesArr[0] * timesArr[1];
        }
    }
}
