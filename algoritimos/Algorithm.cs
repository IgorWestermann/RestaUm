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

    // Implementação do Algoritimo Guloso, com a Heuristic de contar pecas
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

    // Implementação da Busca Ordenada 
    // Implementação da Busca Ordenada usando a heurística de centralidade com critério de desempate baseado em mobilidade futura
    public static bool OrderedSearch(int[,] initialBoard, out Node rootNode) {
        // Calcula a heurística primária: centralidade (soma das distâncias Manhattan dos pinos até o centro)
        int initialCentrality = Heuristica.Centrality(initialBoard);
        // Cria o nó raiz com custo 0 e heurística definida pela centralidade
        rootNode = new Node(initialBoard, null, (0, 0, 0, 0), 0, initialCentrality);

        // Calcula a mobilidade futura para o estado inicial
        int initialFutureMobility = Helpers.FutureMobility(initialBoard);

        // A fila de prioridade utiliza uma tupla: (centralidade, mobilidade futura, custo do caminho)
        // Critério de desempate:
        //   - Primeiro: centralidade (menor é melhor)
        //   - Segundo: mobilidade futura (menor indica menos movimentos disponíveis e pode sinalizar convergência)
        //   - Terceiro: custo do caminho (menor é melhor)
        var frontier = new PriorityQueue<Node, (int, int, int)>();
        var explored = new HashSet<string>();

        frontier.Enqueue(rootNode, (initialCentrality, initialFutureMobility, rootNode.PathCost));
        int iteration = 0;

        while (frontier.Count > 0) {
            iteration++;
            var currentNode = frontier.Dequeue();

            // Se restar apenas 1 pino, a solução foi encontrada
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

            // Expande os movimentos válidos a partir do estado atual
            for (int x = 0; x < 7; x++) {
                for (int y = 0; y < 7; y++) {
                    foreach (var (dx, dy) in Game.directions) {
                        if (Game.IsValidMove(currentNode.State, x, y, dx, dy)) {
                            var newBoard = (int[,])currentNode.State.Clone();
                            Game.MakeMove(newBoard, x, y, dx, dy);

                            int newPathCost = currentNode.PathCost + 2;
                            // Nova heurística baseada na centralidade
                            int newCentrality = Heuristica.Centrality(newBoard);
                            // Calcula a mobilidade futura para o novo estado
                            int newFutureMobility = Helpers.FutureMobility(newBoard);

                            var newNode = new Node(newBoard, currentNode, (x, y, dx, dy), newPathCost, newCentrality);
                            // Adiciona o novo nó como filho do nó atual (para construir a árvore de busca)
                            currentNode.Children.Add(newNode);

                            // Enfileira o novo nó: (centralidade, mobilidade futura, custo)
                            frontier.Enqueue(newNode, (newCentrality, newFutureMobility, newNode.PathCost));
                        }
                    }
                }
            }
        }

        Console.WriteLine("No solution found with Ordered Search using Centrality and Future Mobility tie-breaker.");
        return false;
    }



    // Implementação da Busca A* com heurística de centralidade
    public static bool AStarCentrality(int[,] initialBoard, out Node rootNode) {
        // Estado inicial: custo 0 e heurística de centralidade
        rootNode = new Node(initialBoard, null, (0, 0, 0, 0), 0, Heuristica.Distance(initialBoard));

        // Fila de prioridade com prioridade definida por f(n) = g(n) + h(n)
        var frontier = new PriorityQueue<Node, int>();
        var explored = new HashSet<string>();

        // Enfileira o nó raiz com f(n)=PathCost + HeuristicValue
        frontier.Enqueue(rootNode, rootNode.PathCost + rootNode.HeuristicValue);

        int iteration = 0;
        while (frontier.Count > 0) {
            iteration++;
            var currentNode = frontier.Dequeue();

            // Se encontrar a solução (apenas 1 pino restante), imprime o caminho e retorna true
            if (Heuristica.CountPegs(currentNode.State) == 1) {
                Console.WriteLine("Solution found with A* (Centrality Heuristic)!");
                Console.WriteLine($"\n--- Iterations: {iteration} ---");
                Helpers.PrintSolution(currentNode);
                return true;
            }

            // Evita revisitar estados já explorados
            string boardKey = Helpers.BoardToString(currentNode.State);
            if (explored.Contains(boardKey))
                continue;
            explored.Add(boardKey);

            // Expande os movimentos válidos a partir do estado atual
            for (int x = 0; x < 7; x++) {
                for (int y = 0; y < 7; y++) {
                    foreach (var (dx, dy) in Game.directions) {
                        if (Game.IsValidMove(currentNode.State, x, y, dx, dy)) {
                            var newBoard = (int[,])currentNode.State.Clone();
                            Game.MakeMove(newBoard, x, y, dx, dy);

                            int newPathCost = currentNode.PathCost + 1; // custo definido para o movimento
                                                                        // Heurística de centralidade: soma das distâncias dos pinos ao centro
                            int newHeuristic = Heuristica.Distance(newBoard);
                            var newNode = new Node(newBoard, currentNode, (x, y, dx, dy), newPathCost, newHeuristic);

                            // Enfileira o novo nó usando f(n) = g(n) + h(n)
                            frontier.Enqueue(newNode, newNode.PathCost + newNode.HeuristicValue);
                        }
                    }
                }
            }
        }

        Console.WriteLine("No solution found with A* (Centrality Heuristic).");
        return false;
    }


    // Implementação do Algoritimos Guloso com heurística de centralidade
    public static bool GreedySearch(int[,] initialBoard) {
        // A heurística é sempre definida como a centralidade:
        int initialHeuristic = Heuristica.Distance(initialBoard);
        var initialNode = new Node(initialBoard, null, (0, 0, 0, 0), 0, initialHeuristic);

        // Fila de prioridade onde a prioridade é dada apenas pela heurística (h(n)).
        var frontier = new PriorityQueue<Node, int>();
        var explored = new HashSet<string>();

        frontier.Enqueue(initialNode, initialNode.HeuristicValue);

        int iteration = 0;
        while (frontier.Count > 0) {
            iteration++;
            var currentNode = frontier.Dequeue();

            // Se o estado objetivo for atingido (apenas 1 pino restante), imprime a solução.
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

            // Expande todos os movimentos válidos a partir do estado atual.
            for (int x = 0; x < 7; x++) {
                for (int y = 0; y < 7; y++) {
                    foreach (var (dx, dy) in Game.directions) {
                        if (Game.IsValidMove(currentNode.State, x, y, dx, dy)) {
                            var newBoard = (int[,])currentNode.State.Clone();
                            Game.MakeMove(newBoard, x, y, dx, dy);

                            // Calcula a heurística para o novo estado.
                            int newHeuristic = Heuristica.Distance(newBoard);
                            int newPathCost = currentNode.PathCost + 2; // custo apenas para registro (não afeta a seleção)

                            var newNode = new Node(newBoard, currentNode, (x, y, dx, dy), newPathCost, newHeuristic);

                            // Enfileira o novo nó usando apenas o valor da heurística.
                            frontier.Enqueue(newNode, newNode.HeuristicValue);
                        }
                    }
                }
            }
        }

        Console.WriteLine("No solution found with Greedy Search (Centrality Heuristic).");
        return false;
    }

    // Implementação do A* com Heurística de Centralidade Ponderada.
    // f(n) = g(n) + weight * h(n), onde:
    //   - g(n) é o custo acumulado (PathCost)
    //   - h(n) é a heurística, definida aqui como a centralidade (soma das distâncias Manhattan dos pinos até o centro)
    public static bool AStarWeightedCentrality(int[,] initialBoard, double weight) {
        // Calcula a heurística inicial (centralidade) para o tabuleiro
        int initialCentrality = Heuristica.Centrality(initialBoard);
        // Cria o nó raiz com custo 0 e heurística definida como a centralidade
        var rootNode = new Node(initialBoard, null, (0, 0, 0, 0), 0, initialCentrality);

        // Fila de prioridade utilizando double para acomodar a multiplicação do peso na heurística
        var frontier = new PriorityQueue<Node, double>();
        var explored = new HashSet<string>();

        // A prioridade é f(n) = g(n) + weight * h(n)
        frontier.Enqueue(rootNode, rootNode.PathCost + weight * rootNode.HeuristicValue);

        int iteration = 0;
        while (frontier.Count > 0) {
            iteration++;
            var currentNode = frontier.Dequeue();

            // Verifica se a solução foi encontrada: apenas 1 pino restante
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

            // Expande os movimentos válidos a partir do estado atual
            for (int x = 0; x < 7; x++) {
                for (int y = 0; y < 7; y++) {
                    foreach (var (dx, dy) in Game.directions) {
                        if (Game.IsValidMove(currentNode.State, x, y, dx, dy)) {
                            var newBoard = (int[,])currentNode.State.Clone();
                            Game.MakeMove(newBoard, x, y, dx, dy);

                            int newPathCost = currentNode.PathCost + 2; // Incremento de custo por movimento
                            int newCentrality = Heuristica.Centrality(newBoard);
                            var newNode = new Node(newBoard, currentNode, (x, y, dx, dy), newPathCost, newCentrality);

                            // Enfileira o novo nó usando a fórmula: f(n) = g(n) + weight * h(n)
                            frontier.Enqueue(newNode, newNode.PathCost + weight * newNode.HeuristicValue);
                        }
                    }
                }
            }
        }

        Console.WriteLine("No solution found with AStarWeightedCentrality.");
        return false;
    }

    // Implementação da Busca Backtracking

    public static int iterationCount = 0;

    private static bool BacktrackingSearch(int[,] board, int pegCount, HashSet<string> visited) {
        // Incrementa o contador de iterações a cada chamada recursiva
        iterationCount++;

        // Condição de sucesso: apenas 1 pino restante
        if (pegCount == 1) {
            Console.WriteLine("Solution found!");
            Helpers.PrintBoard(board);
            Console.WriteLine($"\n--- Iterations: {iterationCount} ---");
            return true;
        }

        // Evita reexplorar estados já visitados
        string boardKey = Helpers.BoardToString(board);
        if (visited.Contains(boardKey))
            return false;
        visited.Add(boardKey);

        // Tenta cada movimento válido no tabuleiro
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

    // Função wrapper para iniciar a busca com backtracking
    public static bool SolveBacktracking(int[,] initialBoard, int initialPegCount) {
        iterationCount = 0; // Reseta o contador de iterações
        var visited = new HashSet<string>();
        bool result = BacktrackingSearch(initialBoard, initialPegCount, visited);

        if (!result)
            Console.WriteLine($"\n--- Iterations: {iterationCount} ---");

        return result;
    }

}