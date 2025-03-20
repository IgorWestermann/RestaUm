using System;
using RestaUm.Helpers;
using RestaUm;
using RestaUm.Game;
using RestaUm.Algorithms;
using System.IO;
using RestaUm.Performance;

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
Console.WriteLine("\n");

int pegCount = Heuristica.CountPegs(board);
int distance = Heuristica.Distance(board);
Node root = null;


RunPerformanceAnalysis.Run();

//ExecutarAlgoritmosOtimizados();

return 0;

//bool executando = true;

//while (executando)
//{
//    Console.WriteLine("===== Menu de Algoritmos =====");
//    Console.WriteLine("1. AStarCentrality");
//    Console.WriteLine("2. AStarWeightedCentrality");
//    Console.WriteLine("3. OrderedSearch");
//    Console.WriteLine("4. GreedySearch");
//    Console.WriteLine("5. SolveBacktracking");
//    Console.WriteLine("6. DepthFirstSearch");
//    Console.WriteLine("7. BreadthFirstSearch");
//    Console.WriteLine("8. OptimizedBFSBitmask");
//    Console.WriteLine("9. Limpar console");
//    Console.WriteLine("10. Sair");
//    Console.WriteLine("11. Executar algoritmos otimizados");
//    Console.WriteLine("12. Executar análise de performance");
//    Console.Write("Escolha uma opção: ");

//    string? opcao = Console.ReadLine();
//    Console.WriteLine();

//    switch (opcao)
//    {
//        case "1":
//            Algorithm.AStarCentrality(board, out root);
//            Console.WriteLine("AStarCentrality executado.");
//            Helpers.ExportSearchTreeToDOT(root, "searchTreeCentrality.dot");
//            break;

//        case "2":
//            Console.Write("Digite o valor para AStarWeightedCentrality (ou 'inf' para Double.MaxValue): ");
//            string? entrada = Console.ReadLine();
//            double parametro;
//            if (entrada?.Trim().ToLower() == "inf")
//            {
//                parametro = double.MaxValue;
//            }
//            else if (!double.TryParse(entrada, out parametro))
//            {
//                Console.WriteLine("Valor inválido. Será utilizado Double.MaxValue.");
//                parametro = double.MaxValue;
//            }
//            Algorithm.AStarWeightedCentrality(board, parametro);
//            Console.WriteLine($"AStarWeightedCentrality executado com o valor: {parametro}");
//            break;

//        case "3":
//            Algorithm.OrderedSearch(board, out root);
//            Console.WriteLine("OrderedSearch executado.");
//            break;

//        case "4":
//            Algorithm.GreedySearch(board);
//            Console.WriteLine("GreedySearch executado.");
//            break;

//        case "5":
//            Algorithm.SolveBacktracking(board, pegCount);
//            Console.WriteLine("SolveBacktracking executado.");
//            break;

//        case "6":
//            Algorithm.DepthFirstSearch(board, pegCount);
//            Console.WriteLine("DepthFirstSearch executado.");
//            break;

//        case "7":
//            Algorithm.BreadthFirstSearch(board, pegCount);
//            Console.WriteLine("BreadthFirstSearch executado.");
//            break;

//        case "8":
//            PegSolitaireOptimized.OptimizedBFSBitmask(board, pegCount);
//            Console.WriteLine("OptimizedBFSBitmask executado.");
//            break;

//        case "9":
//            Console.Clear();
//            break;

//        case "10":
//            executando = false;
//            Console.WriteLine("Encerrando o aplicativo...");
//            break;

//        case "11":
//            ExecutarAlgoritmosOtimizados();
//            break;

//        case "12":
//            ExecutarAnaliseDePerformance();
//            break;

//        default:
//            Console.WriteLine("Opção inválida. Tente novamente.");
//            break;
//    }
//    Console.WriteLine();
//}

//void ExecutarAlgoritmosOtimizados()
//{
//    Console.WriteLine("\nRunning new optimized implementations...");
//    Console.WriteLine("---------------------------------");

//    // Configure algorithm parameters
//    Console.WriteLine("Setting algorithm configuration...");
//    // Display configuration
//    Console.WriteLine($"Max iterations: {Config.MaxIterations}");
//    Console.WriteLine($"Timeout: {Config.TimeoutMs}ms");
//    Console.WriteLine($"Hash detection: {Config.CheckHashValues}");
//    Console.WriteLine($"Algorithm iterations: {Config.AlgorithmIterations}");
//    Console.WriteLine("---------------------------------");

