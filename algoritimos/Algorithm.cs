using RestaUm;
using RestaUm.helpers;
using RestaUm.Helpers;

public class Algorithm
{
    public static bool AStar(int[,] initialBoard, int initialPegCount)
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
                Console.WriteLine($"\n--- Iterations: {iteration} ---");
                Console.WriteLine("\nSolution found!");
                Helpers.PrintBoard(currentState.Board);

                return true;
            }

            string boardKey = Helpers.BoardToString(currentState.Board);

            if (visited.Contains(boardKey))
            {
                continue;
            }

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

    public static bool BestFirstSearch(int[,] initialBoard)
    {
        var frontier = new PriorityQueue<Node, int>();

        var explored = new HashSet<string>();

        var initialState = new Node(initialBoard, null, (0, 0, 0, 0), 0, Heuristica.CountPegs(initialBoard));

        int iteration = 0;

        frontier.Enqueue(initialState, initialState.HeuristicValue);

        while (frontier.Count > 0)
        {
            iteration++;

            var currentNode = frontier.Dequeue();

            if (Heuristica.CountPegs(currentNode.State!) == 1)
            {
                Console.WriteLine("Solution found!");
                Console.WriteLine($"\n--- Iterations: {iteration} ---");
                Helpers.PrintSolution(currentNode);
                return true;
            }

            string boardKey = Helpers.BoardToString(currentNode.State);
            if (explored.Contains(boardKey))
                continue;

            explored.Add(boardKey);

            for (int x = 0; x < 7; x++)
            {
                for (int y = 0; y < 7; y++)
                {
                    foreach (var (dx, dy) in Game.directions)
                    {
                        if (Game.IsValidMove(currentNode.State, x, y, dx, dy))
                        {
                            var newBoard = (int[,])currentNode.State.Clone();
                            Game.MakeMove(newBoard, x, y, dx, dy);

                            int newPathCost = currentNode.PathCost + 2;

                            var newNode = new Node(newBoard, currentNode, (x, y, dx, dy), newPathCost, Heuristica.CountPegs(newBoard));

                            frontier.Enqueue(newNode, newNode.HeuristicValue);
                        }
                    }
                }
            }
        }

        Console.WriteLine("No solution found.");
        return false;
    }
}