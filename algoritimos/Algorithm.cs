using RestaUm;
using RestaUm.Game;
using RestaUm.Helpers;
using System;
using System.Collections.Generic;

public class Algorithm
{
    public static bool AStar(int[,] initialBoard, int initialPegCount, bool checkForHashValues = false)
    {
        var queue = new PriorityQueue<State, int>();
        var visited = new HashSet<string>();
        var visitedHash = new HashSet<string>();

        var initialState = new State(initialBoard, initialPegCount, 0, Game.GenerateBoardHashes(initialBoard));
        queue.Enqueue(initialState, initialState.HeuristicValue);

        int iteration = 0;

        while (queue.Count > 0)
        {
            iteration++;
            var currentState = queue.Dequeue();

            if (currentState.PegCount == 1)
            {
                Console.WriteLine($"\n--- Iterations: {iteration} ---");
                Console.WriteLine("\nSolution found!");
                Helpers.PrintBoard(currentState.Board);

                return true;
            }

            string boardKey = Helpers.BoardToString(currentState.Board);
            string boardHash = Game.GenerateBoardHashes(currentState.Board);

            if (visited.Contains(boardKey))
                continue;
            visited.Add(boardKey);

            if (checkForHashValues && visitedHash.Contains(boardHash))
                continue;

            visitedHash.Add(boardHash);

            for (int x = 0; x < 7; x++)
            {
                for (int y = 0; y < 7; y++)
                {
                    foreach (var (dx, dy) in Game.directions)
                    {
                        if (Game.IsValidMove(currentState.Board, x, y, dx, dy))
                        {
                            var newBoard = (int[,])currentState.Board.Clone();
                            Game.MakeMove(newBoard, x, y, dx, dy);

                            int newPegCount = currentState.PegCount - 1;
                            string newBoardHash = Game.GenerateBoardHashes(newBoard);

                            var newState = new State(newBoard, newPegCount, currentState.MovesSoFar + 1, newBoardHash);
                            queue.Enqueue(newState, newState.HeuristicValue);
                        }
                    }
                }
            }

            if (iteration % 10 == 0)
                Console.Write("\r" + new string[] { "|", "/", "-", "\\" }[(iteration / 10) % 4] + $" Iterations: {iteration}");

        }

        Console.WriteLine("No solution found.");
        return false;
    }

    // Implementa��o do Algoritimo Guloso, com a Heuristic de contar pecas
    public static bool BestFirstSearch(int[,] initialBoard, bool checkForHashValues = false)
    {
        var frontier = new PriorityQueue<Node, int>();
        var explored = new HashSet<string>();
        var exploredHash = new HashSet<string>();

        var initialState = new Node(initialBoard, null, (0, 0, 0, 0), 0, Heuristica.CountPegs(initialBoard));
        int iteration = 0;

        frontier.Enqueue(initialState, initialState.HeuristicValue);

        while (frontier.Count > 0)
        {
            iteration++;
            var currentNode = frontier.Dequeue();

            if (Heuristica.CountPegs(currentNode.State!) == 1)
            {
                Console.WriteLine("Solution found!");
                Console.WriteLine($"\n--- Iterations: {iteration} ---");
                Helpers.PrintSolution(currentNode);
                return true;
            }

            string boardKey = Helpers.BoardToString(currentNode.State);
            string boardHash = Game.GenerateBoardHashes(currentNode.State);

            if (explored.Contains(boardKey))
                continue;

            if (checkForHashValues && exploredHash.Contains(boardHash))
                continue;

            explored.Add(boardKey);
            exploredHash.Add(boardHash);

            for (int x = 0; x < 7; x++)
            {
                for (int y = 0; y < 7; y++)
                {
                    foreach (var (dx, dy) in Game.directions)
                    {
                        if (Game.IsValidMove(currentNode.State, x, y, dx, dy))
                        {
                            var newBoard = (int[,])currentNode.State.Clone();
                            Game.MakeMove(newBoard, x, y, dx, dy);

                            int newPathCost = currentNode.PathCost + 2;
                            var newNode = new Node(newBoard, currentNode, (x, y, dx, dy), newPathCost, Heuristica.CountPegs(newBoard));
                            frontier.Enqueue(newNode, newNode.HeuristicValue);
                        }
                    }
                }
            }

            if (iteration % 10 == 0)
                Console.Write("\r" + new string[] { "|", "/", "-", "\\" }[(iteration / 10) % 4] + $" Iterations: {iteration}");
        }

        Console.WriteLine("No solution found.");
        return false;
    }

