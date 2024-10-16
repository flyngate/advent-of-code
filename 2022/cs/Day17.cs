using System.Text.RegularExpressions;

namespace Day17
{
    class Parser
    {
        public string Parse(string[] lines)
        {
            return lines[0];
        }
    }

    public class State
    {
        public int[,] Grid;
        public int PatternIndex = 0;
        public int RockIndex = 0;
        public int Height = -1;

        public State(int width, int height)
        {
            Grid = new int[height, width];
        }

        public string GetKey()
        {
            var lst = new List<string>() {
                PatternIndex.ToString(), "_", RockIndex.ToString(), "_" };

            if (Height > 2)
            {
                lst.Add(string.Concat(Enumerable.Range(0, 7).Select(y => Grid[Height, y])));
                lst.Add("_");
                lst.Add(string.Concat(Enumerable.Range(0, 7).Select(y => Grid[Height - 1, y])));
                lst.Add("_");
                lst.Add(string.Concat(Enumerable.Range(0, 7).Select(y => Grid[Height - 2, y])));
                lst.Add("_");
                lst.Add(string.Concat(Enumerable.Range(0, 7).Select(y => Grid[Height - 3, y])));
            }

            return string.Concat(lst);
        }

        public void Print()
        {
            for (var i = 25; i >= 0; i--)
            {
                for (var j = 0; j < Grid.GetLength(1); j++)
                    Console.Write(Grid[i, j] == 1 ? '#' : '.');

                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine();
        }
    }

    class Simulation
    {
        static int[][] Rock0 = new int[][] {
            new int[] { 1, 1, 1, 1 },
        };

        static int[][] Rock1 = new int[][] {
            new int[] { 0, 1, 0 },
            new int[] { 1, 1, 1 },
            new int[] { 0, 1, 0 },
        };

        static int[][] Rock2 = new int[][] {
            new int[] { 0, 0, 1 },
            new int[] { 0, 0, 1 },
            new int[] { 1, 1, 1 },
        };

        static int[][] Rock3 = new int[][] {
            new int[] { 1 },
            new int[] { 1 },
            new int[] { 1 },
            new int[] { 1 },
        };

        static int[][] Rock4 = new int[][] {
            new int[] { 1, 1 },
            new int[] { 1, 1 },
        };

        static int[][][] Rocks = new int[][][] { Rock0, Rock1, Rock2, Rock3, Rock4 };

        static int Width = 7;

        string Pattern;

        public Simulation(string pattern)
        {
            Pattern = pattern;
        }

        bool TryMove(int[,] grid, int[][] rock, int x, int y)
        {
            if (rock.Length > x + 1)
                return false;

            for (var i = 0; i < rock.Length; i++)
                for (var j = 0; j < rock[0].Length; j++)
                {
                    if (y + j >= grid.GetLength(1) || y + j < 0 || x - i < 0)
                        return false;

                    if (rock[i][j] == 1 && grid[x - i, y + j] == 1)
                        return false;
                }

            return true;
        }

        void Put(int[,] grid, int[][] rock, int x, int y)
        {
            for (var i = 0; i < rock.Length; i++)
                for (var j = 0; j < rock[0].Length; j++)
                    if (rock[i][j] == 1)
                        grid[x - i, y + j] = rock[i][j];
        }

        public void Step(in State state)
        {
            var rock = Rocks[state.RockIndex];
            var x = state.Height + rock.Length + 3;
            var y = 2;

            while (true)
            {
                var direction = Pattern[state.PatternIndex];
                var nextY = direction == '>' ? y + 1 : y - 1;

                if (TryMove(state.Grid, rock, x, nextY))
                    y = nextY;

                state.PatternIndex = (state.PatternIndex + 1) % Pattern.Length;

                var nextX = x - 1;

                if (TryMove(state.Grid, rock, nextX, y))
                    x = nextX;
                else
                    break;
            }

            Put(state.Grid, rock, x, y);

            state.Height = Math.Max(x, state.Height);
            state.RockIndex = (state.RockIndex + 1) % Rocks.Length;
        }
    }

    public class PartOne
    {
        public long Solve(string pattern)
        {
            var iterations = 2022;
            var state = new State(7, 10000);
            var simulation = new Simulation(pattern);

            for (var iteration = 0; iteration < iterations; iteration++)
                simulation.Step(state);

            return state.Height;
        }
    }

    public class PartTwo
    {
        public (State, int, int) FindLoop(string pattern)
        {
            var state = new State(7, 10000);
            var simulation = new Simulation(pattern);
            var loopMap = new Dictionary<string, int>();

            for (var iteration = 0; ; iteration++)
            {
                simulation.Step(state);

                var key = state.GetKey();

                if (loopMap.ContainsKey(key))
                    return (state, state.Height - loopMap[key], iteration);

                loopMap[key] = state.Height;
            }
        }

        public long Solve(string pattern)
        {
            var iterations = 1000000000000L;

            var (loopState, loopHeight, loopIteration) = FindLoop(pattern);

            var result = loopState.Height + loopHeight *
                ((iterations - loopIteration) / loopIteration);

            var iterationsLeft = (int)((iterations - loopIteration) % loopIteration);

            var state = new State(7, 10000);
            var simulation = new Simulation(pattern);

            for (var iteration = 0; iteration < iterationsLeft; iteration++)
                simulation.Step(state);
            
            result += state.Height;

            return result;
        }
    }
}
