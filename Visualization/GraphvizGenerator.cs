using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using RestaUm.Game;
using RestaUm.States;

namespace RestaUm.Visualization
{
    public class GraphvizGenerator
    {
        private const int MaxNodesToRender = 1000000; // Limit for large graphs

        /// <summary>
        /// Generates a DOT file for the full solution tree
        /// </summary>
        public static void GenerateSolutionTree(string algorithmName, Dictionary<string, GameState> allStates, string solutionPath = null)
        {

            algorithmName = algorithmName.Replace("*", "star").Replace(" ", "_");

            string filename = $"reports/tree_{algorithmName}_{DateTime.Now:yyyyMMdd_HHmmss}.dot";
            Directory.CreateDirectory("reports");

            using (StreamWriter writer = new StreamWriter(filename))
            {
                writer.WriteLine("digraph G {");
                writer.WriteLine("  rankdir=LR;");
                writer.WriteLine("  node [shape=box, style=filled, fillcolor=lightblue];");

                // Check if graph is too large
                if (allStates.Count > MaxNodesToRender)
                {
                    writer.WriteLine("  // Graph too large - rendering only solution path");
                    if (solutionPath != null)
                    {
                        WriteSolutionPathEdges(writer, solutionPath);
                    }
                    else
                    {
                        writer.WriteLine("  // No solution path provided");
                    }
                }
                else
                {
                    // Write all nodes first
                    foreach (var state in allStates.Values)
                    {
                        WriteStateNode(writer, state);
                    }

                    // Write all edges
                    foreach (var state in allStates.Values)
                    {
                        if (state.Parent != null)
                        {
                            writer.WriteLine($"  \"{GetNodeId(state.Parent)}\" -> \"{GetNodeId(state)}\" [color=gray];");
                        }
                    }

                    // Highlight solution path if provided
                    if (solutionPath != null)
                    {
                        WriteSolutionPathEdges(writer, solutionPath);
                    }
                }

                writer.WriteLine("}");
            }

            Console.WriteLine($"Solution tree generated: {filename}");
        }

        /// <summary>
        /// Generates a DOT file only for the solution path
        /// </summary>
        public static void GenerateSolutionPath(string algorithmName, GameState solutionState)
        {


            algorithmName = algorithmName.Replace("*", "star").Replace(" ", "_");

            if (solutionState == null)
            {
                Console.WriteLine("No solution state provided");
                return;
            }

            string filename = $"reports/path_{algorithmName}_{DateTime.Now:yyyyMMdd_HHmmss}.dot";
            Directory.CreateDirectory("reports");

            // Build the solution path
            List<GameState> solutionPath = new List<GameState>();
            GameState current = solutionState;

            while (current != null)
            {
                solutionPath.Add(current);
                current = current.Parent;
            }

            // Reverse to get start-to-goal order
            solutionPath.Reverse();

            using (StreamWriter writer = new StreamWriter(filename))
            {
                writer.WriteLine("digraph G {");
                writer.WriteLine("  rankdir=LR;");
                writer.WriteLine("  node [shape=box, style=filled, fillcolor=lightgreen];");

                // Write nodes
                foreach (var state in solutionPath)
                {
                    WriteStateNode(writer, state);
                }

                // Write edges
                for (int i = 0; i < solutionPath.Count - 1; i++)
                {
                    writer.WriteLine($"  \"{GetNodeId(solutionPath[i])}\" -> \"{GetNodeId(solutionPath[i + 1])}\" [color=red, penwidth=2.0];");
                }

                writer.WriteLine("}");
            }

            Console.WriteLine($"Solution path generated: {filename}");
        }

        /// <summary>
        /// Helper method to get a unique node ID
        /// </summary>
        private static string GetNodeId(GameState state)
        {
            return state.GetHashCode().ToString();
        }

        /// <summary>
        /// Helper method to write a state node with its board representation
        /// </summary>
        private static void WriteStateNode(StreamWriter writer, GameState state)
        {
            string label = $"Pegs: {state.PegCount}\\nLevel: {state.Level}";

            // For small boards, include a text representation
            StringBuilder boardText = new StringBuilder();
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    boardText.Append(state.Board[i, j] switch
                    {
                        -1 => " ",
                        0 => ".",
                        1 => "O",
                        _ => "?"
                    });
                }
                boardText.Append("\\n");
            }

            writer.WriteLine($"  \"{GetNodeId(state)}\" [label=\"{label}\\n{boardText}\"];");
        }

        /// <summary>
        /// Helper method to write solution path edges
        /// </summary>
        private static void WriteSolutionPathEdges(StreamWriter writer, string solutionPath)
        {
            // This would parse a string representation of the path and draw edges
            // For now, leaving as a placeholder for implementation with actual solution path
            writer.WriteLine("  // Solution path would be highlighted here");
        }
    }
}