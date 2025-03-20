namespace RestaUm.helpers;

public class Game
{
    public static List<(int, int)> directions =
    [
        (0, 2),  // Right
        (0, -2), // Left
        (2, 0),  // Down
        (-2, 0)  // Up
    ];

    public static bool IsValidMove(int[,] board, int x, int y, int dx, int dy)
    {
        int midX = x + dx / 2;
        int midY = y + dy / 2;
        int newX = x + dx;
        int newY = y + dy;

        if (newX < 0 || newX >= 7 || newY < 0 || newY >= 7)
            return false;

        return board[x, y] == 1 && board[midX, midY] == 1 && board[newX, newY] == 0;
    }

    public static void MakeMove(int[,] board, int x, int y, int dx, int dy)
    {
        int midX = x + dx / 2;
        int midY = y + dy / 2;
        int newX = x + dx;
        int newY = y + dy;

        board[x, y] = 0;
        board[midX, midY] = 0;
        board[newX, newY] = 1;
    }

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
    public static bool IsGoalState(int[,] board)
    {
        return CountPegs(board) == 1;
    }

    public static bool IsValidMove(int[,] state, int i, int j, int x, int y, int x2, int y2)
    {
        int rows = state.GetLength(0);
        int cols = state.GetLength(1);

        if (x2 < 0 || x2 >= rows || y2 < 0 || y2 >= cols)
            return false;

        return state[i, j] == 1 && state[x, y] == 1 && state[x2, y2] == 0; ;
    }
    public static int CountPegs(int[,] board)
    {
        int count = 0;
        for (int i = 0; i < board.GetLength(0); i++)
            for (int j = 0; j < board.GetLength(1); j++)
                if (board[i, j] == 1)
                    count++;
        return count;
    }

    public static string StateToString(int[,] state)
    {
        return string.Join(",", state.Cast<int>());
    }
}
