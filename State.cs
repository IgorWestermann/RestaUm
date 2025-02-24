namespace RestaUm;
public class State
{
    public int[,] Board { get; set; }
    public int PegCount { get; set; }
    public int MovesSoFar { get; set; } // Number of moves taken to reach this state
    public int HeuristicValue { get; set; } // Heuristic value for A*

    public State(int[,] board, int pegCount, int movesSoFar)
    {
        Board = (int[,])board.Clone();
        PegCount = pegCount;
        MovesSoFar = movesSoFar;
        HeuristicValue = movesSoFar + Heuristic(pegCount); // f(n) = g(n) + h(n)
    }

    // Heuristic function
    private int Heuristic(int pegCount)
    {
        return pegCount; // Number of remaining pegs
    }
}