    // Implementa��o da Busca Ordenada 
    // Implementa��o da Busca Ordenada usando a heur�stica de centralidade com crit�rio de desempate baseado em mobilidade futura
    public static bool OrderedSearch(int[,] initialBoard, out Node rootNode)
    {
        // Calcula a heur�stica prim�ria: centralidade (soma das dist�ncias Manhattan dos pinos at� o centro)
        int initialCentrality = Heuristica.Centrality(initialBoard);
        // Cria o n� raiz com custo 0 e heur�stica definida pela centralidade
        rootNode = new Node(initialBoard, null, (0, 0, 0, 0), 0, initialCentrality);

        // Calcula a mobilidade futura para o estado inicial
        int initialFutureMobility = Helpers.FutureMobility(initialBoard);

        // A fila de prioridade utiliza uma tupla: (centralidade, mobilidade futura, custo do caminho)
        // Crit�rio de desempate:
        //   - Primeiro: centralidade (menor � melhor)
        //   - Segundo: mobilidade futura (menor indica menos movimentos dispon�veis e pode sinalizar converg�ncia)
        //   - Terceiro: custo do caminho (menor � melhor)
        var frontier = new PriorityQueue<Node, (int, int, int)>();
        var explored = new HashSet<string>();

        frontier.Enqueue(rootNode, (initialCentrality, initialFutureMobility, rootNode.PathCost));
        int iteration = 0;

        while (frontier.Count > 0)
        {
            iteration++;
            var currentNode = frontier.Dequeue();

            // Se restar apenas 1 pino, a solu��o foi encontrada
            if (Heuristica.CountPegs(currentNode.State) == 1)
            {
                Console.WriteLine("Solution found with Ordered Search using Centrality and Future Mobility tie-breaker!");
                Console.WriteLine($"\n--- Iterations: {iteration} ---");
                Helpers.PrintSolution(currentNode);
                return true;
            }

            string boardKey = Helpers.BoardToString(currentNode.State);
            if (explored.Contains(boardKey))
                continue;
            explored.Add(boardKey);

            // Expande os movimentos v�lidos a partir do estado atual
            for (int x = 0; x < 7; x++)
            {
                for (int y = 0; y < 7; y++)
                {
                    foreach (var (dx, dy) in Game.directions)
                    {
                        if (Game.IsValidMove(currentNode.State, x, y, dx, dy))
                        {
                            var newBoard = (int[,])currentNode.State.Clone();
                            Game.MakeMove(newBoard, x, y, dx, dy);

                            int newPathCost = currentNode.PathCost + 2;
                            // Nova heur�stica baseada na centralidade
                            int newCentrality = Heuristica.Centrality(newBoard);
                            // Calcula a mobilidade futura para o novo estado
                            int newFutureMobility = Helpers.FutureMobility(newBoard);

                            var newNode = new Node(newBoard, currentNode, (x, y, dx, dy), newPathCost, newCentrality);
                            // Adiciona o novo n� como filho do n� atual (para construir a �rvore de busca)
                            currentNode.Children.Add(newNode);

                            // Enfileira o novo n�: (centralidade, mobilidade futura, custo)
                            frontier.Enqueue(newNode, (newCentrality, newFutureMobility, newNode.PathCost));
                        }
                    }
                }
            }

            if (iteration % 10 == 0)
                Console.Write("\r" + new string[] { "|", "/", "-", "\\" }[(iteration / 10) % 4] + $" Iterations: {iteration}");
        }

        Console.WriteLine("No solution found with Ordered Search using Centrality and Future Mobility tie-breaker.");
        return false;
    }



