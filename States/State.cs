using RestaUm.Game;

namespace RestaUm
{
    public class State
    {

        public int[,] Board { get; set; }
        public int PegCount { get; set; }
        public int MovesSoFar { get; set; } // Number of moves taken to reach this state
        public int HeuristicValue { get; set; } // Heuristic value for A*

        public string Hash { get; set; }

        public State(int[,] board, int pegCount, int movesSoFar, string hash = default(string))
        {
            Board = (int[,])board.Clone();
            PegCount = pegCount;
            MovesSoFar = movesSoFar;
            HeuristicValue = movesSoFar + Heuristic(pegCount); // f(n) = g(n) + h(n)
            Hash = hash;
        }

        // Heuristic function
        private int Heuristic(int pegCount)
        {
            return pegCount; // Number of remaining pegs
        }
    }
}
