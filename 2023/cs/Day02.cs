using System;
using System.Text.RegularExpressions;

namespace Day02
{
    public struct Game {
        public int id;
        public List<int> red;
        public List<int> green;
        public List<int> blue;
    }

    public class Parser
    {
        public List<Game> Parse(string[] lines)
        {
            var games = new List<Game>();
            var gameRegex = new Regex(@"Game (\d+): (.+)");
            var cubeSetRegex = new Regex(@"(\d+) (.+)");

            foreach (var line in lines) {
                var matches1 = gameRegex.Match(line);
                var id = int.Parse(matches1.Groups[1].Value);
                var sets = matches1.Groups[2].Value;
                var cubeSets = sets.Split(new char[] { ',', ';' });
                var red = new List<int>();
                var green = new List<int>();
                var blue = new List<int>();

                foreach (var cubeSet in cubeSets) {
                    var matches2 = cubeSetRegex.Match(cubeSet);
                    var count = int.Parse(matches2.Groups[1].Value);
                    var color = matches2.Groups[2].Value;

                    if (color == "red") {
                        red.Add(count);
                    } else if (color == "green") {
                        green.Add(count);
                    } else if (color == "blue") {
                        blue.Add(count);
                    }
                }

                games.Add(new Game() {
                    id = id,
                    red = red,
                    green = green,
                    blue = blue,
                });
            }

            return games;
        }
    }

    public class PartOne
    {
        public int Solve(List<Game> games)
        {
            int result = 0;

            foreach (var game in games) {
                var possible =
                    game.red.All((count) => count <= 12) &&
                    game.green.All((count) => count <= 13) &&
                    game.blue.All((count) => count <= 14);
                
                if (possible) {
                    result += game.id;
                    Console.WriteLine(game.id);
                }
            }

            return result;
        }
    }

    public class PartTwo
    {
        int GetMax(List<int> numbers) {
            return numbers.Count() > 0 ? numbers.Max() : 0;
        }

        public int Solve(List<Game> games)
        {
            int result = 0;

            foreach (var game in games) {
                var product =
                    GetMax(game.red) *
                    GetMax(game.green) *
                    GetMax(game.blue);
                
                Console.WriteLine(product);

                result += product;
            }

            return result;
        }
    }
}
