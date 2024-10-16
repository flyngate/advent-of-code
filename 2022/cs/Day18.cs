using System.Text.RegularExpressions;
using System.Numerics;

namespace Day18
{
    class Parser
    {
        public Vector3[] Parse(string[] lines)
        {
            return lines.Select(line =>
            {
                var nums = line.Split(",").Select(num => int.Parse(num)).ToArray();

                return new Vector3() { X = nums[0], Y = nums[1], Z = nums[2] };
            }).ToArray();
        }
    }

    public class Helpers
    {
        public static bool IsAdjacent(Vector3 a, Vector3 b)
        {
            return (a.X == b.X && a.Y == b.Y && Math.Abs(a.Z - b.Z) == 1)
                || (a.X == b.X && a.Z == b.Z && Math.Abs(a.Y - b.Y) == 1)
                || (a.Y == b.Y && a.Z == b.Z && Math.Abs(a.X - b.X) == 1);
        }
    }

    public class PartOne
    {
        public int Solve(Vector3[] vectors)
        {
            var adjacentCount = 0;

            for (var i = 0; i < vectors.Length; i++)
                for (var j = i + 1; j < vectors.Length; j++)
                    if (Helpers.IsAdjacent(vectors[i], vectors[j]))
                        adjacentCount += 1;

            return vectors.Length * 6 - adjacentCount * 2;
        }
    }

    public class PartTwo
    {
        Vector3[] GetAdjacent(
            Vector3 vector,
            Vector3 min,
            Vector3 max)
        {
            var diffArr = new Vector3[] {
                new Vector3(1, 0, 0),
                new Vector3(-1, 0, 0),
                new Vector3(0, 1, 0),
                new Vector3(0, -1, 0),
                new Vector3(0, 0, 1),
                new Vector3(0, 0, -1)
            };

            return diffArr.Select(diff => vector + diff).Where(v =>
                v.X <= max.X && v.Y <= max.Y && v.Z <= max.Z &&
                v.X >= min.X && v.Y >= min.Y && v.Z >= min.Z
            ).ToArray();
        }

        public int Solve(Vector3[] vectors)
        {
            var partOne = new PartOne();
            var min = new Vector3(100, 100, 100);
            var max = new Vector3(-1, -1, -1);
            var seen = new HashSet<Vector3>();
            var queue = new Queue<Vector3>();
            var result = 0;

            foreach (var v in vectors)
            {
                min.X = Math.Min(min.X, v.X);
                min.Y = Math.Min(min.Y, v.Y);
                min.Z = Math.Min(min.Z, v.Z);
                max.X = Math.Max(max.X, v.X);
                max.Y = Math.Max(max.Y, v.Y);
                max.Z = Math.Max(max.Z, v.Z);
            }

            min -= new Vector3(1, 1, 1);
            max += new Vector3(1, 1, 1);

            queue.Enqueue(min);

            while (queue.Count() > 0)
            {
                var vector = queue.Dequeue();

                foreach (var adjacent in GetAdjacent(vector, min, max))
                {
                    if (vectors.Contains(adjacent))
                        result += 1;
                    else if (!seen.Contains(adjacent))
                    {
                        queue.Enqueue(adjacent);
                        seen.Add(adjacent);
                    }
                }
            }

            return result;
        }
    }
}
