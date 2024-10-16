using System.Text.RegularExpressions;

namespace Day16
{
    public class Valve
    {
        public string Name = "";
        public int Rate;
        public string[] LeadsTo = new string[] { };
    }

    public class Valves : Dictionary<string, Valve> { }

    class Parser
    {
        public Valves Parse(string[] lines)
        {
            var result = new Valves();

            var regex = new Regex(
                @"Valve ([^ ]+) has flow rate=([0-9]+); tunnels? leads? to valves? (.+)");

            foreach (var line in lines)
            {
                var match = regex.Match(line);
                var name = match.Groups[1].Value;
                var rate = int.Parse(match.Groups[2].Value);
                var leadsTo = match.Groups[3].Value.Split(", ");

                result[name] = new Valve()
                {
                    Name = name,
                    Rate = rate,
                    LeadsTo = leadsTo,
                };
            }

            return result;
        }
    }

    class PartOne
    {
        public class Node
        {
            public string Name = "";
            public string[] Open = new string[] { };
            public int Time = 30;

            public string Key
            {
                get
                {
                    return Name + "_" + string.Join(",", Open) + "_" + Time.ToString();
                }
            }
        }

        public int Solve(Valves valves)
        {
            var startNode = new Node()
            {
                Name = "AA"
            };

            var sortedValves = valves
                .Values
                .Where(valve => valve.Rate > 0)
                .OrderBy(valve => valve.Rate)
                .Reverse()
                .ToArray();

            var valvesToOpen = valves.Count(valve => valve.Value.Rate > 0);

            var result = 0;
            var resultNode = startNode;

            var pressureDict = new Dictionary<string, int>();
            var seen = new HashSet<string>();

            var heap = new BinaryHeap<Node>(
                1000000,
                (a, b) => pressureDict.GetValueOrDefault(a.Key, 0) >
                    pressureDict.GetValueOrDefault(b.Key, 0)
            );

            heap.Add(startNode);
            pressureDict[startNode.Key] = 0;

            var i = 0;
            for (; ; i++)
            {
                if (heap.Empty())
                    break;

                var node = heap.Pop();
                var pressure = pressureDict.GetValueOrDefault(node.Key, 0);
                var valve = valves[node.Name];

                seen.Add(node.Key);

                if (pressure > result)
                {
                    resultNode = node;
                    result = pressure;
                    Console.WriteLine(result);
                }

                if (node.Open.Length == valvesToOpen)
                    continue;

                var addNode = (Node node, int newPressure) =>
                {
                    var finalPressure = Math.Max(
                        pressureDict.GetValueOrDefault(node.Key, 0),
                        newPressure
                    );

                    var potential = finalPressure;

                    var time = node.Time;
                    foreach (var valve in sortedValves)
                    {
                        if (time < 0)
                            break;

                        if (node.Open.Contains(valve.Name))
                            continue;

                        potential += valve.Rate * time;
                        time -= 2;
                    }

                    if (potential < result)
                        return;

                    if (!seen.Contains(node.Key))
                        heap.Add(node);

                    if (pressureDict.GetValueOrDefault(node.Key, 0) < newPressure)
                        pressureDict[node.Key] = newPressure;
                };

                foreach (var name in valve.LeadsTo)
                {
                    var otherValve = valves[name];

                    var canOpen = !node.Open.Contains(name) &&
                        node.Time > 1 &&
                        otherValve.Rate > 0;

                    if (canOpen)
                    {
                        var newNode = new Node()
                        {
                            Open = node.Open.Append(name).ToArray(),
                            Name = name,
                            Time = node.Time - 2,
                        };

                        var newPressure = pressure +
                            (node.Time - 2) * otherValve.Rate;

                        addNode(newNode, newPressure);
                    }

                    if (node.Time > 0)
                    {
                        var newNode = new Node()
                        {
                            Open = node.Open,
                            Name = name,
                            Time = node.Time - 1,
                        };

                        addNode(newNode, pressure);
                    }

                }
            }

            Console.WriteLine($"iterations {i}");

            return result;
        }
    }

    public class PartTwo
    {
        public class Node
        {
            public string Self = "";
            public string Elephant = "";
            public string[] Open = new string[] { };
            public int Time = 26;

            public string Key
            {
                get
                {
                    var part = String.Compare(Self, Elephant) > 1
                        ? $"{Self}_{Elephant}"
                        : $"{Elephant}_{Self}";

                    return $"{Self}_{Elephant}_{string.Join(",", Open)}_{Time}";
                }
            }
        }

