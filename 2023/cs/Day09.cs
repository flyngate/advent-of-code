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

namespace Day09
{

    public class Parser
    {
        public List<int[]> Parse(string[] lines, string content)
        {
            List<int[]> result = new();

            foreach (var line in lines)
            {
                var history = line.Split(" ").Select(int.Parse).ToArray();

                result.Add(history);
            }

            return result;
        }
    }

    public class PartOne
    {
        public Parser parser = new();

        int Extrapolate(int[] history)
        {
            var current = history;
            var lastValues = new List<int>() { history.Last() };

            while (!current.All(value => value == current[0]))
            {
                var next = new List<int>();

                for (int i = 1; i < current.Length; i++)
                {
                    next.Add(current[i] - current[i - 1]);
                }

                current = next.ToArray();
                lastValues.Add(current.Last());
            }

            return lastValues.Sum();
        }

        public int Solve(List<int[]> historyList)
        {
            return historyList.Select(Extrapolate).Sum();
        }
    }

    public class PartTwo
    {
        public Parser parser = new();

        int ExtrapolateBackwards(int[] history)
        {
            var current = history;
            var firstValues = new List<int> { history.First() };

            while (!current.All(value => value == current[0]))
            {
                var next = new List<int>();

                for (int i = 1; i < current.Length; i++)
                    next.Add(current[i] - current[i - 1]);

                current = next.ToArray();
                firstValues.Add(current.First());
            }

            return firstValues
                .Select((value, index) => index % 2 == 0 ? value : -value)
                .Sum();
        }

        public int Solve(List<int[]> historyList)
        {
            return historyList.Select(ExtrapolateBackwards).Sum();
        }
    }
}
