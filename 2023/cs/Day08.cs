using System;
using System.ComponentModel;
using System.Data;
using System.IO.Compression;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Xml.Schema;
using System.Xml.XPath;

namespace Day08
{

    public class LeftRight
    {
        public string left;
        public string right;
    }

    public class Network
    {
        public string directions;
        public Dictionary<string, LeftRight> edges;
    }

    public class Parser
    {
        public Network Parse(string[] lines, string content)
        {
            Dictionary<string, LeftRight> edges = new();

            foreach (var line in lines.Skip(2))
            {
                var parts = line.Split(" = ");
                var node = parts[0];
                var left = parts[1][1..4];
                var right = parts[1][6..9];

                edges.Add(node, new LeftRight() { left = left, right = right });
            }

            return new Network()
            {
                directions = lines[0],
                edges = edges,
            };
        }
    }

    public class PartOne
    {
        public Parser parser = new();

        public int Solve(Network network)
        {
            var currentNode = "AAA";
            var steps = 0;

            while (currentNode != "ZZZ")
            {
                var direction = network.directions[steps % network.directions.Length];
                var leftRight = network.edges[currentNode];

                currentNode = direction == 'L' ? leftRight.left : leftRight.right;
                steps += 1;
            }

            return steps;
        }
    }

    public class PartTwo
    {
        public Parser parser = new();

        public (string, int, int) FindCycle(Network network, string start)
        {
            var currentNode = start;
            var steps = 0;
            var potentialCycle = ("", -1, -1);

            while (true)
            {
                var index = steps % network.directions.Length;
                var direction = network.directions[index];
                var leftRight = network.edges[currentNode];

                currentNode = direction == 'L' ? leftRight.left : leftRight.right;
                steps += 1;

                if (currentNode.EndsWith("Z"))
                {
                    if (potentialCycle.Item1 == "") {
                        potentialCycle = (currentNode, index, steps);
                    } else {
                        if (potentialCycle.Item1 != currentNode || potentialCycle.Item2 != index) {
                            Console.WriteLine("!!!");
                        }

                        return potentialCycle;
                    }
                }
            }
        }

        long GCD(long a, long b)
        {
            if (b == 0) return a;

            return GCD(b, a % b);
        }

        public long Solve(Network network)
        {
            var nodes = new List<string>();
            long steps = 0;
            bool stop;
            var cycles = new List<(string, int, int)>();

            foreach (var node in network.edges.Keys)
            {
                if (node.EndsWith("A"))
                {
                    var cycle = FindCycle(network, node);

                    cycles.Add(cycle);

                    // nodes.Add(node);
                }
            }

            long result = cycles[0].Item3;
            
            foreach (var cycle in cycles.Skip(1)) {
                result = result * cycle.Item3 / GCD(result, cycle.Item3);
            }

            return result;

            // do
            // {
            //     stop = true;
            //     var nextNodes = new List<string>();
            //     var direction = network.directions[(int)(steps % network.directions.Length)];

            //     foreach (var node in nodes)
            //     {
            //         var leftRight = network.edges[node];
            //         var nextNode = direction == 'L' ? leftRight.left : leftRight.right;

            //         if (!nextNode.EndsWith("Z"))
            //         {
            //             stop = false;
            //         }

            //         nextNodes.Add(nextNode);
            //     }

            //     var count = nextNodes.Count(node => node.EndsWith("Z"));
            //     if (count > nextNodes.Count / 2) {
            //         Console.WriteLine($"{count}, {steps}");
            //     }

            //     nodes = nextNodes;
            //     steps += 1;
            // } while (!stop);

            // return steps;
        }
    }
}
