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
using System.Text;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Xml.Schema;
using System.Xml.XPath;
using AdventOfCode;

namespace Day15
{
    public class Parser
    {
        public string[] Parse(string[] lines, string content)
        {
            return lines[0].Split(',').ToArray();
        }
    }

    public class Hash
    {
        public static int Get(string s)
        {
            var bytes = Encoding.ASCII.GetBytes(s);
            var result = 0;

            foreach (var ascii in bytes)
            {
                result += ascii;
                result *= 17;
                result %= 256;
            }

            return result;

        }
    }

    public class PartOne
    {
        public Parser parser = new();

        public int Solve(string[] steps)
        {
            return steps.Select(Hash.Get).Sum();
        }
    }

    public class PartTwo
    {
        public Parser parser = new();

        public int Solve(string[] steps)
        {
            var boxes = new Dictionary<int, List<(string, int)>>();

            foreach (var step in steps)
            {
                var regex = new Regex(@"^([a-z]+)([-=])(.*)");
                var match = regex.Match(step);
                var lens = match.Groups[1].Value;
                var operation = match.Groups[2].Value;
                var focalLength = operation == "=" ? int.Parse(match.Groups[3].Value) : 0;
                var boxNumber = Hash.Get(lens);

                boxes.TryAdd(boxNumber, new List<(string, int)>());

                var box = boxes[boxNumber];
                var existingLensIndex = box.FindIndex(pair => pair.Item1 == lens);

                if (operation == "-")
                    if (existingLensIndex != -1)
                        box.RemoveAt(existingLensIndex);

                if (operation == "=")
                {
                    if (existingLensIndex != -1)
                        box[existingLensIndex] = (lens, focalLength);
                    else
                        box.Add((lens, focalLength));
                }
            }

            var result = 0;

            foreach (var (i, box) in boxes)
            {
                for (var j = 0; j < boxes[i].Count; j++)
                {
                    result += (i + 1) * (j + 1) * boxes[i][j].Item2;
                }
            }

            return result;
        }
    }
}
