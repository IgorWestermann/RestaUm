

namespace RestaUm.Algorithms
{
    using RestaUm.Game;
    using RestaUm.Helpers;
    using System;
    using System.Collections.Generic;
    public class PegSolitaireOptimized
    {
        private static readonly int[] dx = { 0, 0, 2, -2 };  // Right, Left, Down, Up
        private static readonly int[] dy = { 2, -2, 0, 0 };  // Right, Left, Down, Up

        /// <summary>
        /// Converts a board state to a bitmask representation
        /// </summary>
        private static ulong BoardToBitmask(int[,] board)
        {
            ulong bitmask = 0;
            int bit = 0;
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (board[i, j] == 1)
                    {
                        bitmask |= 1UL << bit;
                    }
                    bit++;
                }
            }
            return bitmask;
        }

        /// <summary>
        /// Converts a bitmask back to a board state
        /// </summary>
        private static int[,] BitmaskToBoard(ulong bitmask)
        {
            int[,] board = new int[7, 7];
            int bit = 0;
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if ((bitmask & (1UL << bit)) != 0)
                    {
                        board[i, j] = 1;
                    }
                    else
                    {
                        // Preserve the standard board structure with -1 for invalid positions
                        board[i, j] = (i < 2 || i > 4) && (j < 2 || j > 4) ? -1 : 0;
                    }
                    bit++;
                }
            }
            return board;
        }

        /// <summary>
        /// Checks if a move is valid in the bitmask representation
        /// </summary>
        private static bool IsValidMoveBitmask(ulong bitmask, int x, int y, int dx, int dy)
        {
            int midX = x + dx / 2;
            int midY = y + dy / 2;
            int newX = x + dx;
            int newY = y + dy;

            if (newX < 0 || newX >= 7 || newY < 0 || newY >= 7)
                return false;

            int startBit = x * 7 + y;
            int midBit = midX * 7 + midY;
            int endBit = newX * 7 + newY;

            return ((bitmask & (1UL << startBit)) != 0) &&
                   ((bitmask & (1UL << midBit)) != 0) &&
                   ((bitmask & (1UL << endBit)) == 0);
        }

        /// <summary>
        /// Makes a move in the bitmask representation
        /// </summary>
        private static ulong MakeMoveBitmask(ulong bitmask, int x, int y, int dx, int dy)
        {
            int midX = x + dx / 2;
            int midY = y + dy / 2;
            int newX = x + dx;
            int newY = y + dy;

            int startBit = x * 7 + y;
            int midBit = midX * 7 + midY;
            int endBit = newX * 7 + newY;

            return (bitmask & ~(1UL << startBit)) & ~(1UL << midBit) | (1UL << endBit);
        }

        /// <summary>
        /// Optimized Breadth First Search using bitmasks for state representation
        /// </summary>
        public static bool OptimizedBFSBitmask(int[,] initialBoard, int initialPegCount)
        {
            var queue = new Queue<(ulong bitmask, int pegCount, int moves)>();
            var visited = new HashSet<ulong>();

            ulong initialBitmask = BoardToBitmask(initialBoard);
            visited.Add(initialBitmask);
            queue.Enqueue((initialBitmask, initialPegCount, 0));

            int iteration = 0;

            while (queue.Count > 0)
            {
                iteration++;
                var (currentBitmask, currentPegCount, currentMoves) = queue.Dequeue();

                if (currentPegCount == 1)
                {
                    Console.WriteLine($"\nSolution found with Optimized BFS (Bitmask)!");
                    Console.WriteLine($"--- Iterations: {iteration} ---");
                    int[,] solutionBoard = BitmaskToBoard(currentBitmask);
                    Helpers.PrintBoard(solutionBoard);
                    Console.WriteLine($"Pontos Solução: {Helpers.caculePoints(solutionBoard)}");
                    return true;
                }

                // Generate all possible moves
                for (int x = 0; x < 7; x++)
                {
                    for (int y = 0; y < 7; y++)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            if (IsValidMoveBitmask(currentBitmask, x, y, dx[i], dy[i]))
                            {
                                ulong newBitmask = MakeMoveBitmask(currentBitmask, x, y, dx[i], dy[i]);

                                if (!visited.Contains(newBitmask))
                                {
                                    visited.Add(newBitmask);
                                    queue.Enqueue((newBitmask, currentPegCount - 1, currentMoves + 1));
                                }
                            }
                        }
                    }
                }

                if (iteration % 10 == 0)
                    Console.Write("\r" + new string[] { "|", "/", "-", "\\" }[(iteration / 10) % 4] + $" Iterations: {iteration}");
            }

            Console.WriteLine("\nNo solution found.");
            return false;
        }
    }
}