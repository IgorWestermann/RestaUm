using System;
using System.IO;
using RestaUm;
using RestaUm.Game;
using RestaUm.Helpers;
using RestaUm.Performance;

namespace RestaUm
{
    public class RunPerformanceAnalysis
    {
        public static void Run()
        {
            Console.WriteLine("===== Peg Solitaire Algorithm Performance Analysis =====");

            // Set up the initial board
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

            Console.WriteLine("Initial board:");
            Helpers.Helpers.PrintBoard(board);
            Console.WriteLine($"Initial number of pegs: {pegCount}");

            // Configure analysis parameters
            ConfigureAnalysisParameters();

            // Define output path - either from args or default
            string outputPath = Config.OutputPath;
            //Path.Combine(Directory.GetCurrentDirectory(), "AlgorithmPerformanceReport.md");
            Path.Combine(outputPath, "AlgorithmPerformanceReport.md");

            Console.WriteLine($"Report will be saved to: {outputPath}");
            Console.WriteLine("Press any key to start the performance analysis...");
            Console.ReadKey();

            // Create and run the analyzer
            var analyzer = new AlgorithmAnalyzer(board, pegCount, outputPath);
            analyzer.RunAllAlgorithms();
            analyzer.GenerateMarkdownReport();

            Console.WriteLine("\n===== Performance Analysis Complete =====");
            Console.WriteLine($"Full report generated at: {outputPath}");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static void ConfigureAnalysisParameters()
        {
            Console.WriteLine("\nConfiguring analysis parameters...");

            Console.WriteLine("\nAnalysis configuration:");
            Console.WriteLine($"- Iterations per algorithm: {Config.AlgorithmIterations}");
            Console.WriteLine($"- Timeout: {Config.TimeoutMs}ms");
            Console.WriteLine($"- Max iterations: {Config.MaxIterations}");
            Console.WriteLine($"- Progress update interval: {Config.ProgressUpdateInterval}");
            Console.WriteLine();
        }
    }
}