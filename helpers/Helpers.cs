namespace RestaUm.Helpers
{
    public class Helpers
    {
        public static string BoardToString(int[,] board)
        {
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < 7; i++)
                for (int j = 0; j < 7; j++)
                    sb.Append(board[i, j]);
            return sb.ToString();
        }

        public static void PrintBoard(int[,] board)
        {
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    Console.Write(board[i, j] == -1 ? " " : board[i, j].ToString());
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
        }

        public static void PrintSolution(List<Node> path)
        {
            Console.WriteLine("Solution Path:");
            foreach (var node in path)
            {
                PrintBoard(node.Board);
                Console.WriteLine();
            }
        }
    }
}