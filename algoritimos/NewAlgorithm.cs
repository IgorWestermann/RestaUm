namespace RestaUm.Algorithms
{
    //não faço ideia do porque os namespaces estão zuados
    using RestaUm;
    using RestaUm.Game;
    using RestaUm.Helpers;
    using RestaUm.States;
    using RestaUm.Performance;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using RestaUm.Visualization;

    public class NewAlgorithm
    {
        #region Common Utility Methods

        /// <summary>
        /// Displays progress spinner during algorithm execution
        /// </summary>
        private static void ShowProgress(int iteration)
        {
            //if (iteration % Config.ProgressUpdateInterval == 0)
            //    Console.Write("\r" + new string[] { "|", "/", "-", "\\" }[(iteration / Config.ProgressUpdateInterval) % 4] + $" Iterations: {iteration}");

            //// Check for timeout or max iterations
            //if (Config.MaxIterations > 0 && iteration >= Config.MaxIterations)
            //{
            //    throw new TimeoutException($"Algorithm exceeded maximum iterations limit of {Config.MaxIterations}");
            //}
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
        /// Print solution found message with iteration count and diagnostics
        /// </summary>
        private static void PrintSolutionFound(string algorithmName, int iteration, Stopwatch stopwatch, GameState state = null)
        {
            Console.WriteLine($"\nSolution found with {algorithmName}!");
            Console.WriteLine($"--- Iterations: {iteration} ---");
            Console.WriteLine($"--- Time Elapsed: {stopwatch.ElapsedMilliseconds}ms ---");

            if (state != null)
            {
                Console.WriteLine($"--- Solution Level: {state.Level} ---");
                Console.WriteLine($"--- Path Cost: {state.PathCost} ---");
                Console.WriteLine($"--- Final Heuristic Value: {state.HeuristicValue} ---");
                Helpers.PrintSolution(state, algorithmName);
            }
            else if (state?.Board != null)
            {
                Helpers.PrintBoard(state.Board);
            }
            Helpers.PrintBoard(state.Board);
        }

        // /// <summary>
        // /// Print solution found message with iteration count
        // /// </summary>
        // private static void PrintSolutionFound(string algorithmName, int iteration, int[,] board = null)
        // {
        //     Console.WriteLine($"\nSolution found with {algorithmName}!");
        //     Console.WriteLine($"--- Iterations: {iteration} ---");

        //     if (board != null)
        //         Helpers.PrintBoard(board);
        // }

        // /// <summary>
        // /// Print solution found message with iteration count and node path
        // /// </summary>
        // private static void PrintSolutionFound(string algorithmName, int iteration, GameState state)
        // {
        //     Console.WriteLine($"\nSolution found with {algorithmName}!");
        //     Console.WriteLine($"--- Iterations: {iteration} ---");
        //     Helpers.PrintSolution(state, algorithmName);
        // }

        #endregion

        #region Search Algorithms

        private static Stopwatch backtrackingStopwatch = new Stopwatch();

        // Track all visited states for visualization
        private static Dictionary<string, GameState> visitedStates = new Dictionary<string, GameState>();

        /// <summary>
        /// A* algorithm with customizable heuristic function
        /// </summary>
        public static AlgorithmStats AStar(
            int[,] initialBoard,
            int initialPegCount,
            Func<int[,], int> heuristicFunction = null)
        {
            string algorithmName = "A* Search";
            var stats = new AlgorithmStats(algorithmName);

            for (int i = 0; i < Config.AlgorithmIterations; i++)
            {
                visitedStates.Clear(); // Reset for each run
                var stopwatch = Stopwatch.StartNew();
                GameState solutionState = null;
                int iterations = 0;
                bool foundSolution = false;

                try
                {
                    // Initialize algorithm
                    var frontier = new PriorityQueue<GameState, int>();
                    var explored = new HashSet<string>();
                    var exploredHash = new HashSet<string>();

                    // Use default heuristic from config if none provided
                    heuristicFunction ??= Config.DefaultAStarHeuristic;

                    // Create initial state
                    var initialState = new GameState(
                        board: initialBoard,
                        pegCount: initialPegCount,
                        parent: null,
                        action: (0, 0, 0, 0),
                        pathCost: 0,
                        hash: Game.GenerateBoardHashes(initialBoard),
                        heuristicValue: heuristicFunction(initialBoard));

                    initialState.UpdateHeuristic(heuristicFunction);
                    frontier.Enqueue(initialState, initialState.HeuristicValue);
                    visitedStates[GetStateKey(initialState)] = initialState;

                    while (frontier.Count > 0)
                    {
                        iterations++;

                        // Check for timeout or max iterations
                        if (Config.TimeoutMs > 0 && stopwatch.ElapsedMilliseconds > Config.TimeoutMs)
                        {
                            Console.WriteLine($"\nTimeout reached after {iterations} iterations and {stopwatch.ElapsedMilliseconds}ms");
                            break;
                        }

                        if (Config.MaxIterations > 0 && iterations > Config.MaxIterations)
                        {
                            Console.WriteLine($"\nMax iterations ({Config.MaxIterations}) reached after {stopwatch.ElapsedMilliseconds}ms");
                            break;
                        }

                        // Get next state
                        var currentState = frontier.Dequeue();

                        // Check if solution found
                        if (currentState.PegCount == 1)
                        {
                            foundSolution = true;
                            solutionState = currentState;
                            PrintSolutionFound(algorithmName, iterations, stopwatch, currentState);
                            break;
                        }

                        // Skip if already explored
                        string boardKey = Helpers.BoardToString(currentState.Board);
                        if (explored.Contains(boardKey))
                            continue;

                        explored.Add(boardKey);

                        // Check hash if enabled
                        if (Config.CheckHashValues)
                        {
                            if (exploredHash.Contains(currentState.StateHash))
                                continue;
                            exploredHash.Add(currentState.StateHash);
                        }

                        // Expand this state
                        ExpandMoves(currentState, frontier, visitedStates);

                        // Show progress periodically
                        if (iterations % Config.ProgressUpdateInterval == 0)
                        {
                            Console.Write($"\r{algorithmName} Iteration {iterations}, Queue: {frontier.Count}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nError in {algorithmName}: {ex.Message}");
                }
                finally
                {
                    stopwatch.Stop();
                }

                // Record statistics
                stats.RecordIteration(
                    stopwatch.ElapsedMilliseconds,
                    iterations,
                    foundSolution,
                    foundSolution ? solutionState.Level : 0
                );

                // Generate visualization if solution found
                if (foundSolution && solutionState != null)
                {
                    GraphvizGenerator.GenerateSolutionPath(algorithmName, solutionState);

                    // Only generate tree for the first successful run to avoid too many files
                    if (stats.SuccessfulRuns == 1 && visitedStates.Count <= 5000)
                    {
                        GraphvizGenerator.GenerateSolutionTree(algorithmName, visitedStates);
                    }
                }

                Console.WriteLine($"\nRun {i + 1}/{Config.AlgorithmIterations} - {algorithmName} completed in {stopwatch.ElapsedMilliseconds}ms with {iterations} iterations.");
            }

            // Print overall summary
            stats.PrintSummary();
            return stats;
        }

        // Helper method to get a unique key for a state
        private static string GetStateKey(GameState state)
        {
            return Helpers.BoardToString(state.Board);
        }

        // Helper method to expand moves from a state
        private static void ExpandMoves(GameState currentState, PriorityQueue<GameState, int> frontier, Dictionary<string, GameState> visitedStates)
        {
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
                            int newPathCost = currentState.PathCost + Config.DefaultMoveCost;

                            var newState = new GameState(
                                board: newBoard,
                                pegCount: newPegCount,
                                parent: currentState,
                                action: (x, y, dx, dy),
                                pathCost: newPathCost,
                                hash: Game.GenerateBoardHashes(newBoard),
                                heuristicValue: Config.DefaultAStarHeuristic(newBoard));

                            frontier.Enqueue(newState, newState.HeuristicValue);

                            // Track the state for visualization
                            string stateKey = GetStateKey(newState);
                            if (!visitedStates.ContainsKey(stateKey))
                            {
                                visitedStates[stateKey] = newState;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Best First Search algorithm with custom heuristic function.
        /// </summary>
        public static AlgorithmStats BestFirstSearch(int[,] initialBoard, Func<int[,], int> heuristicFunction = null)
        {
            // Use default if no heuristic is provided
            heuristicFunction ??= Heuristica.Centrality;

            string algorithmName = "BestFirstSearch";
            var stats = new AlgorithmStats(algorithmName);

            for (int i = 0; i < Config.AlgorithmIterations; i++)
            {
                visitedStates.Clear(); // Reset for each run
                var stopwatch = Stopwatch.StartNew();
                GameState solutionState = null;
                int iterations = 0;
                bool foundSolution = false;

                try
                {
                    // Initialize algorithm
                    var frontier = new PriorityQueue<GameState, int>();
                    var explored = new HashSet<string>();
                    var exploredHash = new HashSet<string>();

                    // Create initial state
                    var initialState = new GameState(
                        board: initialBoard,
                        pegCount: Heuristica.CountPegs(initialBoard),
                        parent: null,
                        action: (0, 0, 0, 0),
                        pathCost: 0,
                        hash: Config.CheckHashValues ? Game.GenerateBoardHashes(initialBoard) : string.Empty,
                        heuristicValue: heuristicFunction(initialBoard))
                    {
                        Level = 0
                    };

                    frontier.Enqueue(initialState, initialState.HeuristicValue);
                    visitedStates[GetStateKey(initialState)] = initialState;

                    while (frontier.Count > 0)
                    {
                        iterations++;

                        // Check for timeout or max iterations
                        if (Config.TimeoutMs > 0 && stopwatch.ElapsedMilliseconds > Config.TimeoutMs)
                        {
                            Console.WriteLine($"\nTimeout reached after {iterations} iterations and {stopwatch.ElapsedMilliseconds}ms");
                            break;
                        }

                        if (Config.MaxIterations > 0 && iterations > Config.MaxIterations)
                        {
                            Console.WriteLine($"\nMax iterations ({Config.MaxIterations}) reached after {stopwatch.ElapsedMilliseconds}ms");
                            break;
                        }

                        // Get next state
                        var currentState = frontier.Dequeue();

                        // Check if solution found
                        if (currentState.PegCount == 1)
                        {
                            foundSolution = true;
                            solutionState = currentState;
                            PrintSolutionFound(algorithmName, iterations, stopwatch, currentState);
                            break;
                        }

                        // Skip if already explored
                        string boardKey = Helpers.BoardToString(currentState.Board);
                        if (explored.Contains(boardKey))
                            continue;

                        explored.Add(boardKey);

                        // Check hash if enabled
                        if (Config.CheckHashValues)
                        {
                            if (exploredHash.Contains(currentState.StateHash))
                                continue;
                            exploredHash.Add(currentState.StateHash);
                        }

                        // Expand valid moves
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
                                        var newState = new GameState(
                                            board: newBoard,
                                            pegCount: newPegCount,
                                            parent: currentState,
                                            action: (x, y, dx, dy),
                                            pathCost: currentState.PathCost + Config.DefaultMoveCost,
                                            hash: Config.CheckHashValues ? Game.GenerateBoardHashes(newBoard) : string.Empty,
                                            heuristicValue: heuristicFunction(newBoard))
                                        {
                                            Level = currentState.Level + 1
                                        };

                                        frontier.Enqueue(newState, newState.HeuristicValue);

                                        // Track for visualization
                                        string stateKey = GetStateKey(newState);
                                        if (!visitedStates.ContainsKey(stateKey))
                                        {
                                            visitedStates[stateKey] = newState;
                                        }
                                    }
                                }
                            }
                        }

                        // Show progress periodically
                        if (iterations % Config.ProgressUpdateInterval == 0)
                        {
                            Console.Write($"\r{algorithmName} Iteration {iterations}, Queue: {frontier.Count}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nError in {algorithmName}: {ex.Message}");
                }
                finally
                {
                    stopwatch.Stop();
                }

                // Record statistics
                stats.RecordIteration(
                    stopwatch.ElapsedMilliseconds,
                    iterations,
                    foundSolution,
                    foundSolution ? solutionState.Level : 0
                );

                // Generate visualization if solution found
                if (foundSolution && solutionState != null)
                {
                    GraphvizGenerator.GenerateSolutionPath(algorithmName, solutionState);

                    // Only generate tree for the first successful run to avoid too many files
                    if (stats.SuccessfulRuns == 1 && visitedStates.Count <= 5000)
                    {
                        GraphvizGenerator.GenerateSolutionTree(algorithmName, visitedStates);
                    }
                }

                Console.WriteLine($"\nRun {i + 1}/{Config.AlgorithmIterations} - {algorithmName} completed in {stopwatch.ElapsedMilliseconds}ms with {iterations} iterations.");
            }

            // Print overall summary
            stats.PrintSummary();
            return stats;
        }

        /// <summary>
        /// A* algorithm with weighted heuristic
        /// </summary>
        public static AlgorithmStats AStarWeightedHeuristic(
            int[,] initialBoard,
            double weight = 0,
            Func<int[,], int> heuristicFunction = null)
        {
            // Use default weight from config if not specified
            if (weight <= 0)
                weight = Config.DefaultWeight;

            // Use default heuristic from config if none provided
            heuristicFunction ??= Config.DefaultWeightedAStarHeuristic;

            // Create stats tracker
            var stats = new AlgorithmStats("A* Weighted Search");

            // Run algorithm multiple times based on config
            for (int run = 0; run < Config.AlgorithmIterations; run++)
            {
                Console.WriteLine($"\nRunning A* Weighted Search - Iteration {run + 1}/{Config.AlgorithmIterations}");

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
                bool foundSolution = false;
                int solutionDepth = 0;

                try
                {
                    while (frontier.Count > 0)
                    {
                        iteration++;
                        var currentState = frontier.Dequeue();

                        // Check for timeout
                        if (Config.TimeoutMs > 0 && stopwatch.ElapsedMilliseconds > Config.TimeoutMs)
                        {
                            Console.WriteLine($"\nAlgorithm timed out after {stopwatch.ElapsedMilliseconds}ms");
                            break;
                        }

                        if (currentState.IsSolution())
                        {
                            foundSolution = true;
                            solutionDepth = currentState.Level;

                            PrintSolutionFound("A* Weighted", iteration, stopwatch, currentState);

                            break;
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
                }
                catch (TimeoutException e)
                {
                    Console.WriteLine(e.Message);
                }

                stopwatch.Stop();

                // Record statistics
                stats.RecordIteration(
                    stopwatch.ElapsedMilliseconds,
                    iteration,
                    foundSolution,
                    solutionDepth);

                if (!foundSolution && run == Config.AlgorithmIterations - 1)
                    Console.WriteLine("\nNo solution found.");
            }

            // Print performance summary
            stats.PrintSummary();

            return stats;
        }

        /// <summary>
        /// Ordered Search with multiple criteria for tiebreaking
        /// </summary>
        public static AlgorithmStats OrderedSearch(
            int[,] initialBoard,
            Func<int[,], int> primaryHeuristic = null,
            Func<int[,], int> secondaryHeuristic = null)
        {
            // Use default heuristics from config if none provided
            primaryHeuristic ??= Config.DefaultPrimaryHeuristic;
            secondaryHeuristic ??= Config.DefaultSecondaryHeuristic;

            // Create stats tracker
            var stats = new AlgorithmStats("Ordered Search");

            // Run algorithm multiple times based on config
            for (int run = 0; run < Config.AlgorithmIterations; run++)
            {
                Console.WriteLine($"\nRunning Ordered Search - Iteration {run + 1}/{Config.AlgorithmIterations}");

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
                bool foundSolution = false;
                int solutionDepth = 0;

                try
                {
                    while (frontier.Count > 0)
                    {
                        iteration++;
                        var currentState = frontier.Dequeue();

                        // Check for timeout
                        if (Config.TimeoutMs > 0 && stopwatch.ElapsedMilliseconds > Config.TimeoutMs)
                        {
                            Console.WriteLine($"\nAlgorithm timed out after {stopwatch.ElapsedMilliseconds}ms");
                            break;
                        }

                        if (currentState.IsSolution())
                        {
                            foundSolution = true;
                            solutionDepth = currentState.Level;

                            PrintSolutionFound("Ordered Search", iteration, stopwatch, currentState);

                            break;
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
                }
                catch (TimeoutException e)
                {
                    Console.WriteLine(e.Message);
                }

                stopwatch.Stop();

                // Record statistics
                stats.RecordIteration(
                    stopwatch.ElapsedMilliseconds,
                    iteration,
                    foundSolution,
                    solutionDepth);

                if (!foundSolution && run == Config.AlgorithmIterations - 1)
                    Console.WriteLine("\nNo solution found.");
            }

            // Print performance summary
            stats.PrintSummary();

            return stats;
        }

        /// <summary>
        /// Breadth First Search algorithm
        /// </summary>
        public static AlgorithmStats BreadthFirstSearch(int[,] initialBoard, int initialPegCount)
        {
            // Create stats tracker
            var stats = new AlgorithmStats("Breadth First Search");

            // Run algorithm multiple times based on config
            for (int run = 0; run < Config.AlgorithmIterations; run++)
            {
                Console.WriteLine($"\nRunning Breadth First Search - Iteration {run + 1}/{Config.AlgorithmIterations}");

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
                bool foundSolution = false;
                int solutionDepth = 0;

                try
                {
                    while (queue.Count > 0)
                    {
                        iteration++;
                        var currentState = queue.Dequeue();

                        // Check for timeout
                        if (Config.TimeoutMs > 0 && stopwatch.ElapsedMilliseconds > Config.TimeoutMs)
                        {
                            Console.WriteLine($"\nAlgorithm timed out after {stopwatch.ElapsedMilliseconds}ms");
                            break;
                        }

                        if (currentState.IsSolution())
                        {
                            foundSolution = true;
                            solutionDepth = currentState.Level;

                            // Only print in last iteration
                            PrintSolutionFound("BFS", iteration, stopwatch, currentState);

                            break;
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
                }
                catch (TimeoutException e)
                {
                    Console.WriteLine(e.Message);
                }

                stopwatch.Stop();

                // Record statistics
                stats.RecordIteration(
                    stopwatch.ElapsedMilliseconds,
                    iteration,
                    foundSolution,
                    solutionDepth);

                if (!foundSolution && run == Config.AlgorithmIterations - 1)
                    Console.WriteLine("\nNo solution found.");
            }

            // Print performance summary
            stats.PrintSummary();

            return stats;
        }

        /// <summary>
        /// Depth First Search algorithm
        /// </summary>
        public static AlgorithmStats DepthFirstSearch(int[,] initialBoard, int initialPegCount)
        {
            // Create stats tracker
            var stats = new AlgorithmStats("Depth First Search");

            // Run algorithm multiple times based on config
            for (int run = 0; run < Config.AlgorithmIterations; run++)
            {
                Console.WriteLine($"\nRunning Depth First Search - Iteration {run + 1}/{Config.AlgorithmIterations}");

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
                bool foundSolution = false;
                int solutionDepth = 0;

                try
                {
                    while (stack.Count > 0)
                    {
                        iteration++;
                        var currentState = stack.Pop();

                        // Check for timeout
                        if (Config.TimeoutMs > 0 && stopwatch.ElapsedMilliseconds > Config.TimeoutMs)
                        {
                            Console.WriteLine($"\nAlgorithm timed out after {stopwatch.ElapsedMilliseconds}ms");
                            break;
                        }

                        if (currentState.IsSolution())
                        {
                            foundSolution = true;
                            solutionDepth = currentState.Level;

                            PrintSolutionFound("DFS", iteration, stopwatch, currentState);

                            break;
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
                }
                catch (TimeoutException e)
                {
                    Console.WriteLine(e.Message);
                }

                stopwatch.Stop();

                // Record statistics
                stats.RecordIteration(
                    stopwatch.ElapsedMilliseconds,
                    iteration,
                    foundSolution,
                    solutionDepth);

                if (!foundSolution && run == Config.AlgorithmIterations - 1)
                    Console.WriteLine("\nNo solution found.");
            }

            // Print performance summary
            stats.PrintSummary();

            return stats;
        }

        /// <summary>
        /// Recursive Backtracking Search algorithm
        /// </summary>
        private static int backtrackingIterationCount = 0;
        private static bool backtrackingSolutionFound = false;
        private static int backtrackingSolutionDepth = 0;

        public static AlgorithmStats SolveBacktracking(int[,] initialBoard, int initialPegCount)
        {
            // Create stats tracker
            var stats = new AlgorithmStats("Backtracking Search");

            // Run algorithm multiple times based on config
            for (int run = 0; run < Config.AlgorithmIterations; run++)
            {
                Console.WriteLine($"\nRunning Backtracking Search - Iteration {run + 1}/{Config.AlgorithmIterations}");

                backtrackingIterationCount = 0;
                backtrackingSolutionFound = false;
                backtrackingSolutionDepth = 0;
                backtrackingStopwatch = new Stopwatch();
                backtrackingStopwatch.Start();

                var visited = new HashSet<string>();

                var initialState = new GameState(
                    board: initialBoard,
                    pegCount: initialPegCount,
                    parent: null,
                    action: (0, 0, 0, 0),
                    pathCost: 0,
                    hash: Game.GenerateBoardHashes(initialBoard),
                    heuristicValue: 0);

                bool result = BacktrackingSearch(initialState, visited);

                backtrackingStopwatch.Stop();

                // Record statistics
                stats.RecordIteration(
                    backtrackingStopwatch.ElapsedMilliseconds,
                    backtrackingIterationCount,
                    backtrackingSolutionFound,
                    backtrackingSolutionDepth);

                if (!result && run == Config.AlgorithmIterations - 1)
                    Console.WriteLine($"\n--- Iterations: {backtrackingIterationCount} ---");
            }

            // Print performance summary
            stats.PrintSummary();

            return stats;
        }

        private static bool BacktrackingSearch(GameState state, HashSet<string> visited)
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

            if (state.IsSolution())
            {
                backtrackingSolutionFound = true;
                backtrackingSolutionDepth = state.Level;

                if (Config.AlgorithmIterations <= 1) // Only print if not running multiple iterations
                    PrintSolutionFound("Backtracking", backtrackingIterationCount, backtrackingStopwatch, state);

                return true;
            }

            string boardKey = Helpers.BoardToString(state.Board);
            if (visited.Contains(boardKey))
                return false;

            visited.Add(boardKey);

            // Expand all valid moves recursively
            foreach (var (newBoard, move) in ExpandMoves(state.Board))
            {
                var newState = new GameState(
                    board: newBoard,
                    pegCount: state.PegCount - 1,
                    parent: state,
                    action: move,
                    pathCost: state.PathCost + Config.DefaultMoveCost,
                    hash: Game.GenerateBoardHashes(newBoard),
                    heuristicValue: 0);

                if (BacktrackingSearch(newState, visited))
                    return true;

                try
                {
                    ShowProgress(backtrackingIterationCount);
                }
                catch (TimeoutException e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Optimized Breadth First Search using bitmasks
        /// </summary>
        public static AlgorithmStats OptimizedBFSBitmask(int[,] initialBoard, int initialPegCount)
        {
            // Create stats tracker
            var stats = new AlgorithmStats("Optimized BFS Bitmask");

            // Run algorithm multiple times based on config
            for (int run = 0; run < Config.AlgorithmIterations; run++)
            {
                Console.WriteLine($"\nRunning Optimized BFS Bitmask - Iteration {run + 1}/{Config.AlgorithmIterations}");
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                bool result = PegSolitaireOptimized.OptimizedBFSBitmask(initialBoard, initialPegCount);

                stopwatch.Stop();

                // Since we can't get iteration count from the optimized implementation,
                // we'll use a placeholder value of -1
                stats.RecordIteration(
                    stopwatch.ElapsedMilliseconds,
                    -1, // We don't have access to iteration count
                    result,
                    0); // We don't have access to solution depth
            }

            // Print performance summary
            stats.PrintSummary();

            return stats;
        }

        #endregion
    }
}