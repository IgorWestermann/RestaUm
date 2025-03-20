using RestaUm.Helpers;
using RestaUm;
using RestaUm.Game;
using RestaUm.Algorithms;

int[,] board = new int[7, 7]
{
    { -1, -1,  1,  1,  1, -1, -1 },
    { -1, -1,  1,  1,  1, -1, -1 },
    {  1,  1,  1,  1,  1,  1,  1 },
    {  1,  1,  1,  0,  1,  1,  1 },
    {  1,  1,  1,  1,  1,  1,  1 },
    { -1, -1,  1,  1,  1, -1, -1 },
    { -1, -1,  1,  1,  1, -1, -1 }
};

Helpers.PrintBoard(board);

int pegCount = Heuristica.CountPegs(board);
int distance = Heuristica.Distance(board);
Node root = null;

// Original algorithms
//Console.WriteLine("Running original implementations...");
//Console.WriteLine("---------------------------------");

//Console.WriteLine("Heuristica CountPegs");
//Algorithm.AStar(board, pegCount, true);

//Console.WriteLine("Guloso com Heuristica CountPegs");
//Algorithm.BestFirstSearch(board);

//Console.WriteLine("Heuristica Centralidade, para o Aloritimo A*");
//Algorithm.AStarCentrality(board, out root);

//Console.WriteLine("Heuristica Centralidade, para o Aloritimo A* com peso `infinito`");
//Algorithm.AStarWeightedCentrality(board, Double.MaxValue);

//Console.WriteLine("heuristica centralidade, para o aloritimo ordenada");
//Algorithm.OrderedSearch(board, out root);

//Console.WriteLine("Heuristica Centralidade, para o Aloritimo Gulosa");
//Algorithm.GreedySearch(board);

//Console.WriteLine("Busca Backtracking");
//Algorithm.SolveBacktracking(board, pegCount);

//Console.WriteLine("Algoritmo de profundidade");
//Algorithm.DepthFirstSearch(board, pegCount);

//Console.WriteLine("Algoritmo de largura");
//Algorithm.BreadthFirstSearch(board, pegCount);

//Console.WriteLine("Algoritmo de largura otimizado");
//PegSolitaireOptimized.OptimizedBFSBitmask(board, pegCount);

//Test new optimized implementations(uncomment to use)

Console.WriteLine("\nRunning new optimized implementations...");
Console.WriteLine("---------------------------------");

 //A* with default CountPegs heuristic
Console.WriteLine("New A* with CountPegs heuristic:");
NewAlgorithm.AStar(board, pegCount, true);

 //Best First Search with Centrality heuristic
Console.WriteLine("New Best First Search with Centrality heuristic:");
NewAlgorithm.BestFirstSearch(board, false, Heuristica.Centrality);

 //A* with weighted Centrality heuristic
Console.WriteLine("New A* with weighted Centrality heuristic:");
NewAlgorithm.AStarWeightedHeuristic(board, 1.5, Heuristica.Centrality);

 //Ordered Search with Centrality as primary and FutureMobility as secondary heuristic
Console.WriteLine("New Ordered Search with multiple heuristics:");
NewAlgorithm.OrderedSearch(board);

 //Breadth First Search
//Console.WriteLine("New Breadth First Search:");
//NewAlgorithm.BreadthFirstSearch(board, pegCount);

 //Depth First Search
Console.WriteLine("New Depth First Search:");
NewAlgorithm.DepthFirstSearch(board, pegCount);

 //Backtracking Search
Console.WriteLine("New Backtracking Search:");
NewAlgorithm.SolveBacktracking(board, pegCount);
