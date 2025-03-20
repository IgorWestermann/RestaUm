namespace RestaUm
{
    using RestaUm.Game;
    using RestaUm.Helpers;
    using System;

    /// <summary>
    /// Static configuration class that centralizes parameters for algorithm implementations
    /// </summary>
    public static class Config
    {
        #region Algorithm Defaults

        /// <summary>
        /// Default heuristic function for A* algorithm
        /// </summary>
        public static Func<int[,], int> DefaultAStarHeuristic = Heuristica.CountPegs;

        /// <summary>
        /// Default heuristic function for Best First Search algorithm
        /// </summary>
        public static Func<int[,], int> DefaultGreedyHeuristic = Heuristica.CountPegs;

        /// <summary>
        /// Default heuristic function for Weighted A* algorithm
        /// </summary>
        public static Func<int[,], int> DefaultWeightedAStarHeuristic = Heuristica.Centrality;

        /// <summary>
        /// Default primary heuristic function for Ordered Search
        /// </summary>
        public static Func<int[,], int> DefaultPrimaryHeuristic = Heuristica.Centrality;

        /// <summary>
        /// Default secondary heuristic function for Ordered Search
        /// </summary>
        public static Func<int[,], int> DefaultSecondaryHeuristic = Helpers.Helpers.FutureMobility;

        /// <summary>
        /// Default weight for Weighted A* algorithm
        /// </summary>
        public static double DefaultWeight = 1.0;

        /// <summary>
        /// Default path cost increment for each move
        /// </summary>
        public static int DefaultMoveCost = 1;

        #endregion

        #region Performance Configuration

        /// <summary>
        /// Iteration interval for showing progress updates
        /// </summary>
        public static int ProgressUpdateInterval = 500;

        /// <summary>
        /// Maximum iterations before timing out (0 means no limit)
        /// </summary>
        public static int MaxIterations = 0;

        /// <summary>
        /// Maximum time in milliseconds before timing out (0 means no limit)
        /// </summary>
        public static int TimeoutMs = 0;

        /// <summary>
        /// Whether to check for hash values to detect symmetric board positions
        /// </summary>
        public static bool CheckHashValues = true;

        #endregion

        #region Debugging Options

        /// <summary>
        /// Whether to enable verbose logging
        /// </summary>
        public static bool VerboseLogging = false;

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

        #endregion
    }
}