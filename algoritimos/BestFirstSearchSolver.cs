using RestaUm.helpers;
using System.Diagnostics;

namespace RestaUm.algoritimos
{
    public class BestFirstSearchSolver
    {

        public static bool Solve(int[,] initialBoard, Func<int[,], int> heuristica, string name)
        {
            var frontier = new PriorityQueue<Node, int>();
            var explored = new HashSet<string>();

            var initialState = new Node(initialBoard, null, (0, 0, 0, 0), 0, heuristica(initialBoard));

            int iteration = 0;
            int totalNodes = 0;
            var stopwatch = Stopwatch.StartNew();

            frontier.Enqueue(initialState, initialState.HeuristicValue);

            while (frontier.Count > 0)
            {
                iteration++;

                var currentNode = frontier.Dequeue();

                if (Game.IsGoalState(currentNode.State!))
                {
                    stopwatch.Stop();
                    Console.WriteLine($"\n--- {name} ---");
                    Console.WriteLine($"--- Iterations: {iteration} ---");
                    Console.WriteLine($"--- Total Nodes: {totalNodes} ---");
                    Console.WriteLine($"--- Time Elapsed: {stopwatch.ElapsedMilliseconds / 1000.0} s ---");
                    Helpers.Helpers.PrintBoard(currentNode.State);
                    return true;
                }

                string boardKey = Game.StateToString(currentNode.State);
                if (explored.Contains(boardKey))
                    continue;

                explored.Add(boardKey);

                foreach (var child in Node.GenerateChildren(currentNode, heuristica))
                {
                    totalNodes++;
                    frontier.Enqueue(child, child.HeuristicValue);
                }
            }

            stopwatch.Stop();
            Console.WriteLine($"\n--- Iterations: {iteration} ---");
            Console.WriteLine($"--- Total Nodes: {totalNodes} ---");
            Console.WriteLine($"--- Time Elapsed: {stopwatch.ElapsedMilliseconds} ms ---");
            Console.WriteLine("\nNo solution found.");

            return false;
        }
    }
}
