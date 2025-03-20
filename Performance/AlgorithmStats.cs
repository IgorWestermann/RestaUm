namespace RestaUm.Performance;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

/// <summary>
/// Tracks performance statistics for algorithm executions
/// </summary>
public class AlgorithmStats
{
    /// <summary>
    /// The name of the algorithm
    /// </summary>
    public string AlgorithmName { get; private set; }

    /// <summary>
    /// Time elapsed for each iteration in milliseconds
    /// </summary>
    public List<long> IterationTimes { get; private set; } = new List<long>();

    /// <summary>
    /// Iteration count for each execution
    /// </summary>
    public List<int> IterationCounts { get; private set; } = new List<int>();

    /// <summary>
    /// Number of iterations that found a solution
    /// </summary>
    public int SuccessfulRuns { get; private set; } = 0;

    /// <summary>
    /// Total number of runs
    /// </summary>
    public int TotalRuns => IterationTimes.Count;

    /// <summary>
    /// Solution depths found (when solutions were discovered)
    /// </summary>
    public List<int> SolutionDepths { get; private set; } = new List<int>();

    /// <summary>
    /// Creates a new algorithm statistics tracker
    /// </summary>
    public AlgorithmStats(string algorithmName)
    {
        AlgorithmName = algorithmName;
    }

    /// <summary>
    /// Records a completed algorithm iteration
    /// </summary>
    public void RecordIteration(long timeElapsedMs, int iterationCount, bool foundSolution, int solutionDepth = 0)
    {
        IterationTimes.Add(timeElapsedMs);
        IterationCounts.Add(iterationCount);

        if (foundSolution)
        {
            SuccessfulRuns++;
            SolutionDepths.Add(solutionDepth);
        }
    }

    /// <summary>
    /// Prints a summary of algorithm performance statistics
    /// </summary>
    public void PrintSummary()
    {
        Console.WriteLine($"\n=== {AlgorithmName} Performance Summary ===");
        Console.WriteLine($"Total Runs: {TotalRuns}");
        Console.WriteLine($"Successful Runs: {SuccessfulRuns} ({(double)SuccessfulRuns / TotalRuns:P2})");

        if (IterationTimes.Count > 0)
        {
            Console.WriteLine($"Average Time: {IterationTimes.Average():F2}ms");
            Console.WriteLine($"Min Time: {IterationTimes.Min()}ms");
            Console.WriteLine($"Max Time: {IterationTimes.Max()}ms");
        }

        if (IterationCounts.Count > 0)
        {
            Console.WriteLine($"Average Iterations: {IterationCounts.Average():F2}");
            Console.WriteLine($"Min Iterations: {IterationCounts.Min()}");
            Console.WriteLine($"Max Iterations: {IterationCounts.Max()}");
        }

        if (SolutionDepths.Count > 0)
        {
            Console.WriteLine($"Average Solution Depth: {SolutionDepths.Average():F2}");
            Console.WriteLine($"Min Solution Depth: {SolutionDepths.Min()}");
            Console.WriteLine($"Max Solution Depth: {SolutionDepths.Max()}");
        }

        Console.WriteLine("=====================================");
    }
}