        public int Solve(Valves valves)
        {
            var startNode = new Node()
            {
                Self = "AA",
                Elephant = "AA",
            };

            var sortedValves = valves
                .Values
                .Where(valve => valve.Rate > 0)
                .OrderBy(valve => valve.Rate)
                .Reverse()
                .ToArray();

            var valvesToOpen = valves.Count(valve => valve.Value.Rate > 0);

            var result = 0;
            var resultNode = startNode;

            var pressureDict = new Dictionary<string, int>();
            var seen = new HashSet<string>();

            var heap = new BinaryHeap<Node>(
                1000000,
                (a, b) => pressureDict.GetValueOrDefault(a.Key, 0) >
                    pressureDict.GetValueOrDefault(b.Key, 0)
            );

            heap.Add(startNode);
            pressureDict[startNode.Key] = 0;

            var i = 0;
            for (; ; i++)
            {
                if (heap.Empty())
                    break;

                var node = heap.Pop();
                var pressure = pressureDict.GetValueOrDefault(node.Key, 0);

                if (pressure > result)
                {
                    resultNode = node;
                    result = pressure;
                    Console.WriteLine(
                        $"result={result},heap={heap.Length},seen={seen.Count() / 1000000}M"
                        );
                }

                if (node.Open.Length >= valvesToOpen)
                    continue;

                var valveSelf = valves[node.Self];
                var valveElephant = valves[node.Elephant];

                foreach (var nextValveSelfName in valveSelf.LeadsTo)
                {
                    foreach (var nextValveElephantName in valveElephant.LeadsTo)
                    {
                        var canOpen = (string valveName) =>
                            !node.Open.Contains(valveName)
                            && valves[valveName].Rate > 0;

                        var addNode = (Node newNode, int newPressure) =>
                        {
                            var skip = newNode.Self == newNode.Elephant
                                || newNode.Time < 0;

                            if (skip)
                                return;

                            var finalPressure = Math.Max(
                                pressureDict.GetValueOrDefault(newNode.Key, 0),
                                newPressure
                            );

                            var potential = finalPressure;

                            var time = node.Time;
                            var filteredValves = sortedValves
                                .Where(v => !node.Open.Contains(v.Name))
                                .ToArray();
                            for (var j = 0; j < filteredValves.Length - 1; j += 2)
                            {
                                if (time < 0)
                                    break;

                                potential +=
                                    filteredValves[j].Rate * (time - 1) +
                                    filteredValves[j+1].Rate * (time - 1);
                                time -= 2;
                            }

                            if (potential < result)
                                return;

                            if (!seen.Contains(newNode.Key))
                            {
                                seen.Add(newNode.Key);
                                heap.Add(newNode);
                            }

                            if (pressureDict.GetValueOrDefault(newNode.Key, 0) < newPressure)
                                pressureDict[newNode.Key] = newPressure;
                        };

                        var nextValveSelf = valves[nextValveSelfName];
                        var nextValveElephant = valves[nextValveElephantName];
                        var nextNodes = new List<Node>();

                        addNode(
                            new Node()
                            {
                                Self = nextValveSelf.Name,
                                Elephant = nextValveElephant.Name,
                                Open = node.Open,
                                Time = node.Time - 1,
                            },
                            pressure
                        );

                        if (canOpen(node.Self))
                            addNode(
                                new Node()
                                {
                                    Self = node.Self,
                                    Elephant = nextValveElephant.Name,
                                    Open = node.Open.Append(node.Self).ToArray(),
                                    Time = node.Time - 1,
                                },
                                pressure + valves[node.Self].Rate * (node.Time - 1)
                            );

                        if (canOpen(node.Elephant))
                            addNode(
                                new Node()
                                {
                                    Self = nextValveSelf.Name,
                                    Elephant = node.Elephant,
                                    Open = node.Open.Append(node.Elephant).ToArray(),
                                    Time = node.Time - 1,
                                },
                                pressure + valves[node.Elephant].Rate * (node.Time - 1)
                            );

                        if (canOpen(node.Self) && canOpen(node.Elephant))
                            addNode(
                                new Node()
                                {
                                    Self = node.Self,
                                    Elephant = node.Elephant,
                                    Open = node.Open
                                        .Append(node.Self)
                                        .Append(node.Elephant)
                                        .ToArray(),
                                    Time = node.Time - 1,
                                },
                                pressure +
                                    valves[node.Self].Rate * (node.Time - 1) +
                                    valves[node.Elephant].Rate * (node.Time - 1)
                            );
                    }
                }
            }

            Console.WriteLine($"iterations {i}");

            return result;
        }
    }
}
