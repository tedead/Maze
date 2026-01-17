namespace Maze
{
    public sealed class Node
    {
        public int X { get; }
        public int Y { get; }
        public int G { get; set; } = int.MaxValue;
        public int H { get; set; }
        public int F => G + H;
        public Node? Parent { get; set; }

        public Node(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object? obj)
        {
            if (obj is Node n)
            {
                return n.X == X && n.Y == Y;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
    }
}
