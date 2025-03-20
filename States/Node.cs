namespace RestaUm
{
        public class Node {
            public int[,] State { get; set; }
            public Node Parent { get; set; }
            public List<Node> Children { get; set; } = new List<Node>(); 
            public (int, int, int, int) Action { get; set; }
            public int PathCost { get; set; }
            public int HeuristicValue { get; set; }

            public Node(int[,] state, Node parent, (int, int, int, int) action, int pathCost, int heuristicValue) {
                State = (int[,])state.Clone();
                Parent = parent;
                Action = action;
                PathCost = pathCost;
                HeuristicValue = heuristicValue;
                // Se existir pai, adiciona este nó à lista de filhos do pai
                Parent?.Children.Add(this);
            }
        }
}