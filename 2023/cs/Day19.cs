using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO.Compression;
using System.IO.Pipes;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Xml.Schema;
using System.Xml.XPath;
using AdventOfCode;

namespace Day19
{
    public record Part(int X, int M, int A, int S)
    {
        public int Rating { get => X + M + A + S; }
    }

    public record Rule(string Field, string Operator, int Value, string Workflow)
    {
        public bool Test(Part part)
        {
            if (Operator == "")
                return true;

            int value = 0;

            switch (Field)
            {
                case "x": value = part.X; break;
                case "m": value = part.M; break;
                case "a": value = part.A; break;
                case "s": value = part.S; break;
            }

            if (Operator == ">")
                return value > Value;

            return value < Value;
        }
    }

    public record Workflow(Rule[] Rules);

    public record Input(Dictionary<string, Workflow> Workflows, Part[] Parts);

    public class Parser
    {
        public Input Parse(string[] lines, string content)
        {
            var blocks = content.Split("\n\n");

            Dictionary<string, Workflow> workflows = new();

            foreach (var line in blocks[0].Split("\n"))
            {
                var regex = new Regex(@"(\w+){(.+)}$");
                var match = regex.Match(line);
                var name = match.Groups[1].Value;
                var rules = match.Groups[2].Value
                    .Split(",")
                    .Select(rule =>
                    {
                        var colonIndex = rule.IndexOf(":");

                        if (colonIndex == -1)
                            return new Rule("", "", 0, rule);

                        return new Rule(
                            rule[0..1],
                            rule[1..2],
                            int.Parse(rule[2..colonIndex]),
                            rule[(colonIndex + 1)..]
                        );
                    }).ToArray();
                var workflow = new Workflow(rules);

                workflows.Add(name, workflow);
            }

            var parts = blocks[1]
                .Split("\n")
                .Select(line =>
                {
                    var regex = new Regex(@"{x=(\d+),m=(\d+),a=(\d+),s=(\d+)}");
                    var match = regex.Match(line);
                    var x = int.Parse(match.Groups[1].Value);
                    var m = int.Parse(match.Groups[2].Value);
                    var a = int.Parse(match.Groups[3].Value);
                    var s = int.Parse(match.Groups[4].Value);

                    return new Part(x, m, a, s);
                }).ToArray();

            return new Input(workflows, parts);
        }
    }

    public class PartOne
    {
        public Parser parser = new();

        public bool IsAccepted(Dictionary<string, Workflow> workflows, Part part)
        {
            var workflow = workflows["in"];

            while (true)
            {
                foreach (var rule in workflow.Rules)
                {
                    if (!rule.Test(part))
                        continue;

                    if (rule.Workflow == "A")
                        return true;

                    if (rule.Workflow == "R")
                        return false;

                    workflow = workflows[rule.Workflow];
                    break;
                }
            }
        }

        public int Solve(Input input)
        {
            var ratings = input.Parts
                .Where(part => IsAccepted(input.Workflows, part))
                .Select(part => part.Rating)
                .ToArray();

            return ratings.Sum();
        }
    }

    public record Constraints(
        long X1, long X2, long M1, long M2, long A1, long A2, long S1, long S2)
    {
        public long Combinations
        {
            get => (X2 - X1 - 1) * (M2 - M1 - 1) * (A2 - A1 - 1) * (S2 - S1 - 1);
        }

        public bool Valid
        {
            get => X2 - X1 > 2 && M2 - M1 > 2 && A2 - A1 > 2 && S2 - S1 > 2;
        }
    }

    public class PartTwo
    {
        public Parser parser = new();

        long Rec(Dictionary<string, Workflow> workflows, string name, Constraints constraints)
        {
            if (!constraints.Valid)
                return 0;
            
            if (name == "A")
                return constraints.Combinations;
            
            if (name == "R")
                return 0;

            var workflow = workflows[name];
            long result = 0;

            foreach (var rule in workflow.Rules)
            {
                if (rule.Operator == "")
                {
                    result += Rec(workflows, rule.Workflow, constraints);
                    break;
                }

                Constraints constraintsTrue = constraints;
                Constraints constraintsFalse = constraints;

                if (rule.Operator == ">")
                {
                    if (rule.Field == "x")
                    {
                        constraintsTrue = constraints with { X1 = rule.Value };
                        constraintsFalse = constraints with { X2 = rule.Value + 1 };
                    }

                    if (rule.Field == "m")
                    {
                        constraintsTrue = constraints with { M1 = rule.Value };
                        constraintsFalse = constraints with { M2 = rule.Value + 1 };
                    }

                    if (rule.Field == "a")
                    {
                        constraintsTrue = constraints with { A1 = rule.Value };
                        constraintsFalse = constraints with { A2 = rule.Value + 1 };
                    }

                    if (rule.Field == "s")
                    {
                        constraintsTrue = constraints with { S1 = rule.Value };
                        constraintsFalse = constraints with { S2 = rule.Value + 1 };
                    }
                }

                if (rule.Operator == "<")
                {
                    if (rule.Field == "x")
                    {
                        constraintsTrue = constraints with { X2 = rule.Value };
                        constraintsFalse = constraints with { X1 = rule.Value - 1 };
                    }

                    if (rule.Field == "m")
                    {
                        constraintsTrue = constraints with { M2 = rule.Value };
                        constraintsFalse = constraints with { M1 = rule.Value - 1 };
                    }

                    if (rule.Field == "a")
                    {
                        constraintsTrue = constraints with { A2 = rule.Value };
                        constraintsFalse = constraints with { A1 = rule.Value - 1 };
                    }

                    if (rule.Field == "s")
                    {
                        constraintsTrue = constraints with { S2 = rule.Value };
                        constraintsFalse = constraints with { S1 = rule.Value - 1 };
                    }
                }

                result += Rec(
                    workflows,
                    rule.Workflow,
                    constraintsTrue
                );

                constraints = constraintsFalse;
            }

            return result;
        }

        public long Solve(Input input)
        {
            return Rec(
                input.Workflows,
                "in",
                new Constraints(0, 4001, 0, 4001, 0, 4001, 0, 4001)
            );
        }
    }
}
