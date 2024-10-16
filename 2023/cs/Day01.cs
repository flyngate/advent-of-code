using System;
using System.Text.RegularExpressions;

namespace Day01
{
    public class Parser
    {
        public string[] Parse(string[] lines)
        {
            return lines;
        }
    }

    public class PartOne
    {
        string[] Digits = new string[]{
            "one", "two", "three", "four", "five", "six", "seven", "eight", "nine"};

        public int Solve(string[] lines)
        {
            int result = 0;

            foreach (var line in lines)
            {
                int firstDigit = -1;
                int lastDigit = -1;

                for (int i = 0; i < line.Length; i++) {
                    var slice = line[i..];

                    if (Char.IsDigit(slice[0])) {
                        var digit = int.Parse($"{slice[0]}");

                        if (firstDigit == -1) {
                            firstDigit = digit;
                        }

                        lastDigit = digit;
                    }

                    var digitIndex = Array.FindIndex(Digits, (digit) => slice.StartsWith(digit));

                    if (digitIndex >= 0) {
                        var digit = digitIndex + 1;

                        if (firstDigit == -1) {
                            firstDigit = digit;
                        }

                        lastDigit = digit;
                    }
                }

                var number = $"{firstDigit}{lastDigit}";

                Console.WriteLine($"{line} {number}");

                result += int.Parse(number);
            }

            return result;
        }
    }
}
