namespace RestaUm.Game;

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

    public static string GenerateBoardHashes(int[,] board, bool includeMirrors = true)
    {
        List<int[,]> transformations = new List<int[,]>();

        // Add original board
        transformations.Add(CloneBoard(board));

        // Add rotations: 90°, 180°, 270°
        int[,] rotated = CloneBoard(board);
        for (int i = 0; i < 3; i++)
        {
            rotated = RotateBoard90(rotated);
            transformations.Add(CloneBoard(rotated));
        }

        // Horizontal mirror (left-right)
        transformations.Add(MirrorHorizontal(board));
        // Vertical mirror (top-bottom)
        transformations.Add(MirrorVertical(board));
        // Add mirrors if requested
        if (includeMirrors)
        {
            // Rotations of horizontal mirror
            rotated = MirrorHorizontal(board);
            for (int i = 0; i < 3; i++)
            {
                rotated = RotateBoard90(rotated);
                transformations.Add(CloneBoard(rotated));
            }
        }

        // Compute MD5 hashes for each transformation
        List<string> hashes = new List<string>();
        foreach (var transformation in transformations)
        {
            string boardString = BoardToString(transformation);
            string hash = ComputeMD5(boardString);
            hashes.Add(hash);
        }

        // Return the minimum hash
        return hashes.Min();
    }

    private static int[,] CloneBoard(int[,] board)
    {
        int[,] clone = new int[7, 7];
        for (int i = 0; i < 7; i++)
            for (int j = 0; j < 7; j++)
                clone[i, j] = board[i, j];
        return clone;
    }

    private static int[,] RotateBoard90(int[,] board)
    {
        int[,] rotated = new int[7, 7];
        for (int i = 0; i < 7; i++)
            for (int j = 0; j < 7; j++)
                rotated[j, 6 - i] = board[i, j];
        return rotated;
    }

    private static int[,] MirrorHorizontal(int[,] board)
    {
        int[,] mirrored = new int[7, 7];
        for (int i = 0; i < 7; i++)
            for (int j = 0; j < 7; j++)
                mirrored[i, 6 - j] = board[i, j];
        return mirrored;
    }

    private static int[,] MirrorVertical(int[,] board)
    {
        int[,] mirrored = new int[7, 7];
        for (int i = 0; i < 7; i++)
            for (int j = 0; j < 7; j++)
                mirrored[6 - i, j] = board[i, j];
        return mirrored;
    }

    private static string ComputeMD5(string input)
    {
        using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
        {
            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
