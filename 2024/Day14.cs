using System.Numerics;
using System.Text.RegularExpressions;

namespace AdventOfCode.Day14;

record Robot(Vector2 P, Vector2 V);

public class Solution
{
    const int width = 101;
    const int height = 103;

    public object PartOne(string input)
    {
        var iterations = 100;
        var robots = Parse(input);

        for (int i = 0; i < iterations; i++)
            Simulate(ref robots, width, height);

        return GetRobotsInQuadrants(robots).Aggregate((a, b) => a * b);
    }

    public object PartTwo(string input)
    {
        var maxIterations = 10000;
        var robots = Parse(input);
        var initialRobots = robots.ToArray();
        int maxRobotsInOneQuadrant = 0;
        int maxRobotsInOneQuadrantIter = 0;

        for (int i = 0; i < maxIterations; i++)
        {
            Simulate(ref robots, width, height);

            var x = GetRobotsInQuadrants(robots).Max();

            if (x > maxRobotsInOneQuadrant)
            {
                maxRobotsInOneQuadrant = x;
                maxRobotsInOneQuadrantIter = i + 1;
            }
        }

        robots = initialRobots;

        for (int i = 0; i < maxRobotsInOneQuadrantIter; i++)
            Simulate(ref robots, width, height);

        PrintRobots(robots);

        return maxRobotsInOneQuadrantIter;
    }

    void Simulate(ref Robot[] robots, int width, int height)
    {
        var n = 1;

        for (int i = 0; i < robots.Length; i++)
        {
            var robot = robots[i];
            var x = ((robot.P.X + robot.V.X * n) % width + width) % width;
            var y = ((robot.P.Y + robot.V.Y * n) % height + height) % height;
            var p = robot.P with { X = x, Y = y };

            robots[i] = robot with { P = p };
        }
    }

    int[] GetRobotsInQuadrants(Robot[] robots)
    {
        return [
            CountRobots(robots, 0, 0, width / 2 - 1, height / 2 - 1),
            CountRobots(robots, width / 2 + 1, 0, width, height / 2 - 1),
            CountRobots(robots, 0, height / 2 + 1, width / 2 - 1, height),
            CountRobots(robots, width / 2 + 1, height / 2 + 1, width, height)
        ];
    }

    int CountRobots(in Robot[] robots, int x1, int y1, int x2, int y2) =>
        robots.Count(robot =>
            robot.P.X >= x1 && robot.P.X <= x2 && robot.P.Y >= y1 && robot.P.Y <= y2);

    Robot[] Parse(string input)
    {
        Robot parseLine(string line)
        {
            var match = Regex.Match(line, @"p=(.+),(.+) v=(.+),(.+)");
            var nums = match.Groups.Values
                .Skip(1)
                .Select(group => group.Value)
                .Select(int.Parse)
                .ToArray();

            return new Robot(
                new Vector2(nums[0], nums[1]),
                new Vector2(nums[2], nums[3])
            );
        }

        return input.Split("\n").Select(parseLine).ToArray();
    }

    void PrintRobots(Robot[] robots)
    {
        bool[,] buf = new bool[height, width];

        foreach (var robot in robots)
        {
            var p = (int)robot.P.Y;
            var q = (int)robot.P.X;

            buf[p, q] = true;
        }

        for (int p = 0; p < height; p++)
        {
            for (int q = 0; q < width; q++)
                Console.Write(buf[p, q] ? '#' : '.');

            Console.WriteLine();
        }
    }
}