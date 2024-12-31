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
        var input = ReadInput(InputType.Full);
        var result = new Day25.Solution().PartOne(input);

        Console.WriteLine(result);
    }

    static string ReadInput(InputType type)
    {
        var path = type == InputType.Sample ? "input.sample.txt" : "input.txt";

        return System.IO.File.ReadAllText(path).TrimEnd();
    }

}