namespace AdventOfCode.Day04;

public class Parser
{
    public char[][] Parse(string[] lines)
    {
        return lines.Select(line => line.ToArray()).ToArray();
    }
}

public class PartOne
{
    char[][] chars = [];

    public int Solve(char[][] chars)
    {
        this.chars = chars;
        int r = 0;

        for (int i = 0; i < chars.Length; i++)
        {
            for (int j = 0; j < chars[0].Length; j++)
            {
                r += Count(i, j, 1, 0);
                r += Count(i, j, 0, 1);
                r += Count(i, j, 1, 1);
                r += Count(i, j, 1, -1);
            }
        }

        return r;
    }
    int Count(int x, int y, int dirX, int dirY)
    {
        int maxX = x + dirX * 3;
        int maxY = y + dirY * 3;

        if (maxX < 0 || maxY < 0 || maxX >= chars.Length || maxY >= chars[0].Length)
        {
            return 0;
        }

        var s = new string([
            chars[x][y],
                    chars[x + dirX][y + dirY],
                    chars[x + dirX * 2][y + dirY * 2],
                    chars[x + dirX * 3][y + dirY * 3]
        ]);

        return s == "XMAS" || s == "SAMX" ? 1 : 0;
    }
}

public class PartTwo
{
    public int Solve(char[][] chars)
    {
        int r = 0;

        for (int i = 0; i < chars.Length - 2; i++)
        {
            for (int j = 0; j < chars[0].Length - 2; j++)
            {
                var s1 = $"{chars[i][j]}{chars[i + 1][j + 1]}{chars[i + 2][j + 2]}";
                var s2 = $"{chars[i][j + 2]}{chars[i + 1][j + 1]}{chars[i + 2][j]}";
                var b1 = s1 == "MAS" || s1 == "SAM";
                var b2 = s2 == "MAS" || s2 == "SAM";

                if (b1 && b2)
                {
                    r += 1;
                }
            }
        }

        return r;
    }
}