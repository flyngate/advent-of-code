namespace AdventOfCode
{
    public class Gcd
    {
        public static long Run(long a, long b) =>
            b == 0 ? a : Run(b, a % b);

        public static long Run(long[] numbers) =>
            numbers.Aggregate(Gcd.Run);
    }
}
