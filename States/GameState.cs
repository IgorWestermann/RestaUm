using RestaUm.Game;

namespace RestaUm
{
    public class GameState
    {
        #region Common Properties
        public int[,] Board { get; set; }
        public int[,] State => this.Board;
        public int PathCost { get; set; }
        public int HeuristicValue { get; set; }
        public GameState Parent { get; set; }
        public List<GameState> Children { get; set; } = new List<GameState>();
        public (int, int, int, int) Action { get; set; }

        public int PegCount { get; set; }
        public string StateHash { get; set; }
        public GameState(int[,] board, int pegCount, GameState parent, (int, int, int, int) action, int pathCost, string hash ,int heuristicValue)
        {
            Board = (int[,])board.Clone();
            PegCount = pegCount;
            Parent = parent;
            Action = action;
            PathCost = pathCost;
            HeuristicValue = heuristicValue;
            Parent?.Children.Add(this);
            StateHash = hash;
        }
        #endregion

        #region Utility Methods
        public void UpdateHeuristic(Func<int[,], int> heuristicFunction)
        {
            HeuristicValue = heuristicFunction(Board);
        }

        public bool IsSolution()
        {
            return PegCount == 1;
        }
        #endregion
    }
}