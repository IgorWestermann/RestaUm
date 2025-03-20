

namespace RestaUm.Algorithms
{
    //não faço ideia do porque os namespaces estão zuados
    using RestaUm;
    using RestaUm.Game;
    using RestaUm.Helpers;
    using System;
    using System.Collections.Generic;
    public class NewAlgorithm
    {
        #region Common Utility Methods

        /// <summary>
        /// Displays progress spinner during algorithm execution
        /// </summary>
        private static void ShowProgress(int iteration)
        {
            if (iteration % 500 == 0)
                Console.Write("\r" + new string[] { "|", "/", "-", "\\" }[(iteration / 500) % 4] + $" Iterations: {iteration}");
        }

        /// <summary>
        /// Checks if the current state represents a solution
        /// </summary>
        private static bool IsSolution(int[,] board)
        {
            return Heuristica.CountPegs(board) == 1;
        }

        /// <summary>
        /// Common method to expand all valid moves from a board state
        /// </summary>
        private static List<(int[,] board, (int x, int y, int dx, int dy) move)> ExpandMoves(int[,] board)
        {
            var moves = new List<(int[,], (int, int, int, int))>();

            for (int x = 0; x < 7; x++)
            {
                for (int y = 0; y < 7; y++)
                {
                    foreach (var (dx, dy) in Game.directions)
                    {
                        if (Game.IsValidMove(board, x, y, dx, dy))
                        {
                            var newBoard = (int[,])board.Clone();
                            Game.MakeMove(newBoard, x, y, dx, dy);
                            moves.Add((newBoard, (x, y, dx, dy)));
                        }
                    }
                }
            }

            return moves;
        }

        /// <summary>
        /// Print solution found message with iteration count
        /// </summary>
        private static void PrintSolutionFound(string algorithmName, int iteration, int[,] board = null)
        {
            Console.WriteLine($"\nSolution found with {algorithmName}!");
            Console.WriteLine($"--- Iterations: {iteration} ---");

            if (board != null)
                Helpers.PrintBoard(board);
        }

        /// <summary>
        /// Print solution found message with iteration count and node path
        /// </summary>
        private static void PrintSolutionFound(string algorithmName, int iteration, Node node)
        {
            Console.WriteLine($"\nSolution found with {algorithmName}!");
            Console.WriteLine($"--- Iterations: {iteration} ---");
            //Helpers.PrintSolution(node);
        }

        #endregion

        #region Search Algorithms

        /// <summary>
        /// A* algorithm with customizable heuristic function
        /// </summary>
        public static bool AStar(
            int[,] initialBoard,
            int initialPegCount,
            bool checkForHashValues = false,
            Func<int[,], int> heuristicFunction = null)
        {
            // Use CountPegs as default heuristic if none provided
            heuristicFunction ??= Heuristica.CountPegs;

            var queue = new PriorityQueue<State, int>();
            var visited = new HashSet<string>();
            var visitedHash = new HashSet<string>();

            var initialState = new State(
                initialBoard,
                initialPegCount,
                0,
                Game.GenerateBoardHashes(initialBoard));

            queue.Enqueue(initialState, initialState.HeuristicValue);

            int iteration = 0;

            while (queue.Count > 0)
            {
                iteration++;
                var currentState = queue.Dequeue();

                if (currentState.PegCount == 1)
                {
                    PrintSolutionFound("A*", iteration, currentState.Board);
                    return true;
                }

                string boardKey = Helpers.BoardToString(currentState.Board);
                string boardHash = Game.GenerateBoardHashes(currentState.Board);

                if (visited.Contains(boardKey))
                    continue;

                visited.Add(boardKey);

                if (checkForHashValues && visitedHash.Contains(boardHash))
                    continue;

                visitedHash.Add(boardHash);

                // Expand all valid moves
                foreach (var (newBoard, _) in ExpandMoves(currentState.Board))
                {
                    int newPegCount = currentState.PegCount - 1;
                    string newBoardHash = Game.GenerateBoardHashes(newBoard);

                    var newState = new State(
                        newBoard,
                        newPegCount,
                        currentState.MovesSoFar + 1,
                        newBoardHash);

                    queue.Enqueue(newState, newState.HeuristicValue);
                }

                ShowProgress(iteration);
            }

            Console.WriteLine("\nNo solution found.");
            return false;
        }

