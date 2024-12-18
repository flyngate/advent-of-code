namespace AdventOfCode
{
    public class Debug
    {
        public static void PrintMatrix<T>(Func<int, int, T> GetItem, int rows, int columns, string separator = "")
            where T : notnull
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                    Console.Write(GetItem(i, j).ToString() + separator);
                Console.WriteLine();
            }
        }

        public static void PrintMatrix<T>(T[,] matrix, string separator = "")
            where T : notnull
        {
            PrintMatrix(
                (int x, int y) => matrix[x, y],
                matrix.GetLength(0),
                matrix.GetLength(1),
                separator
            );
        }

        public static void PrintMatrix<T>(T[][] matrix, string separator = "")
            where T : notnull
        {
            PrintMatrix(
                (int x, int y) => matrix[x][y],
                matrix.Length,
                matrix[0].Length,
                separator
            );
        }
    }
}