    // Implementa��o da Busca A* com heur�stica de centralidade
    public static bool AStarCentrality(int[,] initialBoard, out Node rootNode)
    {
        // Estado inicial: custo 0 e heur�stica de centralidade
        rootNode = new Node(initialBoard, null, (0, 0, 0, 0), 0, Heuristica.Distance(initialBoard));

        // Fila de prioridade com prioridade definida por f(n) = g(n) + h(n)
        var frontier = new PriorityQueue<Node, int>();
        var explored = new HashSet<string>();

        // Enfileira o n� raiz com f(n)=PathCost + HeuristicValue
        frontier.Enqueue(rootNode, rootNode.PathCost + rootNode.HeuristicValue);

        int iteration = 0;
        while (frontier.Count > 0)
        {
            iteration++;
            var currentNode = frontier.Dequeue();

            // Se encontrar a solu��o (apenas 1 pino restante), imprime o caminho e retorna true
            if (Heuristica.CountPegs(currentNode.State) == 1)
            {
                Console.WriteLine("Solution found with A* (Centrality Heuristic)!");
                Console.WriteLine($"\n--- Iterations: {iteration} ---");
                Helpers.PrintSolution(currentNode);
                return true;
            }

            // Evita revisitar estados j� explorados
            string boardKey = Helpers.BoardToString(currentNode.State);
            if (explored.Contains(boardKey))
                continue;
            explored.Add(boardKey);

            // Expande os movimentos v�lidos a partir do estado atual
            for (int x = 0; x < 7; x++)
            {
                for (int y = 0; y < 7; y++)
                {
                    foreach (var (dx, dy) in Game.directions)
                    {
                        if (Game.IsValidMove(currentNode.State, x, y, dx, dy))
                        {
                            var newBoard = (int[,])currentNode.State.Clone();
                            Game.MakeMove(newBoard, x, y, dx, dy);

                            int newPathCost = currentNode.PathCost + 1; // custo definido para o movimento
                                                                        // Heur�stica de centralidade: soma das dist�ncias dos pinos ao centro
                            int newHeuristic = Heuristica.Distance(newBoard);
                            var newNode = new Node(newBoard, currentNode, (x, y, dx, dy), newPathCost, newHeuristic);

                            // Enfileira o novo n� usando f(n) = g(n) + h(n)
                            frontier.Enqueue(newNode, newNode.PathCost + newNode.HeuristicValue);
                        }
                    }
                }
            }

            if (iteration % 10 == 0)
                Console.Write("\r" + new string[] { "|", "/", "-", "\\" }[(iteration / 10) % 4] + $" Iterations: {iteration}");
        }

        Console.WriteLine("No solution found with A* (Centrality Heuristic).");
        return false;
    }


