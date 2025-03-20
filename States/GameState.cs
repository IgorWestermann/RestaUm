using RestaUm.Game;

namespace RestaUm.States;

public class GameState
{
    #region Common Properties
    public int[,] Board { get; set; }
    public int[,] State => this.Board;
    public int PathCost { get; set; }
    public int HeuristicValue { get; set; }
    public int Level { get; set; }  // Track hierarchy level
    #endregion

    #region Node-specific Properties
    public GameState Parent { get; set; }
    public List<GameState> Children { get; set; } = new List<GameState>();
    public (int x, int y, int dx, int dy) Action { get; set; }
    #endregion

    #region State-specific Properties
    public int PegCount { get; set; }
    public string StateHash { get; set; }
    #endregion

    public GameState(int[,] board, int pegCount, GameState parent, (int x, int y, int dx, int dy) action, int pathCost, string hash, int heuristicValue)
    {
        Board = (int[,])board.Clone();
        PegCount = pegCount;
        Parent = parent;
        Action = action;
        PathCost = pathCost;
        HeuristicValue = heuristicValue;
        Parent?.Children.Add(this);
        StateHash = hash;
        Level = parent?.Level + 1 ?? 0;  // Set level based on parent
    }

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