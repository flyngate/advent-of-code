
namespace AdventOfCode
{
    public class Point
    {
        public int x;
        public int y;

        public Point(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public int ManhattanDistance(Point other)
        {
            return Math.Abs(x - other.x) + Math.Abs(y - other.y);
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Point other)
                return false;

            return x == other.x && y == other.y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y);
        }

        public override string ToString()
        {
            return $"{{{x}, {y}}}";
        }
    }
}
