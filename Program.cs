using System;
using RestaUm.Helpers;
using RestaUm;
using RestaUm.Game;
using RestaUm.Algorithms;
using System.IO;
using RestaUm.Performance;

// Main entry point
RunAndGenerateVisualizations();
return 0;

void RunAndGenerateVisualizations()
{
    Console.WriteLine("===== Generating Algorithm Visualizations =====");

    // Configure parameters for good visualization results
    Config.ConfigureForVisualization();

    // Create initial board
    int[,] board = new int[7, 7]
    {
        { -1, -1,  1,  1,  1, -1, -1 },
        { -1, -1,  1,  1,  1, -1, -1 },
        {  1,  1,  1,  1,  1,  1,  1 },
        {  1,  1,  1,  0,  1,  1,  1 },
        {  1,  1,  1,  1,  1,  1,  1 },
        { -1, -1,  1,  1,  1, -1, -1 },
        { -1, -1,  1,  1,  1, -1, -1 }
    };

    int pegCount = Heuristica.CountPegs(board);

    // Create directory for visualizations
    Directory.CreateDirectory("reports");
    Directory.CreateDirectory("reports/images");

    Console.WriteLine("Initial board state:");
    Helpers.PrintBoard(board);
    Console.WriteLine($"Initial peg count: {pegCount}\n");

    // Run A* Search visualization
    Console.WriteLine("\n[1/5] Running A* Search visualization...");
    var aStarStats = NewAlgorithm.AStar(board, pegCount);

    // Run Best First Search visualization
    Console.WriteLine("\n[2/5] Running Best First Search visualization...");
    var bfsStats = NewAlgorithm.BestFirstSearch(board);

    // Run Depth First Search visualization
    Console.WriteLine("\n[3/5] Running Depth First Search visualization...");
    var dfsStats = NewAlgorithm.DepthFirstSearch(board, pegCount);

    // Run Backtracking Search visualization
    Console.WriteLine("\n[4/5] Running Backtracking Search visualization...");
    var backtrackingStats = NewAlgorithm.SolveBacktracking(board, pegCount);

    // Run Breadth First Search visualization
    Console.WriteLine("\n[5/5] Running Breadth First Search visualization...");
    var breadthFirstStats = NewAlgorithm.BreadthFirstSearch(board, pegCount);

    // Print summary
    Console.WriteLine("\n===== Visualization Generation Complete =====");
    Console.WriteLine("DOT files have been generated in the 'reports' directory.");
    Console.WriteLine("\nTo generate PNG images from DOT files:");
    Console.WriteLine("1. Make sure Graphviz is installed");
    Console.WriteLine("2. Run the appropriate script based on your OS:");
    Console.WriteLine("   - Windows: generate_graphs.bat");
    Console.WriteLine("   - Linux/macOS: ./generate_graphs.sh");
    Console.WriteLine("\nThe generated images will be in the 'reports/images' directory.");

    // Also run performance analysis
    RunPerformanceAnalysis(board, pegCount);
}

void RunPerformanceAnalysis(int[,] board, int pegCount)
{
    Console.WriteLine("\n===== Starting Performance Analysis =====");
    Console.WriteLine("This analysis will run all algorithms with and without hash verification");
    Console.WriteLine("The process may take several minutes. Please wait...");

    // Create the analyzer and run the tests
    string outputPath = Path.Combine(Directory.GetCurrentDirectory(), "reports");
    var analyzer = new AlgorithmAnalyzer(board, pegCount, outputPath);

    analyzer.RunAllAlgorithms();
    analyzer.GenerateMarkdownReport();

    Console.WriteLine("\n===== Performance Analysis Complete =====");
    Console.WriteLine($"Full report generated at: {outputPath}");
}

