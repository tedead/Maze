using System;
using System.Drawing;

namespace Maze
{
    public sealed class MazeGenerator
    {
        private readonly Random _random;

        public MazeGenerator(Random? random = null)
        {
            _random = random ?? new Random();
        }

        public void Generate(int[,] grid, Point start, Point goal, double wallProbability = 0.30)
        {
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if ((r == start.X && c == start.Y) || (r == goal.X && c == goal.Y))
                    {
                        grid[r, c] = 0;
                        continue;
                    }

                    grid[r, c] = _random.NextDouble() < wallProbability ? 1 : 0;
                }
            }

            grid[start.X, start.Y] = 0;
            grid[goal.X, goal.Y] = 0;
        }
    }
}