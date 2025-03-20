using System.Security.Cryptography.X509Certificates;

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

    // Heuristic of conectivity of the board
    public static int Connectivity(int[,] board) {
        int connectivity = 0;
        for (int x = 0; x < 7; x++) {
            for (int y = 0; y < 7; y++) {
                if (board[x, y] == 1) {
                    if (x > 0 && board[x - 1, y] == 1)
                        connectivity++;
                    if (x < 6 && board[x + 1, y] == 1)
                        connectivity++;
                    if (y > 0 && board[x, y - 1] == 1)
                        connectivity++;
                    if (y < 6 && board[x, y + 1] == 1)
                        connectivity++;
                }
            }
        }
        return connectivity;
    }

    public static int Centrality(int[,] board) {
        int centerX = 3, centerY = 3;
        int distance = 0;
        int pegCount = CountPegs(board);

        // Se houver apenas um pino, retorne a dist�ncia dele at� o centro
        if (pegCount == 1) {
            for (int i = 0; i < 7; i++) {
                for (int j = 0; j < 7; j++) {
                    if (board[i, j] == 1)
                        return Math.Abs(i - centerX) + Math.Abs(j - centerY);
                }
            }
        }

        // Para m�ltiplos pinos, podemos usar a soma das dist�ncias
        for (int i = 0; i < 7; i++) {
            for (int j = 0; j < 7; j++) {
                if (board[i, j] == 1)
                    distance += Math.Abs(i - centerX) + Math.Abs(j - centerY);
            }
        }
        return distance;
    }

    public static int CentralityWhitPenality(int[,] board) {
        int centerX = 3, centerY = 3;
        int totalPenalty = 0;

        // Percorre o tabuleiro e soma a penaliza��o de cada pe�a.
        // A penaliza��o de cada pe�a � o quadrado da dist�ncia at� o centro.
        // Assim, pe�as mais perif�ricas ter�o um custo exponencialmente maior.
        for (int i = 0; i < 7; i++) {
            for (int j = 0; j < 7; j++) {
                if (board[i, j] == 1) {
                    int d = Math.Abs(i - centerX) + Math.Abs(j - centerY);
                    totalPenalty += d * d;
                }
            }
        }
        return totalPenalty;
    }
}


