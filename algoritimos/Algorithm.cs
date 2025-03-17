using RestaUm;
using RestaUm.helpers;
using RestaUm.Helpers;

public class Algorithm
{
    public static bool AStar(int[,] initialBoard, int initialPegCount)
    {
        var queue = new PriorityQueue<State, int>();

        var visited = new HashSet<string>();

        var initialState = new State(initialBoard, initialPegCount, 0);
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

            if (visited.Contains(boardKey))
            {
                continue;
            }

            visited.Add(boardKey);

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

                            var newState = new State(newBoard, newPegCount, currentState.MovesSoFar + 1);

                            //Deveria ser currentState.MovesSoFar + newState.HeuristicValue, tentei mudar mas estourava o tempo do algoritmo
                            queue.Enqueue(newState, newState.HeuristicValue);
                        }
                    }
                }
            }
        }

        Console.WriteLine("No solution found.");
        return false;
    }

    // Implementa��o do Algoritimo Guloso, com a Heuristic de contar pecas
    public static bool BestFirstSearch(int[,] initialBoard)
    {
        var frontier = new PriorityQueue<Node, int>();

        var explored = new HashSet<string>();

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
            if (explored.Contains(boardKey))
                continue;

            explored.Add(boardKey);

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
        }

        Console.WriteLine("No solution found.");
        return false;
    }

    // Implementa��o da Busca Ordenada 
    // Implementa��o da Busca Ordenada usando a heur�stica de centralidade com crit�rio de desempate baseado em mobilidade futura
    public static bool OrderedSearch(int[,] initialBoard, out Node rootNode) {
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

        while (frontier.Count > 0) {
            iteration++;
            var currentNode = frontier.Dequeue();

            // Se restar apenas 1 pino, a solu��o foi encontrada
            if (Heuristica.CountPegs(currentNode.State) == 1) {
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
            for (int x = 0; x < 7; x++) {
                for (int y = 0; y < 7; y++) {
                    foreach (var (dx, dy) in Game.directions) {
                        if (Game.IsValidMove(currentNode.State, x, y, dx, dy)) {
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
        }

        Console.WriteLine("No solution found with Ordered Search using Centrality and Future Mobility tie-breaker.");
        return false;
    }



    // Implementa��o da Busca A* com heur�stica de centralidade
    public static bool AStarCentrality(int[,] initialBoard, out Node rootNode) {
        // Estado inicial: custo 0 e heur�stica de centralidade
        rootNode = new Node(initialBoard, null, (0, 0, 0, 0), 0, Heuristica.Distance(initialBoard));

        // Fila de prioridade com prioridade definida por f(n) = g(n) + h(n)
        var frontier = new PriorityQueue<Node, int>();
        var explored = new HashSet<string>();

        // Enfileira o n� raiz com f(n)=PathCost + HeuristicValue
        frontier.Enqueue(rootNode, rootNode.PathCost + rootNode.HeuristicValue);

        int iteration = 0;
        while (frontier.Count > 0) {
            iteration++;
            var currentNode = frontier.Dequeue();

            // Se encontrar a solu��o (apenas 1 pino restante), imprime o caminho e retorna true
            if (Heuristica.CountPegs(currentNode.State) == 1) {
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
            for (int x = 0; x < 7; x++) {
                for (int y = 0; y < 7; y++) {
                    foreach (var (dx, dy) in Game.directions) {
                        if (Game.IsValidMove(currentNode.State, x, y, dx, dy)) {
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
        }

        Console.WriteLine("No solution found with A* (Centrality Heuristic).");
        return false;
    }


    // Implementa��o do Algoritimos Guloso com heur�stica de centralidade
    public static bool GreedySearch(int[,] initialBoard) {
        // A heur�stica � sempre definida como a centralidade:
        int initialHeuristic = Heuristica.Distance(initialBoard);
        var initialNode = new Node(initialBoard, null, (0, 0, 0, 0), 0, initialHeuristic);

        // Fila de prioridade onde a prioridade � dada apenas pela heur�stica (h(n)).
        var frontier = new PriorityQueue<Node, int>();
        var explored = new HashSet<string>();

        frontier.Enqueue(initialNode, initialNode.HeuristicValue);

        int iteration = 0;
        while (frontier.Count > 0) {
            iteration++;
            var currentNode = frontier.Dequeue();

            // Se o estado objetivo for atingido (apenas 1 pino restante), imprime a solu��o.
            if (Heuristica.CountPegs(currentNode.State) == 1) {
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
            for (int x = 0; x < 7; x++) {
                for (int y = 0; y < 7; y++) {
                    foreach (var (dx, dy) in Game.directions) {
                        if (Game.IsValidMove(currentNode.State, x, y, dx, dy)) {
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
        }

        Console.WriteLine("No solution found with Greedy Search (Centrality Heuristic).");
        return false;
    }

    // Implementa��o do A* com Heur�stica de Centralidade Ponderada.
    // f(n) = g(n) + weight * h(n), onde:
    //   - g(n) � o custo acumulado (PathCost)
    //   - h(n) � a heur�stica, definida aqui como a centralidade (soma das dist�ncias Manhattan dos pinos at� o centro)
    public static bool AStarWeightedCentrality(int[,] initialBoard, double weight) {
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
        while (frontier.Count > 0) {
            iteration++;
            var currentNode = frontier.Dequeue();

            // Verifica se a solu��o foi encontrada: apenas 1 pino restante
            if (Heuristica.CountPegs(currentNode.State) == 1) {
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
            for (int x = 0; x < 7; x++) {
                for (int y = 0; y < 7; y++) {
                    foreach (var (dx, dy) in Game.directions) {
                        if (Game.IsValidMove(currentNode.State, x, y, dx, dy)) {
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
        }

        Console.WriteLine("No solution found with AStarWeightedCentrality.");
        return false;
    }

    // Implementa��o da Busca Backtracking

    public static int iterationCount = 0;

    private static bool BacktrackingSearch(int[,] board, int pegCount, HashSet<string> visited) {
        // Incrementa o contador de itera��es a cada chamada recursiva
        iterationCount++;

        // Condi��o de sucesso: apenas 1 pino restante
        if (pegCount == 1) {
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
        for (int x = 0; x < 7; x++) {
            for (int y = 0; y < 7; y++) {
                foreach (var (dx, dy) in Game.directions) {
                    if (Game.IsValidMove(board, x, y, dx, dy)) {
                        var newBoard = (int[,])board.Clone();
                        Game.MakeMove(newBoard, x, y, dx, dy);

                        if (BacktrackingSearch(newBoard, pegCount - 1, visited))
                            return true;
                    }
                }
            }
        }

        return false;
    }

    // Fun��o wrapper para iniciar a busca com backtracking
    public static bool SolveBacktracking(int[,] initialBoard, int initialPegCount) {
        iterationCount = 0; // Reseta o contador de itera��es
        var visited = new HashSet<string>();
        bool result = BacktrackingSearch(initialBoard, initialPegCount, visited);

        if (!result)
            Console.WriteLine($"\n--- Iterations: {iterationCount} ---");

        return result;
    }

}