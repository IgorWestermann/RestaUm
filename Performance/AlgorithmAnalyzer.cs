namespace RestaUm.Performance;

using RestaUm.Algorithms;
using RestaUm.States;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

/// <summary>
/// Utility class to analyze and compare algorithm performance with different configurations
/// </summary>
public class AlgorithmAnalyzer
{
    private readonly int[,] _board;
    private readonly int _pegCount;
    private readonly string _outputFilePath;
    private readonly Dictionary<string, (AlgorithmStats WithHash, AlgorithmStats WithoutHash)> _results;

    /// <summary>
    /// Creates a new algorithm analyzer
    /// </summary>
    /// <param name="board">The initial board configuration</param>
    /// <param name="pegCount">Number of pegs on the board</param>
    /// <param name="outputFilePath">Path to save the markdown report</param>
    public AlgorithmAnalyzer(int[,] board, int pegCount, string outputFilePath)
    {
        _board = board;
        _pegCount = pegCount;
        _outputFilePath = outputFilePath;
        _results = new Dictionary<string, (AlgorithmStats, AlgorithmStats)>();
    }

    /// <summary>
    /// Runs all algorithms with both hash verification enabled and disabled
    /// </summary>
    public void RunAllAlgorithms()
    {

        Console.WriteLine("===== Starting Performance Analysis =====");
        Console.WriteLine("Running all algorithms with and without hash verification");
        Console.WriteLine($"Each algorithm will run {Config.AlgorithmIterations} times per configuration\n");

        // A* algorithm
        Console.WriteLine("Testing A* algorithm...");
        Config.UseHashVerification(true);
        Console.WriteLine("with hash");
        var aStarWithHash = NewAlgorithm.AStar(_board, _pegCount);
        Config.UseHashVerification(false);
        Console.WriteLine("without hash");
        var aStarWithoutHash = NewAlgorithm.AStar(_board, _pegCount);
        _results["A*"] = (aStarWithHash, aStarWithoutHash);

        // Best First Search
        Console.WriteLine("Testing Best First Search algorithm...");
        Config.UseHashVerification(true);
        var bfsWithHash = NewAlgorithm.BestFirstSearch(_board);
        Config.UseHashVerification(false);
        var bfsWithoutHash = NewAlgorithm.BestFirstSearch(_board);
        _results["Best First Search"] = (bfsWithHash, bfsWithoutHash);

        // A* Weighted
        Console.WriteLine("Testing A* Weighted algorithm...");
        Config.UseHashVerification(true);
        var aStarWeightedWithHash = NewAlgorithm.AStarWeightedHeuristic(_board, 1.5);
        Config.UseHashVerification(false);
        var aStarWeightedWithoutHash = NewAlgorithm.AStarWeightedHeuristic(_board, 1.5);
        _results["A* Weighted"] = (aStarWeightedWithHash, aStarWeightedWithoutHash);

        // Ordered Search
        Console.WriteLine("Testing Ordered Search algorithm...");
        Config.UseHashVerification(true);
        var orderedSearchWithHash = NewAlgorithm.OrderedSearch(_board);
        Config.UseHashVerification(false);
        var orderedSearchWithoutHash = NewAlgorithm.OrderedSearch(_board);
        _results["Ordered Search"] = (orderedSearchWithHash, orderedSearchWithoutHash);

        // Depth First Search
        Console.WriteLine("Testing Depth First Search algorithm...");
        Console.WriteLine("Testing Ordered Search algorithm...");
        Config.UseHashVerification(true);
        var dfsWithHash = NewAlgorithm.DepthFirstSearch(_board, _pegCount);
        Config.UseHashVerification(false);
        var dfsWithoutHash = NewAlgorithm.DepthFirstSearch(_board, _pegCount);
        _results["Depth First Search"] = (dfsWithHash, dfsWithoutHash);

        // Backtracking
        Console.WriteLine("Testing Backtracking algorithm...");
        Config.UseHashVerification(true);
        var backtrackingWithHash = NewAlgorithm.SolveBacktracking(_board, _pegCount);
        Config.UseHashVerification(false);
        var backtrackingWithoutHash = NewAlgorithm.SolveBacktracking(_board, _pegCount);
        _results["Backtracking"] = (backtrackingWithHash, backtrackingWithoutHash);

        // BFS
        // Console.WriteLine("Testing Breadth First Search algorithm...");
        // Config.UseHashVerification(true);
        // var breadthFirstWithHash = NewAlgorithm.BreadthFirstSearch(_board, _pegCount);
        // Config.UseHashVerification(false);
        // var breadthFirstWithoutHash = NewAlgorithm.BreadthFirstSearch(_board, _pegCount);
        // _results["Breadth First Search"] = (breadthFirstWithHash, breadthFirstWithoutHash);

        Console.WriteLine("All algorithms have been tested!");
    }

