using System;
using System.Data;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Xml.Schema;
using System.Xml.XPath;

namespace Day06
{
    public class Race
    {
        public long time;
        public long distance;
    }

    public class ParserPartOne
    {
        public Race[] Parse(string[] lines, string content)
        {
            var timeList = lines[0]
                .Split(':')[1]
                .Split(' ')
                .Where(x => x != "")
                .Select(long.Parse);

            var distanceList = lines[1]
                .Split(':')[1]
                .Split(' ')
                .Where(x => x != "")
                .Select(long.Parse);

            return timeList.Zip(distanceList).Select((pair) => new Race()
            {
                time = pair.First,
                distance = pair.Second,
            }).ToArray();
        }
    }

    public class PartOne
    {
        public ParserPartOne parser = new();

        public long Solve(Race[] races)
        {
            var result = 1;

            foreach (var race in races)
            {
                int count = 0;

                for (int t = 0; t < race.time; t++)
                {
                    if ((race.time - t) * t > race.distance)
                    {
                        count++;
                    }
                }

                result *= count;
            }

            return result;
        }
    }

    public class ParserPartTwo
    {
        public Race Parse(string[] lines, string content)
        {
            var partTwoTime = long.Parse(lines[0]
                .Split(':')[1]
                .Replace(" ", ""));

            var partTwoDistance = long.Parse(lines[1]
                .Split(':')[1]
                .Replace(" ", ""));

            return new Race()
            {
                time = partTwoTime,
                distance = partTwoDistance,
            };
        }
    }
    public class PartTwo
    {
        public ParserPartTwo parser = new();

        public long Solve(Race race)
        {
            int count = 0;

            for (int t = 0; t < race.time; t++)
            {
                if ((race.time - t) * t > race.distance)
                {
                    count++;
                }

                if (t % 1000000 == 0)
                {
                    Console.WriteLine($"t={t} of {race.time}");
                }
            }

            return count;
        }
    }
}
