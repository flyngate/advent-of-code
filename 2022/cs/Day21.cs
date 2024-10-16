using System.Text.RegularExpressions;
using System.Numerics;
using System.Linq;

namespace Day21
{
    public class Node { }

    public class Operation : Node
    {
        public string left = "", right = "", op = "";
    }

    public class Number : Node
    {
        public decimal value;
    }

    public class Tree : Dictionary<string, Node> { }

    class Parser
    {
        public Operation ParseOperation(string str)
        {
            var regex = new Regex(@"([a-z]+) ([+\-*/]) ([a-z]+)");
            var match = regex.Match(str);

            return new Operation()
            {
                left = match.Groups[1].Value,
                right = match.Groups[3].Value,
                op = match.Groups[2].Value
            };
        }

        public Tree Parse(string[] lines)
        {
            var result = new Tree();

            foreach (var line in lines)
            {
                var parts = line.Split(": ");

                if (parts[1].Contains(" "))
                    result[parts[0]] = ParseOperation(parts[1]);
                else
                    result[parts[0]] = new Number() { value = decimal.Parse(parts[1]) };
            }

            return result;
        }
    }

    class Calculator
    {
        public static decimal Calculate(Tree tree, string nodeId)
        {
            Node node = tree[nodeId];

            if (node is Number)
                return (node as Number).value;

            var operation = node as Operation;
            var left = Calculate(tree, operation.left);
            var right = Calculate(tree, operation.right);

            switch (operation.op)
            {
                case "+":
                    return left + right;
                case "-":
                    return left - right;
                case "*":
                    return left * right;
                case "/":
                default:
                    return Math.Floor(left / right);
            }

        }
    }

    public class PartOne
    {
        public decimal Solve(Tree tree)
        {
            return Calculator.Calculate(tree, "root");
        }
    }

    public class PartTwo
    {
        public decimal Solve(Tree tree)
        {
            var root = tree["root"] as Operation;
            var min = Decimal.Parse("0");
            var max = Decimal.Parse("1000000000000000");

            while (max - min > 0)
            {
                var middle = Math.Floor((min + max) / 2);

                tree["humn"] = new Number() { value = middle };

                var left = Calculator.Calculate(tree, root.left);
                var right = Calculator.Calculate(tree, root.right);

                if (left > right)
                    min = middle + 1;
                else
                    max = middle;
            }

            return min;
        }
    }
}
