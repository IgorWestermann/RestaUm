using RestaUm.helpers;

namespace RestaUm
{
    public class Node
    {
        public int[,] State { get; set; }
        public Node Parent { get; set; }
        public List<Node> Children { get; set; } = new List<Node>();
        public (int, int, int, int) Action { get; set; }
        public int PathCost { get; set; }
        public int HeuristicValue { get; set; }

        public Node(int[,] state, Node parent, (int, int, int, int) action, int pathCost, int heuristicValue)
        {
            State = (int[,])state.Clone();
            Parent = parent;
            Action = action;
            PathCost = pathCost;
            HeuristicValue = heuristicValue;
            // Se existir pai, adiciona este nó à lista de filhos do pai
            Parent?.Children.Add(this);
        }

        public static List<Node> GenerateChildren(Node parent, Func<int[,], int> heuristicFunction)
        {
            int[] dx = { -1, 1, 0, 0 };
            int[] dy = { 0, 0, -1, 1 };
            List<Node> children = new List<Node>();
            int[,] state = parent.State;

            for (int i = 0; i < state.GetLength(0); i++)
            {
                for (int j = 0; j < state.GetLength(1); j++)
                {
                    if (state[i, j] == 1)
                    {
                        for (int k = 0; k < 4; k++)
                        {
                            (int dx1, int dy1) = (dx[k], dy[k]);
                            int x = i + dx1;
                            int y = j + dy1;
                            int x2 = i + 2 * dx1;
                            int y2 = j + 2 * dy1;

                            if (Game.IsValidMove(state, i, j, x, y, x2, y2))
                            {
                                int[,] newState = (int[,])state.Clone();
                                newState[i, j] = 0;
                                newState[x, y] = 0;
                                newState[x2, y2] = 1;

                                int newPathCost = parent.PathCost + 1;
                                int newHeuristicValue = newPathCost + heuristicFunction(newState);

                                Node child = new Node(newState, parent, (i, j, x2, y2), newPathCost, newHeuristicValue);
                                children.Add(child);
                            }
                        }
                    }
                }
            }

            return children;
        }
    }
}