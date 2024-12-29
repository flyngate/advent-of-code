namespace AdventOfCode;

enum InputType
{
    Sample,
    Full
}

class Runner
{

    public static void Main()
    {
        var input = ReadInput(InputType.Sample);
        var result = new Day23.Solution().PartOne(input);

        Console.WriteLine(result);
    }

    static string ReadInput(InputType type)
    {
        // var basePath = "/Users/igor/code/problems/advent-of-code/2024/cs";
        var path = type == InputType.Sample ? "input.sample.txt" : "input.txt";

        return System.IO.File.ReadAllText(path).TrimEnd();
    }

}