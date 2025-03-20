using RestaUm.helpers;

namespace RestaUm.algoritimos
{
    public class AStarSolver
    {
        public static Node Solve(int[,] initialBoard, Func<int[,], int> heuristicFunction, string name, out Node rootNode)
        {
            int pegCount = Game.CountPegs(initialBoard);
            State initialState = new State(initialBoard, pegCount, 0);
            rootNode = new Node(initialBoard, null, (-1, -1, -1, -1), 0, heuristicFunction(initialBoard));


            PriorityQueue<Node, int> openSet = new PriorityQueue<Node, int>();
            HashSet<string> closedSet = new HashSet<string>();

            openSet.Enqueue(rootNode, rootNode.PathCost + rootNode.HeuristicValue);

            int iteration = 0;
            int totalNodes = 0;
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            while (openSet.Count > 0)
            {
                iteration++;
                Node currentNode = openSet.Dequeue();
                string currentStateString = Game.StateToString(currentNode.State);

                if (closedSet.Contains(currentStateString))
                    continue;

                closedSet.Add(currentStateString);

                if (Game.IsGoalState(currentNode.State))
                {
                    stopwatch.Stop();
                    Console.WriteLine($"\n--- {name} ---");
                    Console.WriteLine($"--- Iterations: {iteration} ---");
                    Console.WriteLine($"--- Total Nodes: {totalNodes} ---");
                    Console.WriteLine($"--- Time Elapsed: {stopwatch.ElapsedMilliseconds / 1000.0} s ---");
                    Console.WriteLine($"Pontos Solução: {Helpers.Helpers.caculePoints(currentNode.State)}");
                    Game.PrintBoard(currentNode.State);

                    return currentNode;
                }

                foreach (var child in Node.GenerateChildren(currentNode, heuristicFunction))
                {
                    totalNodes++;
                    string childStateString = Game.StateToString(child.State);
                    if (!closedSet.Contains(childStateString))
                        openSet.Enqueue(child, child.HeuristicValue);
                }
            }

            stopwatch.Stop();
            Console.WriteLine($"\n--- Iterations: {iteration} ---");
            Console.WriteLine($"--- Total Nodes: {totalNodes} ---");
            Console.WriteLine($"--- Time Elapsed: {stopwatch.ElapsedMilliseconds / 1000.0} s ---");
            Console.WriteLine("\nNo solution found.");

            return null;
        }
    }
}
