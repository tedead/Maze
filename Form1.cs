using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Maze
{
    public partial class Form1 : Form
    {
        private int _delayMs = 0;
        const int CellSize = 15;
        const int Rows = 40;
        const int Cols = 40;
        int[,] grid = new int[Rows, Cols];
        List<Node> path = new();
        Point start = new(0, 0);
        Point goal = new(Rows - 1, Cols - 1);
        private readonly MazeGenerator _mazeGenerator = new();

        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;
            Width = Cols * CellSize + 200;
            Height = Rows * CellSize + 50;

            Label lblDelay = new Label
            {
                Location = new Point(Cols * CellSize + 20, 150),
                AutoSize = true,
                Text = "Delay: 0 ms"
            };

            Controls.Add(lblDelay);

            TrackBar speedBar = new TrackBar
            {
                Minimum = 0,          // Fast
                Maximum = 200,        // Slow
                Value = 0,           // Default delay (ms)
                TickFrequency = 20,
                SmallChange = 1,
                LargeChange = 20,
                Width = 120,
                Location = new Point(Cols * CellSize + 20, 120)
            };

            speedBar.ValueChanged += (_, _) =>
            {
                _delayMs = speedBar.Value;
                lblDelay.Text = $"Delay: {_delayMs} ms";
            };

            Controls.Add(speedBar);

            var genBtn = new Button
            {
                Text = "Generate Maze",
                Location = new Point(Cols * CellSize + 20, 60),
                Width = 120
            };

            genBtn.Click += (_, _) => GenerateMaze();
            Controls.Add(genBtn);

            var btn = new Button
            {
                Text = "Solve (A*)",
                Location = new Point(Cols * CellSize + 20, 20),
                Width = 120
            };

            btn.Click += async (_, _) =>
            {
                path.Clear();
                Invalidate();

                var finalPath = await AStar.FindPathAsync(grid,
                                                          start,
                                                          goal,
                                                          (p, _) =>
                                                          {
                                                            path = p;
                                                            Invalidate();
                                                          },
                                                          delayMs: _delayMs
                                                         );

                path = finalPath;
                Invalidate();

                if (path.Count > 0)
                {
                    int steps = path.Count - 1;

                    MessageBox.Show($"Finished in {steps} step{(steps == 1 ? "" : "s")}.", "A* Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No path found.", "A* Result", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            Controls.Add(btn);

            MouseClick += Form1_MouseClick;
        }

        private void GenerateMaze()
        {
            _mazeGenerator.Generate(grid, start, goal, wallProbability: 0.30);

            path.Clear();
            Invalidate();
        }

        private void Form1_MouseClick(object? sender, MouseEventArgs e)
        {
            int x = e.Y / CellSize;
            int y = e.X / CellSize;

            if (x < 0 || y < 0 || x >= Rows || y >= Cols)
            {
                return;
            }

            if (new Point(x, y) == start || new Point(x, y) == goal)
            {
                return;
            }

            grid[x, y] = grid[x, y] == 0 ? 1 : 0;
            path.Clear();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;

            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    var rect = new Rectangle(c * CellSize, r * CellSize, CellSize, CellSize);

                    Brush brush = grid[r, c] == 1 ? Brushes.Black : Brushes.White;
                    g.FillRectangle(brush, rect);
                    g.DrawRectangle(Pens.Gray, rect);
                }
            }

            foreach (var n in path)
            {
                g.FillRectangle(Brushes.LightGreen, n.Y * CellSize, n.X * CellSize, CellSize, CellSize);
            }

            g.FillRectangle(Brushes.Blue, start.Y * CellSize, start.X * CellSize, CellSize, CellSize);
            g.FillRectangle(Brushes.Red, goal.Y * CellSize, goal.X * CellSize, CellSize, CellSize);
        }
    }
}
