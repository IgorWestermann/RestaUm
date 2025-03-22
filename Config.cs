namespace RestaUm
{
    using RestaUm.Game;
    using RestaUm.Helpers;
    using System;
    using System.IO;

    /// <summary>
    /// Static configuration class that centralizes parameters for algorithm implementations
    /// </summary>
    public static class Config
    {
        #region Algorithm Defaults

        /// <summary>
        /// Default heuristic function for A* algorithm
        /// </summary>
        public static Func<int[,], int> DefaultAStarHeuristic { get; set; } = Heuristica.CountPegs;

        /// <summary>
        /// Default heuristic function for Best First Search algorithm
        /// </summary>
        public static Func<int[,], int> DefaultGreedyHeuristic { get; set; } = Heuristica.CountPegs;

        /// <summary>
        /// Default heuristic function for Weighted A* algorithm
        /// </summary>
        public static Func<int[,], int> DefaultWeightedAStarHeuristic { get; set; } = Heuristica.Centrality;

        /// <summary>
        /// Default primary heuristic function for Ordered Search
        /// </summary>
        public static Func<int[,], int> DefaultPrimaryHeuristic { get; set; } = Heuristica.Centrality;

        /// <summary>
        /// Default secondary heuristic function for Ordered Search
        /// </summary>
        public static Func<int[,], int> DefaultSecondaryHeuristic { get; set; } = Helpers.Helpers.FutureMobility;

        /// <summary>
        /// Default weight for Weighted A* algorithm
        /// </summary>
        public static double DefaultWeight { get; private set; } = 1.5;

        /// <summary>
        /// Default path cost increment for each move
        /// </summary>
        public static int DefaultMoveCost { get; private set; } = 1;

        /// <summary>
        /// Number of times to run each algorithm for benchmarking
        /// </summary>
        public static int AlgorithmIterations { get; private set; } = 1;

        #endregion

        #region Performance Configuration

        /// <summary>
        /// Iteration interval for showing progress updates
        /// </summary>
        public static int ProgressUpdateInterval { get; private set; } = 1000;

        /// <summary>
        /// Maximum iterations before timing out (0 means no limit)
        /// </summary>
        public static int MaxIterations { get; private set; } = 1000000;

        /// <summary>
        /// Maximum time in milliseconds before timing out (0 means no limit)
        /// </summary>
        public static int TimeoutMs { get; private set; } = 3 * 60000;

        /// <summary>
        /// Whether to check for hash values to detect symmetric board positions
        /// </summary>
        public static bool CheckHashValues { get; private set; } = true;

        #endregion

        #region Debugging Options

        /// <summary>
        /// Whether to enable verbose logging
        /// </summary>
        public static bool VerboseLogging = true;

        /// <summary>
        /// Whether to export search trees to DOT format for visualization
        /// </summary>
        public static bool ExportSearchTrees = false;

        /// <summary>
        /// Path where to save search tree visualizations
        /// </summary>
        public static string SearchTreeExportPath = "./";

        #endregion

        #region Initial Board Configuration

        /// <summary>
        /// Standard initial board for the game
        /// </summary>
        public static int[,] StandardBoard = new int[7, 7]
        {
            { -1, -1,  1,  1,  1, -1, -1 },
            { -1, -1,  1,  1,  1, -1, -1 },
            {  1,  1,  1,  1,  1,  1,  1 },
            {  1,  1,  1,  0,  1,  1,  1 },
            {  1,  1,  1,  1,  1,  1,  1 },
            { -1, -1,  1,  1,  1, -1, -1 },
            { -1, -1,  1,  1,  1, -1, -1 }
        };

        /// <summary>
        /// Board size (default is 7x7)
        /// </summary>
        public static int BoardSize { get; set; } = 7;
        public static string OutputPath { get; internal set; } = Directory.GetCurrentDirectory() + "/reports/";

        #endregion

        #region Configuration Methods

        /// <summary>
        /// Set hash verification option
        /// </summary>
        public static void UseHashVerification(bool value)
        {
            CheckHashValues = value;
        }

        /// <summary>
        /// Set the number of algorithm iterations
        /// </summary>
        public static void SetAlgorithmIterations(int value)
        {
            AlgorithmIterations = value;
        }

        /// <summary>
        /// Set the maximum number of iterations before timing out
        /// </summary>
        public static void SetMaxIterations(int value)
        {
            MaxIterations = value;
        }

        /// <summary>
        /// Set the timeout value in milliseconds
        /// </summary>
        public static void SetTimeoutMs(int value)
        {
            TimeoutMs = value;
        }

        /// <summary>
        /// Set the progress update interval
        /// </summary>
        public static void SetProgressUpdateInterval(int value)
        {
            ProgressUpdateInterval = value;
        }

        /// <summary>
        /// Configure visualization settings
        /// </summary>
        public static void ConfigureForVisualization()
        {
            SetAlgorithmIterations(1);
            SetMaxIterations(10000000);
            SetTimeoutMs(100000);
            UseHashVerification(false);
            SetProgressUpdateInterval(50);
        }

        #endregion
    }
}