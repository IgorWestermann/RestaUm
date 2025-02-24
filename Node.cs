namespace RestaUm
{
    public class Node
    {
        public int[,] Board { get; set; }
        public int PegCount { get; set; }
        public int MovesSoFar { get; set; }
        public int HeuristicValue { get; set; }
        public List<Node> Path { get; set; }

        public Node(int[,] board, int pegCount, int movesSoFar, List<Node> path)
        {
            Board = (int[,])board.Clone();
            PegCount = pegCount;
            MovesSoFar = movesSoFar;
            HeuristicValue = movesSoFar + Heuristic(pegCount); // f(n) = g(n) + h(n)
            Path = new List<Node>(path);
            Path.Add(this);
        }

        private int Heuristic(int pegCount)
        {
            return pegCount;
        }
    }
}