    // Implementa��o do Algoritimos Guloso com heur�stica de centralidade
    public static bool GreedySearch(int[,] initialBoard)
    {
        // A heur�stica � sempre definida como a centralidade:
        int initialHeuristic = Heuristica.Distance(initialBoard);
        var initialNode = new Node(initialBoard, null, (0, 0, 0, 0), 0, initialHeuristic);

        // Fila de prioridade onde a prioridade � dada apenas pela heur�stica (h(n)).
        var frontier = new PriorityQueue<Node, int>();
        var explored = new HashSet<string>();

        frontier.Enqueue(initialNode, initialNode.HeuristicValue);

        int iteration = 0;
        while (frontier.Count > 0)
        {
            iteration++;
            var currentNode = frontier.Dequeue();

            // Se o estado objetivo for atingido (apenas 1 pino restante), imprime a solu��o.
            if (Heuristica.CountPegs(currentNode.State) == 1)
            {
                Console.WriteLine("Solution found with Greedy Search (Centrality Heuristic)!");
                Console.WriteLine($"\n--- Iterations: {iteration} ---");
                Helpers.PrintSolution(currentNode);
                return true;
            }

            string boardKey = Helpers.BoardToString(currentNode.State);
            if (explored.Contains(boardKey))
                continue;
            explored.Add(boardKey);

            // Expande todos os movimentos v�lidos a partir do estado atual.
            for (int x = 0; x < 7; x++)
            {
                for (int y = 0; y < 7; y++)
                {
                    foreach (var (dx, dy) in Game.directions)
                    {
                        if (Game.IsValidMove(currentNode.State, x, y, dx, dy))
                        {
                            var newBoard = (int[,])currentNode.State.Clone();
                            Game.MakeMove(newBoard, x, y, dx, dy);

                            // Calcula a heur�stica para o novo estado.
                            int newHeuristic = Heuristica.Distance(newBoard);
                            int newPathCost = currentNode.PathCost + 2; // custo apenas para registro (n�o afeta a sele��o)

                            var newNode = new Node(newBoard, currentNode, (x, y, dx, dy), newPathCost, newHeuristic);

                            // Enfileira o novo n� usando apenas o valor da heur�stica.
                            frontier.Enqueue(newNode, newNode.HeuristicValue);
                        }
                    }
                }
            }

            if (iteration % 10 == 0)
                Console.Write("\r" + new string[] { "|", "/", "-", "\\" }[(iteration / 10) % 4] + $" Iterations: {iteration}");
        }

        Console.WriteLine("No solution found with Greedy Search (Centrality Heuristic).");
        return false;
    }

    // Implementa��o do A* com Heur�stica de Centralidade Ponderada.
    // f(n) = g(n) + weight * h(n), onde:
    //   - g(n) � o custo acumulado (PathCost)
    //   - h(n) � a heur�stica, definida aqui como a centralidade (soma das dist�ncias Manhattan dos pinos at� o centro)
    public static bool AStarWeightedCentrality(int[,] initialBoard, double weight)
    {
        // Calcula a heur�stica inicial (centralidade) para o tabuleiro
        int initialCentrality = Heuristica.Centrality(initialBoard);
        // Cria o n� raiz com custo 0 e heur�stica definida como a centralidade
        var rootNode = new Node(initialBoard, null, (0, 0, 0, 0), 0, initialCentrality);

        // Fila de prioridade utilizando double para acomodar a multiplica��o do peso na heur�stica
        var frontier = new PriorityQueue<Node, double>();
        var explored = new HashSet<string>();

        // A prioridade � f(n) = g(n) + weight * h(n)
        frontier.Enqueue(rootNode, rootNode.PathCost + weight * rootNode.HeuristicValue);

        int iteration = 0;
        while (frontier.Count > 0)
        {
            iteration++;
            var currentNode = frontier.Dequeue();

            // Verifica se a solu��o foi encontrada: apenas 1 pino restante
            if (Heuristica.CountPegs(currentNode.State) == 1)
            {
                Console.WriteLine("Solution found with AStarWeightedCentrality!");
                Console.WriteLine($"\n--- Iterations: {iteration} ---");
                Helpers.PrintSolution(currentNode);
                return true;
            }

            string boardKey = Helpers.BoardToString(currentNode.State);
            if (explored.Contains(boardKey))
                continue;
            explored.Add(boardKey);

            // Expande os movimentos v�lidos a partir do estado atual
            for (int x = 0; x < 7; x++)
            {
                for (int y = 0; y < 7; y++)
                {
                    foreach (var (dx, dy) in Game.directions)
                    {
                        if (Game.IsValidMove(currentNode.State, x, y, dx, dy))
                        {
                            var newBoard = (int[,])currentNode.State.Clone();
                            Game.MakeMove(newBoard, x, y, dx, dy);

                            int newPathCost = currentNode.PathCost + 2; // Incremento de custo por movimento
                            int newCentrality = Heuristica.Centrality(newBoard);
                            var newNode = new Node(newBoard, currentNode, (x, y, dx, dy), newPathCost, newCentrality);

                            // Enfileira o novo n� usando a f�rmula: f(n) = g(n) + weight * h(n)
                            frontier.Enqueue(newNode, newNode.PathCost + weight * newNode.HeuristicValue);
                        }
                    }
                }
            }

            if (iteration % 10 == 0)
                Console.Write("\r" + new string[] { "|", "/", "-", "\\" }[(iteration / 10) % 4] + $" Iterations: {iteration}");
        }

        Console.WriteLine("No solution found with AStarWeightedCentrality.");
        return false;
    }

