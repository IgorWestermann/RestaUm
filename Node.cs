namespace RestaUm
{
    public class Node(int[,] state, Node parent, (int, int, int, int) action, int pathCost, int heuristicValue)
    {
        public int[,] State { get; set; } = (int[,])state.Clone();
        public Node Parent { get; set; } = parent;
        public (int, int, int, int) Action { get; set; } = action;
        public int PathCost { get; set; } = pathCost;
        public int HeuristicValue { get; set; } = heuristicValue;
    }
}