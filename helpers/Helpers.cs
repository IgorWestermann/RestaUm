using RestaUm.helpers;

namespace RestaUm.Helpers;

public class Helpers
{
    private static readonly int[] dx = { -1, 1, 0, 0 };
    private static readonly int[] dy = { 0, 0, -1, 1 };
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

    public static void PrintSolution(Node node)
    {
        var path = new List<Node>();
        while (node != null)
        {
            path.Add(node);
            node = node.Parent;
        }

        path.Reverse();

        Console.WriteLine("Solution Path:");
        foreach (var n in path)
        {
            Console.WriteLine($"Move: {n.Action}");
            PrintBoard(n.State);
            Console.WriteLine();
        }
    }

    // Método para exportar a árvore de busca para um arquivo DOT
    public static void ExportSearchTreeToDOT(Node root, string filePath)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine("digraph SearchTree {");
            writer.WriteLine("    node [shape=box, style=filled, color=lightblue];");

            int idCounter = 0;
            ExportNodeDOT(root, writer, ref idCounter);

            writer.WriteLine("}");
        }
        Console.WriteLine($"Arquivo DOT criado em: {filePath}");
    }

    // Método auxiliar recursivo para escrever cada nó e as arestas correspondentes
    private static int ExportNodeDOT(Node node, StreamWriter writer, ref int idCounter)
    {
        int currentId = idCounter;
        idCounter++;

        // Cria o rótulo do nó com informações importantes (você pode personalizar)
        string label = $"Move: {node.Action} | Cost: {node.PathCost} | Heur: {node.HeuristicValue}";
        writer.WriteLine($"    Node{currentId} [label=\"{label}\"];");

        // Para cada filho do nó atual, escreve a aresta e chama recursivamente
        foreach (var child in node.Children)
        {
            int childId = ExportNodeDOT(child, writer, ref idCounter);
            writer.WriteLine($"    Node{currentId} -> Node{childId};");
        }
        return currentId;
    }

    //Metodo que retorna a quantidade de movimentos possiveis do tabuleiro
    public static int FutureMobility(int[,] board)
    {
        int mobility = 0;
        for (int x = 0; x < 7; x++)
        {
            for (int y = 0; y < 7; y++)
            {
                foreach (var (dx, dy) in Game.directions)
                {
                    if (Game.IsValidMove(board, x, y, dx, dy))
                        mobility++;
                }
            }
        }
        return mobility;
    }

    //Metodo que retorna pontos para a quantidade de pinos no tabuleiro
    public static int caculePoints(int[,] board)
    {
        int pegCount = 0;
        int distToCenter = 0;

        for (int x = 0; x < 7; x++)
        {
            for (int y = 0; y < 7; y++)
            {
                if (board[x, y] == 1)
                {
                    pegCount++;
                    distToCenter += Math.Abs(x - 3) + Math.Abs(y - 3);
                }
            }
        }
        return Math.Abs(pegCount + distToCenter - 121);

    }
}
