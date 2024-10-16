namespace AdventOfCode
{
    enum Test {
        Easy,
        Hard,
    }

    class Runner
    {
        public static void Main()
        {
            var parser = new Day22.Parser();
            var solution = new Day22.PartOne();
            var test = Test.Hard;

            var path = test == Test.Easy
                ? "/Users/igor/Downloads/input.sample.txt"
                : "/Users/igor/Downloads/input.txt";
            var lines = System.IO.File.ReadAllLines(path);
            var filteredLines = lines.Last() == "" ?
                lines.SkipLast(1).ToArray()
                : lines;
            var input = parser.Parse(filteredLines);
            var result = solution.Solve(input);
            Console.WriteLine(result);
        }
    }
}
