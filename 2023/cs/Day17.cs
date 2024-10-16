using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO.Compression;
using System.IO.Pipes;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Xml.Schema;
using System.Xml.XPath;
using AdventOfCode;

namespace Day17
{
    public struct Node
    {
        public int X;
        public int Y;
        public Direction D;
        public int L;

        readonly static Dictionary<Direction, int[]> offsets = new Dictionary<Direction, int[]> {
                { Direction.Right, [0, 1] },
                { Direction.Down, [1, 0] },
                { Direction.Left, [0, -1] },
                { Direction.Up, [-1, 0] }
            };

        public static Node Next(int x, int y, Direction d, int l = 1)
        {
            var offset = offsets[d];

            return new Node() { X = x + offset[0], Y = y + offset[1], D = d, L = l };
        }

        public override string ToString()
        {
            return $"{{{X}, {Y}, {D}, {L}}}";
        }
    }

    public struct Edge
    {
        public Node node;
        public int weight;

        public override string ToString()
        {
            return $"{{{node}, {weight}}}";
        }
    }

    public class Grid
    {
        public int[][] grid;

        public bool IsValidNode(Node node)
        {
            return node.X >= 0 && node.Y >= 0 &&
                node.X < grid.Length && node.Y < grid[0].Length;
        }
    }

    public enum Direction
    {
        Right = 0,
        Down = 1,
        Left = 2,
        Up = 3,
    }

    public class Parser
    {
        public Grid Parse(string[] lines, string content)
        {
            return new Grid()
            {
                grid = lines.Select(line =>
                {
                    return line.Select(c => int.Parse(c.ToString())).ToArray();
                }).ToArray()
            };
        }
    }

    public class PartOne
    {
        public Parser parser = new();

        Dictionary<Node, HashSet<Edge>> MakeGraph(Grid grid, int minMoves, int maxMoves)
        {
            var rows = grid.grid.Length;
            var cols = grid.grid[0].Length;
            Dictionary<Node, HashSet<Edge>> connections = [];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    foreach (Direction d in Enum.GetValues(typeof(Direction)))
                        for (int l = 1; l <= maxMoves; l++)
                        {
                            var node = new Node() { X = i, Y = j, D = d, L = l };
                            var nextNodes = new List<Node>();

                            if (l >= minMoves)
                            {
                                if (d == Direction.Right || d == Direction.Left)
                                {
                                    var node1 = Node.Next(i, j, Direction.Up);
                                    var node2 = Node.Next(i, j, Direction.Down);

                                    nextNodes.AddRange([node1, node2]);
                                }

                                if (d == Direction.Up || d == Direction.Down)
                                {
                                    var node1 = Node.Next(i, j, Direction.Left);
                                    var node2 = Node.Next(i, j, Direction.Right);

                                    nextNodes.AddRange([node1, node2]);
                                }
                            }

                            if (l < maxMoves)
                                nextNodes.Add(Node.Next(i, j, d, l + 1));

                            if (!connections.ContainsKey(node))
                                connections[node] = [];

                            var adjacent = connections[node];

                            foreach (var nextNode in nextNodes)
                            {
                                if (!grid.IsValidNode(nextNode))
                                    continue;

                                var weight = grid.grid[nextNode.X][nextNode.Y];
                                var edge = new Edge() { node = nextNode, weight = weight };

                                adjacent.Add(edge);
                            }
                        }

            return connections;
        }

        int Dijkstra(Dictionary<Node, HashSet<Edge>> connections, int rows, int cols, int minMoves, int maxMoves)
        {
            var minDistanceDict = new Dictionary<Node, int>();
            var heap = new BinaryHeap<Node>(
                connections.Count,
                (Node a, Node b) =>
                    minDistanceDict.GetValueOrDefault(a, int.MaxValue) >
                    minDistanceDict.GetValueOrDefault(b, int.MaxValue),
                BinaryHeapType.MinHeap
            );

            Node[] startNodes = [
                new Node() { X = 0, Y = 0, D = Direction.Right, L = 1},
                new Node() { X = 0, Y = 0, D = Direction.Down, L = 1}
            ];

            foreach (var startNode in startNodes)
            {
                heap.Add(startNode);
                minDistanceDict[startNode] = 0;
            }

            while (heap.Length > 0)
            {
                var node = heap.Pop();
                var edges = connections[node];

                foreach (var edge in edges)
                {
                    bool addToHeap = !minDistanceDict.ContainsKey(edge.node);

                    if (minDistanceDict.GetValueOrDefault(edge.node, int.MaxValue) > minDistanceDict[node] + edge.weight)
                        minDistanceDict[edge.node] = minDistanceDict[node] + edge.weight;

                    if (addToHeap)
                        heap.Add(edge.node);
                }
            }

            var result = int.MaxValue;

            foreach (var direction in new Direction[] { Direction.Down, Direction.Right })
            {
                for (int l = minMoves; l <= maxMoves; l++)
                {
                    var node = new Node()
                    {
                        X = rows - 1,
                        Y = cols - 1,
                        D = direction,
                        L = l
                    };

                    if (!minDistanceDict.ContainsKey(node))
                        continue;

                    var minDistance = minDistanceDict[node];

                    if (result > minDistance)
                        result = minDistance;
                }
            }

            return result;
        }

        public int FindMinHeatLoss(Grid grid, int minMoves, int maxMoves)
        {
            var rows = grid.grid.Length;
            var cols = grid.grid[0].Length;
            var connections = MakeGraph(grid, minMoves, maxMoves);

            return Dijkstra(connections, rows, cols, minMoves, maxMoves);
        }

        public int Solve(Grid grid)
        {
            return FindMinHeatLoss(grid, 1, 3);
        }
    }

    public class PartTwo : PartOne
    {
        public int Solve(Grid grid)
        {
            return FindMinHeatLoss(grid, 4, 10);
        }
    }
}