        /// <summary>
        /// Best First Search (Greedy) with configurable heuristic
        /// </summary>
        public static bool BestFirstSearch(
            int[,] initialBoard,
            bool checkForHashValues = false,
            Func<int[,], int> heuristicFunction = null)
        {
            // Use CountPegs as default heuristic if none provided
            heuristicFunction ??= Heuristica.CountPegs;

            var frontier = new PriorityQueue<Node, int>();
            var explored = new HashSet<string>();
            var exploredHash = new HashSet<string>();

            var initialNode = new Node(
                initialBoard,
                null,
                (0, 0, 0, 0),
                0,
                heuristicFunction(initialBoard));

            frontier.Enqueue(initialNode, initialNode.HeuristicValue);

            int iteration = 0;

            while (frontier.Count > 0)
            {
                iteration++;
                var currentNode = frontier.Dequeue();

                if (IsSolution(currentNode.State))
                {
                    PrintSolutionFound("Best First Search", iteration, currentNode);
                    return true;
                }

                string boardKey = Helpers.BoardToString(currentNode.State);
                string boardHash = Game.GenerateBoardHashes(currentNode.State);

                if (explored.Contains(boardKey))
                    continue;

                if (checkForHashValues && exploredHash.Contains(boardHash))
                    continue;

                explored.Add(boardKey);
                exploredHash.Add(boardHash);

                // Expand all valid moves
                foreach (var (newBoard, move) in ExpandMoves(currentNode.State))
                {
                    int newPathCost = currentNode.PathCost + 1;
                    var newNode = new Node(
                        newBoard,
                        currentNode,
                        move,
                        newPathCost,
                        heuristicFunction(newBoard));

                    frontier.Enqueue(newNode, newNode.HeuristicValue);
                }

                ShowProgress(iteration);
            }

            Console.WriteLine("\nNo solution found.");
            return false;
        }

        /// <summary>
        /// A* algorithm with weighted heuristic
        /// </summary>
        public static bool AStarWeightedHeuristic(
            int[,] initialBoard,
            double weight = 1.0,
            Func<int[,], int> heuristicFunction = null)
        {
            // Use Centrality as default heuristic if none provided
            heuristicFunction ??= Heuristica.Centrality;

            // Create the root node
            var rootNode = new Node(
                initialBoard,
                null,
                (0, 0, 0, 0),
                0,
                heuristicFunction(initialBoard));

            var frontier = new PriorityQueue<Node, double>();
            var explored = new HashSet<string>();

            // f(n) = g(n) + weight * h(n)
            frontier.Enqueue(rootNode, rootNode.PathCost + weight * rootNode.HeuristicValue);

            int iteration = 0;
            while (frontier.Count > 0)
            {
                iteration++;
                var currentNode = frontier.Dequeue();

                if (IsSolution(currentNode.State))
                {
                    PrintSolutionFound("A* Weighted", iteration, currentNode);
                    return true;
                }

                string boardKey = Helpers.BoardToString(currentNode.State);
                if (explored.Contains(boardKey))
                    continue;

                explored.Add(boardKey);

                // Expand all valid moves
                foreach (var (newBoard, move) in ExpandMoves(currentNode.State))
                {
                    int newPathCost = currentNode.PathCost + 1;
                    int newHeuristic = heuristicFunction(newBoard);

                    var newNode = new Node(
                        newBoard,
                        currentNode,
                        move,
                        newPathCost,
                        newHeuristic);

                    // f(n) = g(n) + weight * h(n)
                    frontier.Enqueue(newNode, newNode.PathCost + weight * newNode.HeuristicValue);
                }

                ShowProgress(iteration);
            }

            Console.WriteLine("\nNo solution found.");
            return false;
        }

        /// <summary>
        /// Ordered Search with multiple criteria for tiebreaking
        /// </summary>
        public static bool OrderedSearch(
            int[,] initialBoard,
            Func<int[,], int> primaryHeuristic = null,
            Func<int[,], int> secondaryHeuristic = null)
        {
            // Use Centrality as primary heuristic if none provided
            primaryHeuristic ??= Heuristica.Centrality;

            // Use FutureMobility as secondary heuristic if none provided
            secondaryHeuristic ??= Helpers.FutureMobility;

            // Calculate the initial heuristics
            int initialPrimaryValue = primaryHeuristic(initialBoard);
            int initialSecondaryValue = secondaryHeuristic(initialBoard);

            // Create the root node
            var rootNode = new Node(
                initialBoard,
                null,
                (0, 0, 0, 0),
                0,
                initialPrimaryValue);

            // Priority queue with tuple priority: (primary, secondary, cost)
            var frontier = new PriorityQueue<Node, (int, int, int)>();
            var explored = new HashSet<string>();

            frontier.Enqueue(rootNode, (initialPrimaryValue, initialSecondaryValue, rootNode.PathCost));

            int iteration = 0;
            while (frontier.Count > 0)
            {
                iteration++;
                var currentNode = frontier.Dequeue();

                if (IsSolution(currentNode.State))
                {
                    PrintSolutionFound("Ordered Search", iteration, currentNode);
                    return true;
                }

                string boardKey = Helpers.BoardToString(currentNode.State);
                if (explored.Contains(boardKey))
                    continue;

                explored.Add(boardKey);

                // Expand all valid moves
                foreach (var (newBoard, move) in ExpandMoves(currentNode.State))
                {
                    int newPathCost = currentNode.PathCost + 1;
                    int newPrimaryValue = primaryHeuristic(newBoard);
                    int newSecondaryValue = secondaryHeuristic(newBoard);

                    var newNode = new Node(
                        newBoard,
                        currentNode,
                        move,
                        newPathCost,
                        newPrimaryValue);

                    currentNode.Children.Add(newNode);

                    frontier.Enqueue(newNode, (newPrimaryValue, newSecondaryValue, newPathCost));
                }

                ShowProgress(iteration);
            }

            Console.WriteLine("\nNo solution found.");
            return false;
        }

