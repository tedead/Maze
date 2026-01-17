using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Maze
{
    public static class AStar
    {
        //public static List<Node> FindPath(int[,] grid, Point start, Point goal)
        public static async Task<List<Node>> FindPathAsync(int[,] grid, Point start, Point goal,
                                                           Action<List<Node>, Node>? onStep = null, int delayMs = 50)
        {
            var open = new PriorityQueue<Node, int>();
            var closed = new HashSet<Node>();

            var startNode = new Node(start.X, start.Y)
            {
                G = 0,
                H = Heuristic(start, goal)
            };

            open.Enqueue(startNode, startNode.F);

            while (open.Count > 0)
            {
                var current = open.Dequeue();

                if (current.X == goal.X && current.Y == goal.Y)
                {
                    return Reconstruct(current);
                }

                closed.Add(current);

                foreach (var n in Neighbors(current, grid))
                {
                    if (closed.Contains(n))
                    {
                        continue;
                    }

                    int tentativeG = current.G + 1;

                    if (tentativeG < n.G)
                    {
                        n.Parent = current;
                        n.G = tentativeG;
                        n.H = Heuristic(new Point(n.X, n.Y), goal);
                        open.Enqueue(n, n.F);
                    }
                }

                onStep?.Invoke(Reconstruct(current), current);
                await Task.Delay(delayMs);
            }

            return new();
        }

        static IEnumerable<Node> Neighbors(Node n, int[,] grid)
        {
            int[,] d = { { 1, 0 }, { -1, 0 }, { 0, 1 }, { 0, -1 } };

            for (int i = 0; i < 4; i++)
            {
                int x = n.X + d[i, 0];
                int y = n.Y + d[i, 1];

                if (x < 0 || y < 0 || x >= grid.GetLength(0) || y >= grid.GetLength(1))
                {
                    continue;
                }

                if (grid[x, y] == 1)
                {
                    continue;
                }

                yield return new Node(x, y);
            }
        }

        static int Heuristic(Point a, Point b)
        {
           return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }

        static List<Node> Reconstruct(Node end)
        {
            var path = new List<Node>();
            var cur = end;

            while (cur != null)
            {
                path.Add(cur);
                cur = cur.Parent;
            }

            path.Reverse();
            return path;
        }
    }
}