    // Algoritmo de busca em largura


    //Algoritmo de busca em profundidade
    public static bool DepthFirstSearch(int[,] initialBoard, int initialPegCount)
    {
        // Pilha para armazenar os estados a serem explorados (DFS)
        var stack = new Stack<State>();

        // Conjunto para armazenar estados j� visitados e evitar repeti��es
        var visited = new HashSet<string>();

        // Estado inicial: tabuleiro, quantidade inicial de pinos e custo zero
        var initialState = new State(initialBoard, initialPegCount, 0);
        stack.Push(initialState);

        int iteration = 0;

        while (stack.Count > 0)
        {
            iteration++;
            var currentState = stack.Pop();

            // Verifica se a solu��o foi encontrada (apenas 1 pino restante)
            if (currentState.PegCount == 1)
            {
                Console.WriteLine($"\n--- Iterations: {iteration} ---");
                Console.WriteLine("\nSolution found!");
                Helpers.PrintBoard(currentState.Board);
                return true;
            }

            // Gera uma chave �nica para o estado atual
            string boardKey = Helpers.BoardToString(currentState.Board);
            if (visited.Contains(boardKey))
                continue;
            visited.Add(boardKey);

            // Expande os movimentos v�lidos a partir do estado atual
            for (int x = 0; x < 7; x++)
            {
                for (int y = 0; y < 7; y++)
                {
                    foreach (var (dx, dy) in Game.directions)
                    {
                        if (Game.IsValidMove(currentState.Board, x, y, dx, dy))
                        {
                            // Clona o tabuleiro e realiza o movimento
                            var newBoard = (int[,])currentState.Board.Clone();
                            Game.MakeMove(newBoard, x, y, dx, dy);
                            int newPegCount = currentState.PegCount - 1;

                            // Cria um novo estado com custo incrementado em 1 movimento
                            var newState = new State(newBoard, newPegCount, currentState.MovesSoFar + 1);
                            stack.Push(newState);
                        }
                    }
                }
            }

            if (iteration % 10 == 0)
                Console.Write("\r" + new string[] { "|", "/", "-", "\\" }[(iteration / 10) % 4] + $" Iterations: {iteration}");
        }

        Console.WriteLine("No solution found.");
        return false;
    }

    //Busca em Largura sem otimiza��o

