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
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Xml.Schema;
using System.Xml.XPath;
using AdventOfCode;
using Microsoft.VisualBasic;

namespace Day12
{
    public class Record
    {
        public string Springs;
        public int[] Groups;
        int[] CanFitMax;
        public int LastBrokenSpringIndex;

        public Record(string springs, int[] groups)
        {
            Springs = springs;
            Groups = groups;
            CanFitMax = new int[springs.Length];

            for (int i = CanFitMax.Length - 1; i >= 0; i--)
            {
                if (Springs[i] == '.')
                    continue;

                CanFitMax[i] = i < CanFitMax.Length - 1 ? CanFitMax[i + 1] + 1 : 1;
            }

            for (int i = 0; i < Springs.Length; i++)
                if (Springs[i] == '#')
                    LastBrokenSpringIndex = i;
        }

        public bool CanFit(int start, int len)
        {
            if (start > 0 && Springs[start - 1] == '#')
                return false;

            if (start + len < Springs.Length && Springs[start + len] == '#')
                return false;

            return CanFitMax[start] >= len;
        }
    }

    public class Parser
    {
        public Record[] Parse(string[] lines, string content)
        {
            List<Record> result = new();

            foreach (var line in lines)
            {
                var parts = line.Split(" ");
                var springs = parts[0];
                var groups = parts[1].Split(",").Select(int.Parse).ToArray();

                result.Add(new Record(springs, groups));
            }

            return result.ToArray();
        }
    }

    public class PartOne
    {
        public Parser parser = new();

        protected long GetPossibleArrangements(Record record)
        {
            Dictionary<(int, int), long> memo = new();

            long Rec(int index, int groupIndex)
            {
                if (memo.ContainsKey((index, groupIndex)))
                    return memo[(index, groupIndex)];

                if (groupIndex == record.Groups.Length)
                {
                    if (record.LastBrokenSpringIndex < index)
                        return 1;

                    return 0;
                }

                if (index >= record.Springs.Length)
                    return 0;

                var group = record.Groups[groupIndex];
                long result = 0;

                for (int j = index; j < record.Springs.Length - group + 1; j++)
                {
                    if (record.CanFit(j, group))
                    {
                        var localResult = Rec(j + group + 1, groupIndex + 1);
                        var key = (j + group + 1, groupIndex + 1);

                        if (!memo.ContainsKey(key))
                            memo[key] = localResult;

                        result += localResult;
                    }

                    if (record.Springs[j] == '#')
                        break;
                }

                return result;
            }

            return Rec(0, 0);
        }

        public long Solve(Record[] records)
        {
            var arrangements = records.Select(GetPossibleArrangements).ToArray();

            return arrangements.Sum();
        }
    }

    public class PartTwo : PartOne
    {

        public long Solve(Record[] records)
        {
            var unfoldedRecords = records
                .Select(record => new Record(
String.Join("?", Enumerable.Repeat(record.Springs, 5)),
Enumerable.Repeat(record.Groups, 5).SelectMany(x => x).ToArray()
                )).ToArray();
            long sum = 0;

            for (int i = 0; i < unfoldedRecords.Length; i++)
            {
                sum += GetPossibleArrangements(unfoldedRecords[i]);
                Console.WriteLine(i);
            }

            return sum;
        }
    }
}
