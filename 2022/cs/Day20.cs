using System.Text.RegularExpressions;
using System.Numerics;
using System.Linq;

namespace Day20
{
    class Parser
    {
        public long[] Parse(string[] lines)
        {
            return lines.Select(long.Parse).ToArray();
        }
    }

    public class Box
    {
        public long value;
    }

    public class Helper
    {
        public static int[] Indexes = new int[] { 1000, 2000, 3000 };

        public static long GetGroveCoordinatesSum(long[] numbers)
        {
            var zeroIndex = numbers.ToList().FindIndex(0, x => x == 0);

            var getNthMixedNumberAfterZero = (int index) =>
                numbers[(zeroIndex + index) % numbers.Length];

            var number1 = getNthMixedNumberAfterZero(Indexes[0]);
            var number2 = getNthMixedNumberAfterZero(Indexes[1]);
            var number3 = getNthMixedNumberAfterZero(Indexes[2]);

            return number1 + number2 + number3;
        }
    }

    public class PartOne
    {
        public long[] Mix(long[] numbers)
        {
            var length = numbers.Length;
            var initial = numbers.Select(x => new Box() { value = x }).ToList();
            var mixed = initial.ToList();

            foreach (var box in initial)
            {
                var index = mixed.FindIndex(0, _box => _box == box);
                var direction = box.value > 0 ? 1 : -1;

                for (var i = 0; i < Math.Abs(box.value); i++)
                {
                    var nextIndex = index + direction;

                    if (nextIndex < 0)
                        nextIndex = length - 1;

                    if (nextIndex >= length)
                        nextIndex = 0;

                    var temp = mixed[index];
                    mixed[index] = mixed[nextIndex];
                    mixed[nextIndex] = temp;

                    index = nextIndex;
                }
            }

            return mixed.Select(box => box.value).ToArray();
        }

        public long Solve(long[] numbers)
        {
            var mixed = Mix(numbers);

            return Helper.GetGroveCoordinatesSum(mixed);
        }
    }

    public class PartTwo
    {
        const long DecryptionKey = 811589153;
        const int MixTimes = 10;

        int GetNextIndex(int index, long value, int length)
        {
            return (int)((index + value) % length + length) % length;
        }

        Box[] Mix(Box[] initialBoxes, Box[] boxes)
        {
            var length = initialBoxes.Count();
            var result = boxes.ToList();

            foreach (var box in initialBoxes)
            {
                var index = result.FindIndex(0, _box => _box == box);

                result.RemoveAt(index);

                var nextIndex = GetNextIndex(index, box.value, length - 1);

                result.Insert(nextIndex, box);
            }

            return result.ToArray();
        }

        public long Solve(long[] numbers)
        {
            var actualNumbers = numbers
                .Select(value => value * DecryptionKey)
                .ToArray();
            var initialBoxes = actualNumbers
                .Select(value => new Box() { value = value })
                .ToArray();
            var boxes = initialBoxes.ToArray();

            for (var i = 0; i < MixTimes; i++)
                boxes = Mix(initialBoxes, boxes);

            var mixed = boxes.Select(box => box.value).ToArray();

            return Helper.GetGroveCoordinatesSum(mixed);
        }
    }
}
