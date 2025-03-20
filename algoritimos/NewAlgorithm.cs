namespace RestaUm.Algorithms
{
    //não faço ideia do porque os namespaces estão zuados
    using RestaUm;
    using RestaUm.Game;
    using RestaUm.Helpers;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    public class NewAlgorithm
    {
        #region Common Utility Methods

        /// <summary>
        /// Displays progress spinner during algorithm execution
        /// </summary>
        private static void ShowProgress(int iteration)
        {
            if (iteration % Config.ProgressUpdateInterval == 0)
                Console.Write("\r" + new string[] { "|", "/", "-", "\\" }[(iteration / Config.ProgressUpdateInterval) % 4] + $" Iterations: {iteration}");

            // Check for timeout or max iterations
            if (Config.MaxIterations > 0 && iteration >= Config.MaxIterations)
            {
                throw new TimeoutException($"Algorithm exceeded maximum iterations limit of {Config.MaxIterations}");
            }
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
        private static void PrintSolutionFound(string algorithmName, int iteration, GameState state)
        {
            Console.WriteLine($"\nSolution found with {algorithmName}!");
            Console.WriteLine($"--- Iterations: {iteration} ---");
            Helpers.PrintSolution(state);
        }

        #endregion

        #region Search Algorithms

        /// <summary>
        /// A* algorithm with customizable heuristic function
        /// </summary>
        public static bool AStar(
            int[,] initialBoard,
            int initialPegCount,
            Func<int[,], int> heuristicFunction = null)
        {
            // Use default heuristic from config if none provided
            heuristicFunction ??= Config.DefaultAStarHeuristic;

            var queue = new PriorityQueue<GameState, int>();
            var visited = new HashSet<string>();
            var visitedHash = new HashSet<string>();
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            var initialState = new GameState(
                board: initialBoard,
                pegCount: initialPegCount,
                parent: null,
                action: (0, 0, 0, 0),
                pathCost: 0,
                hash: Game.GenerateBoardHashes(initialBoard),
                heuristicValue: heuristicFunction(initialBoard));

            initialState.UpdateHeuristic(heuristicFunction);
            queue.Enqueue(initialState, initialState.HeuristicValue);

            int iteration = 0;

            while (queue.Count > 0)
            {
                iteration++;
                var currentState = queue.Dequeue();

                // Check for timeout
                if (Config.TimeoutMs > 0 && stopwatch.ElapsedMilliseconds > Config.TimeoutMs)
                {
                    Console.WriteLine($"\nAlgorithm timed out after {stopwatch.ElapsedMilliseconds}ms");
                    return false;
                }

                if (currentState.IsSolution())
                {
                    PrintSolutionFound("A*", iteration, currentState.Board);
                    return true;
                }

                string boardKey = Helpers.BoardToString(currentState.Board);
                string boardHash = Game.GenerateBoardHashes(currentState.Board);

                if (visited.Contains(boardKey))
                    continue;

                visited.Add(boardKey);

                if (Config.CheckHashValues && visitedHash.Contains(boardHash))
                    continue;

                visitedHash.Add(boardHash);

                // Expand all valid moves
                foreach (var (newBoard, move) in ExpandMoves(currentState.Board))
                {
                    int newPegCount = currentState.PegCount - 1;
                    string newBoardHash = Game.GenerateBoardHashes(newBoard);

                    var newState = new GameState(
                        board: newBoard,
                        pegCount: newPegCount,
                        parent: currentState,
                        action: move,
                        pathCost: currentState.PathCost + Config.DefaultMoveCost,
                        hash: newBoardHash,
                        heuristicValue: heuristicFunction(newBoard));

                    newState.UpdateHeuristic(heuristicFunction);
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
            Func<int[,], int> heuristicFunction = null)
        {
            // Use default heuristic from config if none provided
            heuristicFunction ??= Config.DefaultGreedyHeuristic;

            var frontier = new PriorityQueue<GameState, int>();
            var explored = new HashSet<string>();
            var exploredHash = new HashSet<string>();
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            var initialState = new GameState(
                board: initialBoard,
                pegCount: Heuristica.CountPegs(initialBoard),
                parent: null,
                action: (0, 0, 0, 0),
                pathCost: 0,
                hash: Game.GenerateBoardHashes(initialBoard),
                heuristicValue: heuristicFunction(initialBoard));

            frontier.Enqueue(initialState, initialState.HeuristicValue);

            int iteration = 0;

            while (frontier.Count > 0)
            {
                iteration++;
                var currentState = frontier.Dequeue();

                // Check for timeout
                if (Config.TimeoutMs > 0 && stopwatch.ElapsedMilliseconds > Config.TimeoutMs)
                {
                    Console.WriteLine($"\nAlgorithm timed out after {stopwatch.ElapsedMilliseconds}ms");
                    return false;
                }

                if (currentState.IsSolution())
                {
                    PrintSolutionFound("Best First Search", iteration, currentState);
                    return true;
                }

                string boardKey = Helpers.BoardToString(currentState.Board);
                string boardHash = Game.GenerateBoardHashes(currentState.Board);

                if (explored.Contains(boardKey))
                    continue;

                if (Config.CheckHashValues && exploredHash.Contains(boardHash))
                    continue;

                explored.Add(boardKey);
                exploredHash.Add(boardHash);

                // Expand all valid moves
                foreach (var (newBoard, move) in ExpandMoves(currentState.Board))
                {
                    int newPathCost = currentState.PathCost + Config.DefaultMoveCost;
                    int newHeuristic = heuristicFunction(newBoard);

                    var newState = new GameState(
                        board: newBoard,
                        pegCount: Heuristica.CountPegs(newBoard),
                        parent: currentState,
                        action: move,
                        pathCost: newPathCost,
                        hash: Game.GenerateBoardHashes(newBoard),
                        heuristicValue: newHeuristic);

                    frontier.Enqueue(newState, newState.HeuristicValue);
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
            double weight = 0,
            Func<int[,], int> heuristicFunction = null)
        {
            // Use default weight from config if not specified
            if (weight <= 0)
                weight = Config.DefaultWeight;

            // Use default heuristic from config if none provided
            heuristicFunction ??= Config.DefaultWeightedAStarHeuristic;

            // Create the root state
            var rootState = new GameState(
                board: initialBoard,
                pegCount: Heuristica.CountPegs(initialBoard),
                parent: null,
                action: (0, 0, 0, 0),
                pathCost: 0,
                hash: Game.GenerateBoardHashes(initialBoard),
                heuristicValue: heuristicFunction(initialBoard));

            var frontier = new PriorityQueue<GameState, double>();
            var explored = new HashSet<string>();
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            // f(n) = g(n) + weight * h(n)
            frontier.Enqueue(rootState, rootState.PathCost + weight * rootState.HeuristicValue);

            int iteration = 0;
            while (frontier.Count > 0)
            {
                iteration++;
                var currentState = frontier.Dequeue();

                // Check for timeout
                if (Config.TimeoutMs > 0 && stopwatch.ElapsedMilliseconds > Config.TimeoutMs)
                {
                    Console.WriteLine($"\nAlgorithm timed out after {stopwatch.ElapsedMilliseconds}ms");
                    return false;
                }

                if (currentState.IsSolution())
                {
                    PrintSolutionFound("A* Weighted", iteration, currentState);
                    return true;
                }

                string boardKey = Helpers.BoardToString(currentState.Board);
                if (explored.Contains(boardKey))
                    continue;

                explored.Add(boardKey);

                // Expand all valid moves
                foreach (var (newBoard, move) in ExpandMoves(currentState.Board))
                {
                    int newPathCost = currentState.PathCost + Config.DefaultMoveCost;
                    int newHeuristic = heuristicFunction(newBoard);

                    var newState = new GameState(
                        board: newBoard,
                        pegCount: Heuristica.CountPegs(newBoard),
                        parent: currentState,
                        action: move,
                        pathCost: newPathCost,
                        hash: Game.GenerateBoardHashes(newBoard),
                        heuristicValue: newHeuristic);

                    // f(n) = g(n) + weight * h(n)
                    frontier.Enqueue(newState, newState.PathCost + weight * newState.HeuristicValue);
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
            // Use default heuristics from config if none provided
            primaryHeuristic ??= Config.DefaultPrimaryHeuristic;
            secondaryHeuristic ??= Config.DefaultSecondaryHeuristic;

            // Calculate the initial heuristics
            int initialPrimaryValue = primaryHeuristic(initialBoard);
            int initialSecondaryValue = secondaryHeuristic(initialBoard);

            // Create the root state
            var rootState = new GameState(
                board: initialBoard,
                pegCount: Heuristica.CountPegs(initialBoard),
                parent: null,
                action: (0, 0, 0, 0),
                pathCost: 0,
                hash: Game.GenerateBoardHashes(initialBoard),
                heuristicValue: initialPrimaryValue);

            // Priority queue with tuple priority: (primary, secondary, cost)
            var frontier = new PriorityQueue<GameState, (int, int, int)>();
            var explored = new HashSet<string>();
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            frontier.Enqueue(rootState, (initialPrimaryValue, initialSecondaryValue, rootState.PathCost));

            int iteration = 0;
            while (frontier.Count > 0)
            {
                iteration++;
                var currentState = frontier.Dequeue();

                // Check for timeout
                if (Config.TimeoutMs > 0 && stopwatch.ElapsedMilliseconds > Config.TimeoutMs)
                {
                    Console.WriteLine($"\nAlgorithm timed out after {stopwatch.ElapsedMilliseconds}ms");
                    return false;
                }

                if (currentState.IsSolution())
                {
                    PrintSolutionFound("Ordered Search", iteration, currentState);
                    return true;
                }

                string boardKey = Helpers.BoardToString(currentState.Board);
                if (explored.Contains(boardKey))
                    continue;

                explored.Add(boardKey);

                // Expand all valid moves
                foreach (var (newBoard, move) in ExpandMoves(currentState.Board))
                {
                    int newPathCost = currentState.PathCost + Config.DefaultMoveCost;
                    int newPrimaryValue = primaryHeuristic(newBoard);
                    int newSecondaryValue = secondaryHeuristic(newBoard);

                    var newState = new GameState(
                        board: newBoard,
                        pegCount: Heuristica.CountPegs(newBoard),
                        parent: currentState,
                        action: move,
                        pathCost: newPathCost,
                        hash: Game.GenerateBoardHashes(newBoard),
                        heuristicValue: newPrimaryValue);

                    currentState.Children.Add(newState);

                    frontier.Enqueue(newState, (newPrimaryValue, newSecondaryValue, newPathCost));
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
            var queue = new Queue<GameState>();
            var visited = new HashSet<string>();
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            var initialState = new GameState(
                board: initialBoard,
                pegCount: initialPegCount,
                parent: null,
                action: (0, 0, 0, 0),
                pathCost: 0,
                hash: Game.GenerateBoardHashes(initialBoard),
                heuristicValue: 0);

            string initialKey = Helpers.BoardToString(initialBoard);

            visited.Add(initialKey);
            queue.Enqueue(initialState);

            int iteration = 0;

            while (queue.Count > 0)
            {
                iteration++;
                var currentState = queue.Dequeue();

                // Check for timeout
                if (Config.TimeoutMs > 0 && stopwatch.ElapsedMilliseconds > Config.TimeoutMs)
                {
                    Console.WriteLine($"\nAlgorithm timed out after {stopwatch.ElapsedMilliseconds}ms");
                    return false;
                }

                if (currentState.IsSolution())
                {
                    PrintSolutionFound("BFS", iteration, currentState.Board);
                    return true;
                }

                // Expand all valid moves
                foreach (var (newBoard, move) in ExpandMoves(currentState.Board))
                {
                    int newPegCount = currentState.PegCount - 1;
                    string boardKey = Helpers.BoardToString(newBoard);

                    // Only enqueue states we haven't seen before
                    if (!visited.Contains(boardKey))
                    {
                        visited.Add(boardKey);
                        var newState = new GameState(
                            board: newBoard,
                            pegCount: newPegCount,
                            parent: currentState,
                            action: move,
                            pathCost: currentState.PathCost + Config.DefaultMoveCost,
                            hash: Game.GenerateBoardHashes(newBoard),
                            heuristicValue: 0);
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
            var stack = new Stack<GameState>();
            var visited = new HashSet<string>();
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            var initialState = new GameState(
                board: initialBoard,
                pegCount: initialPegCount,
                parent: null,
                action: (0, 0, 0, 0),
                pathCost: 0,
                hash: Game.GenerateBoardHashes(initialBoard),
                heuristicValue: 0);

            stack.Push(initialState);

            int iteration = 0;

            while (stack.Count > 0)
            {
                iteration++;
                var currentState = stack.Pop();

                // Check for timeout
                if (Config.TimeoutMs > 0 && stopwatch.ElapsedMilliseconds > Config.TimeoutMs)
                {
                    Console.WriteLine($"\nAlgorithm timed out after {stopwatch.ElapsedMilliseconds}ms");
                    return false;
                }

                if (currentState.IsSolution())
                {
                    PrintSolutionFound("DFS", iteration, currentState.Board);
                    return true;
                }

                string boardKey = Helpers.BoardToString(currentState.Board);
                if (visited.Contains(boardKey))
                    continue;

                visited.Add(boardKey);

                // Expand all valid moves
                foreach (var (newBoard, move) in ExpandMoves(currentState.Board))
                {
                    int newPegCount = currentState.PegCount - 1;
                    var newState = new GameState(
                        board: newBoard,
                        pegCount: newPegCount,
                        parent: currentState,
                        action: move,
                        pathCost: currentState.PathCost + Config.DefaultMoveCost,
                        hash: Game.GenerateBoardHashes(newBoard),
                        heuristicValue: 0);
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
        private static Stopwatch backtrackingStopwatch = new Stopwatch();

        public static bool SolveBacktracking(int[,] initialBoard, int initialPegCount)
        {
            backtrackingIterationCount = 0;
            backtrackingStopwatch.Restart();
            var visited = new HashSet<string>();
            bool result = BacktrackingSearch(initialBoard, initialPegCount, visited);

            if (!result)
                Console.WriteLine($"\n--- Iterations: {backtrackingIterationCount} ---");

            return result;
        }

        private static bool BacktrackingSearch(int[,] board, int pegCount, HashSet<string> visited)
        {
            backtrackingIterationCount++;

            // Check for timeout
            if (Config.TimeoutMs > 0 && backtrackingStopwatch.ElapsedMilliseconds > Config.TimeoutMs)
            {
                Console.WriteLine($"\nAlgorithm timed out after {backtrackingStopwatch.ElapsedMilliseconds}ms");
                return false;
            }

            // Check for iteration limit
            if (Config.MaxIterations > 0 && backtrackingIterationCount > Config.MaxIterations)
            {
                Console.WriteLine($"\nAlgorithm exceeded maximum iterations limit of {Config.MaxIterations}");
                return false;
            }

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

                if (backtrackingIterationCount % Config.ProgressUpdateInterval == 0)
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