        /// <summary>
        /// Breadth First Search algorithm
        /// </summary>
        public static bool BreadthFirstSearch(int[,] initialBoard, int initialPegCount)
        {
            var queue = new Queue<State>();
            var visited = new HashSet<string>();

            var initialState = new State(initialBoard, initialPegCount, 0);
            string initialKey = Helpers.BoardToString(initialBoard);

            visited.Add(initialKey);
            queue.Enqueue(initialState);

            int iteration = 0;

            while (queue.Count > 0)
            {
                iteration++;
                var currentState = queue.Dequeue();

                if (currentState.PegCount == 1)
                {
                    PrintSolutionFound("BFS", iteration, currentState.Board);
                    return true;
                }

                // Expand all valid moves
                foreach (var (newBoard, _) in ExpandMoves(currentState.Board))
                {
                    int newPegCount = currentState.PegCount - 1;
                    string boardKey = Helpers.BoardToString(newBoard);

                    // Only enqueue states we haven't seen before
                    if (!visited.Contains(boardKey))
                    {
                        visited.Add(boardKey);
                        var newState = new State(newBoard, newPegCount, currentState.MovesSoFar + 1);
                        queue.Enqueue(newState);
                    }
                }

                ShowProgress(iteration);
            }

            Console.WriteLine("\nNo solution found.");
            return false;
        }

        /// <summary>
        /// Depth First Search algorithm
        /// </summary>
        public static bool DepthFirstSearch(int[,] initialBoard, int initialPegCount)
        {
            var stack = new Stack<State>();
            var visited = new HashSet<string>();

            var initialState = new State(initialBoard, initialPegCount, 0);
            stack.Push(initialState);

            int iteration = 0;

            while (stack.Count > 0)
            {
                iteration++;
                var currentState = stack.Pop();

                if (currentState.PegCount == 1)
                {
                    PrintSolutionFound("DFS", iteration, currentState.Board);
                    return true;
                }

                string boardKey = Helpers.BoardToString(currentState.Board);
                if (visited.Contains(boardKey))
                    continue;

                visited.Add(boardKey);

                // Expand all valid moves
                foreach (var (newBoard, _) in ExpandMoves(currentState.Board))
                {
                    int newPegCount = currentState.PegCount - 1;
                    var newState = new State(newBoard, newPegCount, currentState.MovesSoFar + 1);
                    stack.Push(newState);
                }

                ShowProgress(iteration);
            }

            Console.WriteLine("\nNo solution found.");
            return false;
        }

        /// <summary>
        /// Recursive Backtracking Search algorithm
        /// </summary>
        private static int backtrackingIterationCount = 0;

        public static bool SolveBacktracking(int[,] initialBoard, int initialPegCount)
        {
            backtrackingIterationCount = 0;
            var visited = new HashSet<string>();
            bool result = BacktrackingSearch(initialBoard, initialPegCount, visited);

            if (!result)
                Console.WriteLine($"\n--- Iterations: {backtrackingIterationCount} ---");

            return result;
        }

        private static bool BacktrackingSearch(int[,] board, int pegCount, HashSet<string> visited)
        {
            backtrackingIterationCount++;

            if (pegCount == 1)
            {
                PrintSolutionFound("Backtracking", backtrackingIterationCount, board);
                return true;
            }

            string boardKey = Helpers.BoardToString(board);
            if (visited.Contains(boardKey))
                return false;

            visited.Add(boardKey);

            // Expand all valid moves recursively
            foreach (var (newBoard, _) in ExpandMoves(board))
            {
                if (BacktrackingSearch(newBoard, pegCount - 1, visited))
                    return true;

                if (backtrackingIterationCount % 10 == 0)
                    ShowProgress(backtrackingIterationCount);
            }

            return false;
        }

        /// <summary>
        /// Optimized Breadth First Search using bitmasks
        /// </summary>
        public static bool OptimizedBFSBitmask(int[,] initialBoard, int initialPegCount)
        {
            return PegSolitaireOptimized.OptimizedBFSBitmask(initialBoard, initialPegCount);
        }

        #endregion
    }
}