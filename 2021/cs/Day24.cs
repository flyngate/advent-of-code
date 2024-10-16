using System.Numerics;

// run with 13799949489194


class Day24 {
    public static void Main() {
        // var input = File.ReadAllText("./input.txt");
        var input = File.ReadAllText("/Users/igor/Downloads/input.txt");
        var program = new Parser().Parse(input);

        // var number = "13799949489194";
        var number = "92793949489995";
        var vm = new VM(number.Select((c) => Int32.Parse(c.ToString())));
        vm.Run(program);
        Console.WriteLine(vm.Vars["z"]);

        // var result = new Solution().Part1(program);
        // Console.WriteLine(result);
    }
}

class Arg {
    public string Type;
    public string Var;
    public int Value;

    public Arg(string type, string var, int value) {
        this.Type = type;
        this.Var = var;
        this.Value = value;
    }

    public override string ToString()
    {
        return Type == "var" ? Var : Value.ToString();
    }
}

class Instr {
    public string Type;
    public Arg Arg1;
    public Arg Arg2;

    public Instr(string type, Arg arg1, Arg arg2) {
        this.Type = type;
        this.Arg1 = arg1;
        this.Arg2 = arg2;
    }
}

class VM {
    public Dictionary<string, long> Vars = new Dictionary<string, long>() {
        { "w", 0 },
        { "x", 0 },
        { "y", 0 },
        { "z", 0 },
    };
    
    IList<int> InputNumbers;
    int CurrentInputNumber;

    public VM(IEnumerable<int> inputNumbers) {
        this.InputNumbers = inputNumbers.ToList();
    }

    long GetValue(Arg arg) {
        if (arg.Type == "var") {
            return Vars[arg.Var];
        }

        return arg.Value;
    }

    void Run(Instr instr) {
        var type = instr.Type;
        var arg1 = instr.Arg1;
        var arg2 = instr.Arg2;

        if (type == "inp") {
            Vars[instr.Arg1.Var] = InputNumbers[CurrentInputNumber];
            CurrentInputNumber++;
        }

        if (type == "add") {
            Vars[arg1.Var] += GetValue(arg2);
        }

        if (type == "mul") {
            Vars[arg1.Var] *= GetValue(arg2);
        }

        if (type == "div") {
            Vars[arg1.Var] /= GetValue(arg2);
        }

        if (type == "mod") {
            Vars[arg1.Var] %= GetValue(arg2);
        }

        if (type == "eql") {
            Vars[arg1.Var] = Vars[arg1.Var] == GetValue(arg2) ? 1 : 0;
        }
    }

    public void Run(IEnumerable<Instr> program) {
        foreach (var instr in program) {
            Run(instr);
        }
    }
}

class Parser {
    public IList<Instr> Parse(string input) {
        var result = new List<Instr>();
        var lines = input.Split("\n");

        foreach (var line in lines) {
            var parts = line.Split(" ");
            var arg1 = parts.Length > 1 ? ParseArg(parts[1]) : null;
            var arg2 = parts.Length > 2 ? ParseArg(parts[2]) : null;
            
            result.Add(new Instr(parts[0], arg1, arg2));
        }

        return result;
    }

    Arg ParseArg(string str) {
        var vars = new string[]{ "w", "x", "y", "z" };

        if (vars.Contains(str)) {
            return new Arg("var", str, 0);
        }

        return new Arg("value", "", Int32.Parse(str));
    }
}

class ModelNumber {
    BigInteger Counter;

    public ModelNumber(int counter) {
        this.Counter = new BigInteger(counter);
    }

    public BigInteger Get() {
        return BigInteger.Parse("137999" + Counter + "194");
    }

    public void Decrement() {
        Counter--;

        if (Counter < 0) {
            throw new Exception("end");
        }
    }
}

class Solution {
    public BigInteger Part1(IList<Instr> program) {
        var start = BigInteger.Parse("13799952000000");
        var end = BigInteger.Parse("11111111111111");
        long min = 1000000000;

        for (var modelNumber = start; modelNumber > end; modelNumber--) {
            if (modelNumber % BigInteger.Pow(10, 6) == 0) {
                Console.WriteLine($"Checking {modelNumber}...");
            }

            if (!modelNumber.ToString().Contains("0")) {
                var inputNumbers = modelNumber
                    .ToString()
                    .Select((digit) => Int32.Parse(digit.ToString()))
                    .ToList();
                var vm = new VM(inputNumbers);

                vm.Run(program);

                if (vm.Vars["z"] < min) {
                    min = vm.Vars["z"];
                    Console.WriteLine($"{modelNumber}, Min = {vm.Vars["z"]}");
                }

                // Console.WriteLine($"{modelNumber} > {vm.Vars["z"]}");

                if (vm.Vars["z"] == 1) {
                    return modelNumber;
                }
            }

        }

        return 0;
    }

    public BigInteger Part1_Alt(IList<Instr> program) {
        long min = 1000000000;
        var iteration = 0;

        for (var modelNumber = new ModelNumber(99999); ; modelNumber.Decrement()) {
            var modelNumberValue = modelNumber.Get();
            iteration++;

            if (iteration % 10000 == 0) {
                Console.WriteLine($"Checking {modelNumberValue}...");
            }

            if (!modelNumberValue.ToString().Contains("0")) {
                var inputNumbers = modelNumberValue
                    .ToString()
                    .Select((digit) => Int32.Parse(digit.ToString()))
                    .ToList();
                var vm = new VM(inputNumbers);

                vm.Run(program);

                if (vm.Vars["z"] < min) {
                    min = vm.Vars["z"];
                    Console.WriteLine($"{modelNumberValue}, Min = {vm.Vars["z"]}");
                }

                if (vm.Vars["z"] == 1) {
                    return modelNumber.Get();
                }
            }

        }

        return 0;
    }
}