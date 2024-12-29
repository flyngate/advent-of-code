namespace AdventOfCode;

public record struct Vec2(int X, int Y)
{
    public static Vec2 operator +(Vec2 a, Vec2 b) =>
        new(a.X + b.X, a.Y + b.Y);

    public static Vec2 operator -(Vec2 a, Vec2 b) =>
        new(a.X - b.X, a.Y - b.Y);

    public static Vec2 operator *(Vec2 a, int b) =>
        new(a.X * b, a.Y * b);

    public static implicit operator Vec2((int, int) v) =>
        new(v.Item1, v.Item2);

    public override readonly string ToString() =>
        $"({X}, {Y})";
}