//    // A* with default CountPegs heuristic
//    Console.WriteLine("New A* with CountPegs heuristic:");
//    var aStarStats = NewAlgorithm.AStar(board, pegCount);

//    // Best First Search with Centrality heuristic
//    Console.WriteLine("New Best First Search with Centrality heuristic:");
//    var bfsStats = NewAlgorithm.BestFirstSearch(board);

//    // A* with weighted Centrality heuristic
//    Console.WriteLine("New A* with weighted Centrality heuristic:");
//    var aStarWeightedStats = NewAlgorithm.AStarWeightedHeuristic(board, 1.5);

//    // Ordered Search with Centrality as primary and FutureMobility as secondary heuristic
//    Console.WriteLine("New Ordered Search with multiple heuristics:");
//    var orderedSearchStats = NewAlgorithm.OrderedSearch(board);

//    // Breadth First Search - commented out as it may be too slow
//    //Console.WriteLine("New Breadth First Search:");
//    //var breadthFirstStats = NewAlgorithm.BreadthFirstSearch(board, pegCount);

//    // Depth First Search
//    Console.WriteLine("New Depth First Search:");
//    var depthFirstStats = NewAlgorithm.DepthFirstSearch(board, pegCount);

//    // Backtracking Search
//    Console.WriteLine("New Backtracking Search:");
//    var backtrackingStats = NewAlgorithm.SolveBacktracking(board, pegCount);

//    // Optimized BFS with Bitmask
//    Console.WriteLine("New Optimized BFS with Bitmask:");
//    var bfsBitmaskStats = NewAlgorithm.OptimizedBFSBitmask(board, pegCount);

//    // Print overall summary
//    Console.WriteLine("\n=== Overall Performance Summary ===");
//    Console.WriteLine($"A* Success Rate: {aStarStats.SuccessfulRuns}/{aStarStats.TotalRuns} ({(double)aStarStats.SuccessfulRuns / aStarStats.TotalRuns:P2})");
//    Console.WriteLine($"Best First Success Rate: {bfsStats.SuccessfulRuns}/{bfsStats.TotalRuns} ({(double)bfsStats.SuccessfulRuns / bfsStats.TotalRuns:P2})");
//    Console.WriteLine($"A* Weighted Success Rate: {aStarWeightedStats.SuccessfulRuns}/{aStarWeightedStats.TotalRuns} ({(double)aStarWeightedStats.SuccessfulRuns / aStarWeightedStats.TotalRuns:P2})");
//    Console.WriteLine($"Ordered Search Success Rate: {orderedSearchStats.SuccessfulRuns}/{orderedSearchStats.TotalRuns} ({(double)orderedSearchStats.SuccessfulRuns / orderedSearchStats.TotalRuns:P2})");
//    Console.WriteLine($"DFS Success Rate: {depthFirstStats.SuccessfulRuns}/{depthFirstStats.TotalRuns} ({(double)depthFirstStats.SuccessfulRuns / depthFirstStats.TotalRuns:P2})");
//    Console.WriteLine($"Backtracking Success Rate: {backtrackingStats.SuccessfulRuns}/{backtrackingStats.TotalRuns} ({(double)backtrackingStats.SuccessfulRuns / backtrackingStats.TotalRuns:P2})");
//    Console.WriteLine($"Optimized BFS Success Rate: {bfsBitmaskStats.SuccessfulRuns}/{bfsBitmaskStats.TotalRuns} ({(double)bfsBitmaskStats.SuccessfulRuns / bfsBitmaskStats.TotalRuns:P2})");
//}

//void ExecutarAnaliseDePerformance()
//{
//    Console.WriteLine("\n===== Iniciando Análise de Performance dos Algoritmos =====");
//    Console.WriteLine("Esta análise executará todos os algoritmos com e sem verificação de hash");
//    Console.WriteLine("O processo pode demorar vários minutos. Por favor, aguarde...");

//    // Create the analyzer and run the tests
//    var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "PerformanceReport.md");
//    var analyzer = new RestaUm.Performance.AlgorithmAnalyzer(board, pegCount, outputPath);

//    analyzer.RunAllAlgorithms();
//    analyzer.GenerateMarkdownReport();

//    Console.WriteLine("\n===== Análise de Performance Concluída =====");
//    Console.WriteLine($"Relatório completo gerado em: {outputPath}");
//}

