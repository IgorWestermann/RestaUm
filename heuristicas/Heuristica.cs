public class Heuristica
{
    public static int CountPegs(int[,] board)
    {
        int count = 0;
        for (int i = 0; i < 7; i++)
            for (int j = 0; j < 7; j++)
                if (board[i, j] == 1)
                    count++;
        return count;
    }

    public static int Distance(int[,] board)
    {
        int centerX = 3, centerY = 3;
        int totalDistance = 0;

        for (int x = 0; x < 7; x++)
        {
            for (int y = 0; y < 7; y++)
            {
                if (board[x, y] == 1)
                {
                    int dx = Math.Abs(x - centerX);
                    int dy = Math.Abs(y - centerY);
                    totalDistance += dx + dy;
                }
            }
        }

        return totalDistance;
    }
}
