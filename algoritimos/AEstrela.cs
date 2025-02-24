using RestaUm;
using RestaUm.helpers;
using RestaUm.Helpers;

public class AEstrela
{
    public static bool Solve(int[,] initialBoard, int initialPegCount)
    {
        var queue = new PriorityQueue<State, int>();

        var visited = new HashSet<string>();

        var initialState = new State(initialBoard, initialPegCount, 0);
        queue.Enqueue(initialState, initialState.HeuristicValue);

        int iteration = 0;

        while (queue.Count > 0)
        {
            iteration++;
            var currentState = queue.Dequeue();

            if (currentState.PegCount == 1)
            {
                Console.WriteLine("\nSolution found!");
                Helpers.PrintBoard(currentState.Board);
                Console.WriteLine($"\n--- Iteration {iteration} ---");

                return true;
            }

            string boardKey = Helpers.BoardToString(currentState.Board);
            if (visited.Contains(boardKey))
                continue;

            visited.Add(boardKey);

            for (int x = 0; x < 7; x++)
            {
                for (int y = 0; y < 7; y++)
                {
                    foreach (var (dx, dy) in Game.directions)
                    {
                        if (Game.IsValidMove(currentState.Board, x, y, dx, dy))
                        {
                            var newBoard = (int[,])currentState.Board.Clone();
                            Game.MakeMove(newBoard, x, y, dx, dy);

                            int newPegCount = currentState.PegCount - 1;

                            var newState = new State(newBoard, newPegCount, currentState.MovesSoFar + 1);

                            queue.Enqueue(newState, newState.HeuristicValue);
                        }
                    }
                }
            }
        }

        Console.WriteLine("No solution found.");
        return false;
    }
}