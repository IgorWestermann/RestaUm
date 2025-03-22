

namespace RestaUm.helpers
{

    using System;
    using System.Drawing;
    using System.IO;


    public class PegSolitaireImageGenerator
    {
        private const int CellSize = 50; // Size of each cell in pixels
        private readonly string outputDirectory;

        public PegSolitaireImageGenerator(string outputDirectory)
        { 
            this.outputDirectory = outputDirectory.Replace("*", "star").Replace(" ", "_");
            Directory.CreateDirectory(this.outputDirectory);
        }

        public void GenerateImage(int[,] board, int step)
        {



            string filename = $"PegSolitaire_{step:D3}.png"; // e.g., PegSolitaire_001.png
            string filepath = Path.Combine(outputDirectory, filename);
            DrawBoardImage(board, filepath, step);

        }

        private void DrawBoardImage(int[,] board, string filepath, int step)
        {
            int rows = board.GetLength(0);
            int cols = board.GetLength(1);
            int width = cols * CellSize;
            int height = rows * CellSize;

            using (Bitmap bmp = new Bitmap(width, height))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);

                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < cols; col++)
                    {
                        if (board[row, col] != -1)
                        {
                            int x = col * CellSize;
                            int y = row * CellSize;

                            // Draw hole
                            g.FillEllipse(Brushes.Gray, x + 5, y + 5, CellSize - 10, CellSize - 10);

                            // If there's a peg (1), draw it
                            if (board[row, col] == 1)
                            {
                                g.FillEllipse(Brushes.DarkBlue, x + 10, y + 10, CellSize - 20, CellSize - 20);
                            }
                        }
                    }
                }

                // Save the image
                bmp.Save(filepath, System.Drawing.Imaging.ImageFormat.Png);
            }
        }
    }
}