    public static bool BreadthFirstSearch(int[,] initialBoard, int initialPegCount)
    {
        // Fila FIFO para armazenar os estados a serem explorados
        var queue = new Queue<State>();
        // Conjunto para armazenar estados j� visitados (usamos a representa��o em string do tabuleiro)
        var visited = new HashSet<string>();

        // Cria o estado inicial e adiciona-o imediatamente ao conjunto de visitados
        var initialState = new State(initialBoard, initialPegCount, 0);
        string initialKey = Helpers.BoardToString(initialBoard);
        visited.Add(initialKey);
        queue.Enqueue(initialState);

        int iteration = 0;

        while (queue.Count > 0)
        {
            iteration++;
            var currentState = queue.Dequeue();

            // Verifica se a solu��o foi encontrada: apenas 1 pino restante
            if (currentState.PegCount == 1)
            {
                Console.WriteLine($"\n--- Iterations: {iteration} ---");
                Console.WriteLine("\nSolution found!");
                Helpers.PrintBoard(currentState.Board);
                return true;
            }

            // Para cada posi��o do tabuleiro
            for (int x = 0; x < 7; x++)
            {
                for (int y = 0; y < 7; y++)
                {
                    // Para cada dire��o poss�vel
                    foreach (var (dx, dy) in Game.directions)
                    {
                        if (Game.IsValidMove(currentState.Board, x, y, dx, dy))
                        {
                            // Clona o tabuleiro e realiza o movimento
                            var newBoard = (int[,])currentState.Board.Clone();
                            Game.MakeMove(newBoard, x, y, dx, dy);
                            int newPegCount = currentState.PegCount - 1;

                            // Gera a chave do novo estado
                            string boardKey = Helpers.BoardToString(newBoard);
                            // S� enfileira se o estado n�o foi visitado ainda
                            if (!visited.Contains(boardKey))
                            {
                                visited.Add(boardKey);
                                var newState = new State(newBoard, newPegCount, currentState.MovesSoFar + 1);
                                queue.Enqueue(newState);
                            }
                        }
                    }
                }
            }

            if (iteration % 10 == 0)
                Console.Write("\r" + new string[] { "|", "/", "-", "\\" }[(iteration / 10) % 4] + $" Iterations: {iteration}");
        }

        Console.WriteLine("No solution found.");
        return false;
    }


    // Implementa��o da Busca Backtracking

    public static int iterationCount = 0;

    private static bool BacktrackingSearch(int[,] board, int pegCount, HashSet<string> visited)
    {
        // Incrementa o contador de itera��es a cada chamada recursiva
        iterationCount++;

        // Condi��o de sucesso: apenas 1 pino restante
        if (pegCount == 1)
        {
            Console.WriteLine("Solution found!");
            Helpers.PrintBoard(board);
            Console.WriteLine($"\n--- Iterations: {iterationCount} ---");
            return true;
        }

        // Evita reexplorar estados j� visitados
        string boardKey = Helpers.BoardToString(board);
        if (visited.Contains(boardKey))
            return false;
        visited.Add(boardKey);

        // Tenta cada movimento v�lido no tabuleiro
        for (int x = 0; x < 7; x++)
        {
            for (int y = 0; y < 7; y++)
            {
                foreach (var (dx, dy) in Game.directions)
                {
                    if (Game.IsValidMove(board, x, y, dx, dy))
                    {
                        var newBoard = (int[,])board.Clone();
                        Game.MakeMove(newBoard, x, y, dx, dy);

                        if (BacktrackingSearch(newBoard, pegCount - 1, visited))
                            return true;
                    }
                }
            }

            if (iterationCount % 10 == 0)
                Console.Write("\r" + new string[] { "|", "/", "-", "\\" }[(iterationCount / 10) % 4] + $" Iterations: {iterationCount}");
        }

        return false;
    }

    // Fun��o wrapper para iniciar a busca com backtracking
    public static bool SolveBacktracking(int[,] initialBoard, int initialPegCount)
    {
        iterationCount = 0; // Reseta o contador de itera��es
        var visited = new HashSet<string>();
        bool result = BacktrackingSearch(initialBoard, initialPegCount, visited);

        if (!result)
            Console.WriteLine($"\n--- Iterations: {iterationCount} ---");

        return result;
    }
}

// Busca em largura com bitMask e pr�-computa��o dos movimentos, pra tentar ser mais otimizado
public struct Move
{
    public int from;
    public int mid;
    public int to;
    public Move(int from, int mid, int to)
    {
        this.from = from;
        this.mid = mid;
        this.to = to;
    }
}