    /// <summary>
    /// Generates a comprehensive Markdown report of algorithm performance
    /// </summary>
    public void GenerateMarkdownReport()
    {
        Console.WriteLine($"Generating performance report at: {_outputFilePath}");

        var sb = new StringBuilder();

        // Add report header
        sb.AppendLine("# Algorithm Performance Analysis Report");
        sb.AppendLine($"Generated on: {DateTime.Now}\n");

        sb.AppendLine("## Test Configuration");
        sb.AppendLine($"- Algorithm Iterations: {Config.AlgorithmIterations}");
        sb.AppendLine($"- Max Iterations Limit: {(Config.MaxIterations > 0 ? Config.MaxIterations.ToString() : "Unlimited")}");
        sb.AppendLine($"- Timeout: {(Config.TimeoutMs > 0 ? $"{Config.TimeoutMs}ms" : "Unlimited")}");
        sb.AppendLine($"- Default Move Cost: {Config.DefaultMoveCost}");
        sb.AppendLine($"- Progress Update Interval: {Config.ProgressUpdateInterval}\n");

        // Add executive summary
        sb.AppendLine("## Executive Summary");
        sb.AppendLine("This report compares the performance of various search algorithms for the Peg Solitaire puzzle with and without hash verification. Hash verification helps detect symmetric board positions to avoid redundant exploration.\n");

        // Create summary table
        sb.AppendLine("### Performance Summary");
        sb.AppendLine("| Algorithm | Hash Verification | Success Rate | Avg Time (ms) | Avg Iterations | Avg Solution Depth |");
        sb.AppendLine("|-----------|-------------------|--------------|---------------|----------------|-------------------|");

        foreach (var result in _results)
        {
            string algoName = result.Key;
            var withHash = result.Value.WithHash;
            var withoutHash = result.Value.WithoutHash;

            // With hash
            sb.AppendLine($"| {algoName} | Enabled | {withHash.SuccessfulRuns}/{withHash.TotalRuns} ({(double)withHash.SuccessfulRuns / withHash.TotalRuns:P0}) | {(withHash.IterationTimes.Count > 0 ? withHash.IterationTimes.Average().ToString("F0") : "N/A")} | {(withHash.IterationCounts.Count > 0 ? withHash.IterationCounts.Average().ToString("F0") : "N/A")} | {(withHash.SolutionDepths.Count > 0 ? withHash.SolutionDepths.Average().ToString("F1") : "N/A")} |");

            // Without hash
            sb.AppendLine($"| {algoName} | Disabled | {withoutHash.SuccessfulRuns}/{withoutHash.TotalRuns} ({(double)withoutHash.SuccessfulRuns / withoutHash.TotalRuns:P0}) | {(withoutHash.IterationTimes.Count > 0 ? withoutHash.IterationTimes.Average().ToString("F0") : "N/A")} | {(withoutHash.IterationCounts.Count > 0 ? withoutHash.IterationCounts.Average().ToString("F0") : "N/A")} | {(withoutHash.SolutionDepths.Count > 0 ? withoutHash.SolutionDepths.Average().ToString("F1") : "N/A")} |");
        }

        sb.AppendLine("\n### Hash Verification Impact");
        sb.AppendLine("| Algorithm | Time Difference | Iteration Difference | Memory/State Savings |");
        sb.AppendLine("|-----------|-----------------|----------------------|---------------------|");

        foreach (var result in _results)
        {
            string algoName = result.Key;
            var withHash = result.Value.WithHash;
            var withoutHash = result.Value.WithoutHash;

            double avgTimeWithHash = withHash.IterationTimes.Count > 0 ? withHash.IterationTimes.Average() : 0;
            double avgTimeWithoutHash = withoutHash.IterationTimes.Count > 0 ? withoutHash.IterationTimes.Average() : 0;
            double timeDiff = avgTimeWithoutHash - avgTimeWithHash;
            string timeImpact = timeDiff > 0 ?
                $"{timeDiff:F0}ms faster with hash ({timeDiff / avgTimeWithoutHash:P0} improvement)" :
                $"{-timeDiff:F0}ms slower with hash ({-timeDiff / avgTimeWithHash:P0} degradation)";

            double avgIterWithHash = withHash.IterationCounts.Count > 0 ? withHash.IterationCounts.Average() : 0;
            double avgIterWithoutHash = withoutHash.IterationCounts.Count > 0 ? withoutHash.IterationCounts.Average() : 0;
            double iterDiff = avgIterWithoutHash - avgIterWithHash;
            string iterImpact = iterDiff > 0 ?
                $"{iterDiff:F0} fewer iterations with hash ({iterDiff / avgIterWithoutHash:P0} reduction)" :
                $"{-iterDiff:F0} more iterations with hash ({-iterDiff / avgIterWithHash:P0} increase)";

            // Simple calculation for memory savings - approximately how many states were skipped
            double memorySavings = iterDiff > 0 ? iterDiff : 0;
            string memoryImpact = memorySavings > 0 ?
                $"~{memorySavings:F0} states avoided" :
                "No memory savings";

            sb.AppendLine($"| {algoName} | {timeImpact} | {iterImpact} | {memoryImpact} |");
        }

        // Add detailed results for each algorithm
        sb.AppendLine("\n## Detailed Algorithm Results");

        foreach (var result in _results)
        {
            string algoName = result.Key;
            var withHash = result.Value.WithHash;
            var withoutHash = result.Value.WithoutHash;

            sb.AppendLine($"\n### {algoName}");

            // With hash verification
            sb.AppendLine("#### With Hash Verification");
            sb.AppendLine($"- **Success Rate**: {withHash.SuccessfulRuns}/{withHash.TotalRuns} ({(double)withHash.SuccessfulRuns / withHash.TotalRuns:P0})");
            if (withHash.IterationTimes.Count > 0)
            {
                sb.AppendLine($"- **Execution Time**: Avg = {withHash.IterationTimes.Average():F0}ms, Min = {withHash.IterationTimes.Min()}ms, Max = {withHash.IterationTimes.Max()}ms");
            }
            if (withHash.IterationCounts.Count > 0)
            {
                sb.AppendLine($"- **Iterations**: Avg = {withHash.IterationCounts.Average():F0}, Min = {withHash.IterationCounts.Min()}, Max = {withHash.IterationCounts.Max()}");
            }
            if (withHash.SolutionDepths.Count > 0)
            {
                sb.AppendLine($"- **Solution Depth**: Avg = {withHash.SolutionDepths.Average():F1}, Min = {withHash.SolutionDepths.Min()}, Max = {withHash.SolutionDepths.Max()}");
            }

            // Individual run details with hash
            sb.AppendLine("\n**Individual Runs:**");
            sb.AppendLine("| Run | Time (ms) | Iterations | Found Solution | Solution Depth |");
            sb.AppendLine("|-----|-----------|------------|----------------|----------------|");
            for (int i = 0; i < withHash.TotalRuns; i++)
            {
                bool foundSolution = i < withHash.SolutionDepths.Count;
                sb.AppendLine($"| {i + 1} | {withHash.IterationTimes[i]} | {withHash.IterationCounts[i]} | {(foundSolution ? "Yes" : "No")} | {(foundSolution ? withHash.SolutionDepths[i].ToString() : "N/A")} |");
            }

            // Without hash verification
            sb.AppendLine("\n#### Without Hash Verification");
            sb.AppendLine($"- **Success Rate**: {withoutHash.SuccessfulRuns}/{withoutHash.TotalRuns} ({(double)withoutHash.SuccessfulRuns / withoutHash.TotalRuns:P0})");
            if (withoutHash.IterationTimes.Count > 0)
            {
                sb.AppendLine($"- **Execution Time**: Avg = {withoutHash.IterationTimes.Average():F0}ms, Min = {withoutHash.IterationTimes.Min()}ms, Max = {withoutHash.IterationTimes.Max()}ms");
            }
            if (withoutHash.IterationCounts.Count > 0)
            {
                sb.AppendLine($"- **Iterations**: Avg = {withoutHash.IterationCounts.Average():F0}, Min = {withoutHash.IterationCounts.Min()}, Max = {withoutHash.IterationCounts.Max()}");
            }
            if (withoutHash.SolutionDepths.Count > 0)
            {
                sb.AppendLine($"- **Solution Depth**: Avg = {withoutHash.SolutionDepths.Average():F1}, Min = {withoutHash.SolutionDepths.Min()}, Max = {withoutHash.SolutionDepths.Max()}");
            }

            // Individual run details without hash
            sb.AppendLine("\n**Individual Runs:**");
            sb.AppendLine("| Run | Time (ms) | Iterations | Found Solution | Solution Depth |");
            sb.AppendLine("|-----|-----------|------------|----------------|----------------|");
            for (int i = 0; i < withoutHash.TotalRuns; i++)
            {
                bool foundSolution = i < withoutHash.SolutionDepths.Count;
                sb.AppendLine($"| {i + 1} | {withoutHash.IterationTimes[i]} | {withoutHash.IterationCounts[i]} | {(foundSolution ? "Yes" : "No")} | {(foundSolution ? withoutHash.SolutionDepths[i].ToString() : "N/A")} |");
            }

            // Comparison and analysis
            sb.AppendLine("\n#### Analysis");

            double avgTimeWithHash = withHash.IterationTimes.Count > 0 ? withHash.IterationTimes.Average() : 0;
            double avgTimeWithoutHash = withoutHash.IterationTimes.Count > 0 ? withoutHash.IterationTimes.Average() : 0;
            double timeDiff = avgTimeWithoutHash - avgTimeWithHash;

            double avgIterWithHash = withHash.IterationCounts.Count > 0 ? withHash.IterationCounts.Average() : 0;
            double avgIterWithoutHash = withoutHash.IterationCounts.Count > 0 ? withoutHash.IterationCounts.Average() : 0;
            double iterDiff = avgIterWithoutHash - avgIterWithHash;

            if (timeDiff > 0)
            {
                sb.AppendLine($"- Hash verification made the algorithm **{timeDiff:F0}ms faster** ({timeDiff / avgTimeWithoutHash:P0} improvement).");
            }
            else
            {
                sb.AppendLine($"- Hash verification made the algorithm **{-timeDiff:F0}ms slower** ({-timeDiff / avgTimeWithHash:P0} degradation).");
            }

            if (iterDiff > 0)
            {
                sb.AppendLine($"- With hash verification, the algorithm explored **{iterDiff:F0} fewer states** ({iterDiff / avgIterWithoutHash:P0} reduction).");
            }
            else
            {
                sb.AppendLine($"- With hash verification, the algorithm explored **{-iterDiff:F0} more states** ({-iterDiff / avgIterWithHash:P0} increase).");
            }

            sb.AppendLine($"- Success rate {(withHash.SuccessfulRuns > withoutHash.SuccessfulRuns ? "improved" : withHash.SuccessfulRuns < withoutHash.SuccessfulRuns ? "decreased" : "remained the same")} with hash verification.");
        }

        // Add conclusion
        sb.AppendLine("\n## Conclusion");

        // Calculate overall impact
        int betterWithHash = 0;
        int betterWithoutHash = 0;

        foreach (var result in _results)
        {
            var withHash = result.Value.WithHash;
            var withoutHash = result.Value.WithoutHash;

            double avgTimeWithHash = withHash.IterationTimes.Count > 0 ? withHash.IterationTimes.Average() : 0;
            double avgTimeWithoutHash = withoutHash.IterationTimes.Count > 0 ? withoutHash.IterationTimes.Average() : 0;

            if (avgTimeWithHash < avgTimeWithoutHash)
                betterWithHash++;
            else if (avgTimeWithHash > avgTimeWithoutHash)
                betterWithoutHash++;
        }

        sb.AppendLine($"Based on the performance analysis across {_results.Count} different search algorithms:");
        sb.AppendLine($"- **{betterWithHash}** algorithms performed better **with** hash verification");
        sb.AppendLine($"- **{betterWithoutHash}** algorithms performed better **without** hash verification");
        if (_results.Count - betterWithHash - betterWithoutHash > 0)
            sb.AppendLine($"- **{_results.Count - betterWithHash - betterWithoutHash}** algorithms showed no significant difference");

        sb.AppendLine("\n### Key Findings");
        sb.AppendLine("1. Hash verification generally reduces the number of states explored, which can lead to performance improvements in algorithms that explore many redundant states.");
        sb.AppendLine("2. For some algorithms (especially depth-limited ones), the overhead of hash computation and checking may outweigh the benefits of avoiding redundant states.");
        sb.AppendLine("3. The benefit of hash verification tends to increase with longer-running searches where more symmetric positions are encountered.");

        // Write the report to the file
        File.WriteAllText(_outputFilePath, sb.ToString());

        Console.WriteLine($"Performance report successfully generated at: {_outputFilePath}");
    }
}