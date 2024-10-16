using System.Reflection.Metadata;

namespace AdventOfCode
{
    enum Test {
        Sample,
        Full,
    }

    class Runner
    {
        public static void Main()
        {
            var solution = new Day22.PartOne();
            var test = Test.Sample;

            var parser = solution.parser;
            var path = test == Test.Sample
                ? "/Users/igor/Downloads/input.sample.txt"
                : "/Users/igor/Downloads/input.txt";
            var lines = System.IO.File.ReadAllLines(path);
            var filteredLines = lines.Last() == "" ?
                lines.SkipLast(1).ToArray()
                : lines;
            var content = String.Join("\n", lines);
            var input = parser.Parse(filteredLines, content);
            var result = solution.Solve(input);
            Console.WriteLine(result);
        }
    }
}