public struct StateBitmask
{
    public long Bitmask;
    public int PegCount;
    public int MovesSoFar;

    public StateBitmask(long bitmask, int pegCount, int movesSoFar)
    {
        Bitmask = bitmask;
        PegCount = pegCount;
        MovesSoFar = movesSoFar;
    }
}

public static class PegSolitaireOptimized
{
    // Lista de todos os movimentos poss�veis pr�-computados
    public static List<Move> PrecomputedMoves = GenerateMoves();

    // Gera a lista de movimentos v�lidos para um tabuleiro 7x7
    public static List<Move> GenerateMoves()
    {
        var moves = new List<Move>();
        int rows = 7, cols = 7;
        // Dire��es: cima, baixo, esquerda, direita
        int[,] directions = new int[,] { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } };

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                int fromIndex = i * cols + j;
                for (int d = 0; d < directions.GetLength(0); d++)
                {
                    int dx = directions[d, 0];
                    int dy = directions[d, 1];
                    int midI = i + dx;
                    int midJ = j + dy;
                    int toI = i + 2 * dx;
                    int toJ = j + 2 * dy;
                    if (midI >= 0 && midI < rows && midJ >= 0 && midJ < cols &&
                        toI >= 0 && toI < rows && toJ >= 0 && toJ < cols)
                    {
                        int midIndex = midI * cols + midJ;
                        int toIndex = toI * cols + toJ;
                        moves.Add(new Move(fromIndex, midIndex, toIndex));
                    }
                }
            }
        }
        return moves;
    }

    // Converte o tabuleiro (matriz 7x7) para uma bitmask (long)
    public static long BoardToBitmask(int[,] board)
    {
        long bitmask = 0;
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (board[i, j] == 1)
                    bitmask |= (1L << (i * cols + j));
            }
        }
        return bitmask;
    }

    // Algoritmo BFS otimizado utilizando bitmask e movimentos pr�-computados
    public static bool OptimizedBFSBitmask(int[,] initialBoard, int initialPegCount)
    {
        Queue<StateBitmask> queue = new Queue<StateBitmask>();
        HashSet<long> visited = new HashSet<long>();

        long initialMask = BoardToBitmask(initialBoard);
        visited.Add(initialMask);
        queue.Enqueue(new StateBitmask(initialMask, initialPegCount, 0));

        int iteration = 0;
        while (queue.Count > 0)
        {
            iteration++;
            var currentState = queue.Dequeue();

            if (currentState.PegCount == 1)
            {
                Console.WriteLine($"Solution found in {iteration} iterations with {currentState.MovesSoFar} moves.");
                // Opcional: converter o bitmask de volta para uma matriz para exibir o tabuleiro
                return true;
            }

            // Testa cada movimento pr�-computado
            foreach (var move in PrecomputedMoves)
            {
                // Verifica se o movimento � v�lido:
                // Deve existir pino na posi��o 'from' e 'mid', e a posi��o 'to' deve estar vazia.
                if (((currentState.Bitmask >> move.from) & 1L) == 1 &&
                    ((currentState.Bitmask >> move.mid) & 1L) == 1 &&
                    ((currentState.Bitmask >> move.to) & 1L) == 0)
                {
                    long newMask = currentState.Bitmask;
                    // Remove o pino da posi��o 'from' e da posi��o 'mid'
                    newMask &= ~(1L << move.from);
                    newMask &= ~(1L << move.mid);
                    // Coloca o pino na posi��o 'to'
                    newMask |= (1L << move.to);

                    if (!visited.Contains(newMask))
                    {
                        visited.Add(newMask);
                        queue.Enqueue(new StateBitmask(newMask, currentState.PegCount - 1, currentState.MovesSoFar + 1));
                    }
                }
            }
        }

        Console.WriteLine("No solution found.");
        return false;
    }
}
