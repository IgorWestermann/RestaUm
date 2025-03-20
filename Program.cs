﻿using RestaUm;
using RestaUm.algoritimos;
using RestaUm.Helpers;

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

bool executando = true;

while (executando)
{
    Console.WriteLine("===== Menu de Algoritmos =====");
    Console.WriteLine("1. AStarCentrality");
    Console.WriteLine("2. AStarWeightedCentrality");
    Console.WriteLine("3. OrderedSearch");
    Console.WriteLine("4. GreedySearch");
    Console.WriteLine("5. SolveBacktracking");
    Console.WriteLine("6. DepthFirstSearch");
    Console.WriteLine("7. BreadthFirstSearch");
    Console.WriteLine("8. Greedy Best First Search - CountPegs");
    Console.WriteLine("9. Greedy Best First Search - Distance");
    Console.WriteLine("10. Greedy Best First Search - Connectivity");
    Console.WriteLine("11. Greedy Best First Search - Centrality");
    Console.WriteLine("12. Greedy Best First Search - Centrality With Penality");
    Console.WriteLine("13. A* - CountPegs");
    Console.WriteLine("14. A* - Distance");
    Console.WriteLine("15. A* - Connectivity");
    Console.WriteLine("16. A* - Centrality");
    Console.WriteLine("17. A* - Centrality With Penality");
    Console.WriteLine("99. Limpar console");
    Console.WriteLine("00. Sair");
    Console.Write("Escolha uma opção: ");

    string? opcao = Console.ReadLine();
    Console.WriteLine();

    switch (opcao)
    {
        case "1":
            Algorithm.AStarCentrality(board, out root);
            Console.WriteLine("AStarCentrality executado.");
            //Helpers.ExportSearchTreeToDOT(root, "searchTreeCentrality.dot");
            break;

        case "2":
            Console.Write("Digite o valor para AStarWeightedCentrality (ou 'inf' para Double.MaxValue): ");
            string? entrada = Console.ReadLine();
            double parametro;
            if (entrada?.Trim().ToLower() == "inf")
            {
                parametro = double.MaxValue;
            }
            else if (!double.TryParse(entrada, out parametro))
            {
                Console.WriteLine("Valor inválido. Será utilizado Double.MaxValue.");
                parametro = double.MaxValue;
            }
            Algorithm.AStarWeightedCentrality(board, parametro);
            Console.WriteLine($"AStarWeightedCentrality executado com o valor: {parametro}");
            break;

        case "3":
            Algorithm.OrderedSearch(board, out root);
            Console.WriteLine("OrderedSearch executado.");
            break;

        case "4":
            Algorithm.GreedySearch(board);
            Console.WriteLine("GreedySearch executado.");
            break;

        case "5":
            Algorithm.SolveBacktracking(board, pegCount);
            Console.WriteLine("SolveBacktracking executado.");
            break;

        case "6":
            Algorithm.DepthFirstSearch(board, pegCount);
            Console.WriteLine("DepthFirstSearch executado.");
            break;

        case "7":
            Algorithm.BreadthFirstSearch(board, pegCount);
            Console.WriteLine("BreadthFirstSearch executado.");
            break;

        case "8":
            BestFirstSearchSolver.Solve(board, Heuristica.CountPegs, "Best First Search - CountPegs", out root);
            Helpers.ExportSearchTreeToDOT(root, "greedyBFS.dot");
            break;
        case "9":
            BestFirstSearchSolver.Solve(board, Heuristica.Distance, "Best First Search - Distance", out root);
            break;
        case "10":
            BestFirstSearchSolver.Solve(board, Heuristica.Connectivity, "Best First Search - Connectivity", out root);
            break;
        case "11":
            BestFirstSearchSolver.Solve(board, Heuristica.Centrality, "Best First Search - Centrality", out root);
            break;
        case "12":
            BestFirstSearchSolver.Solve(board, Heuristica.CentralityWhitPenality, "Best First Search - CentralityWhitPenality", out root);
            break;
        case "13":
            AStarSolver.Solve(board, Heuristica.CountPegs, "A* - CountPegs", out root);
            break;
        case "14":
            AStarSolver.Solve(board, Heuristica.Distance, "A* - Distance", out root);
            break;
        case "15":
            AStarSolver.Solve(board, Heuristica.Connectivity, "A* - Connectivity", out root);
            break;
        case "16":
            AStarSolver.Solve(board, Heuristica.Centrality, "A* - Centrality", out root);
            break;
        case "17":
            AStarSolver.Solve(board, Heuristica.CentralityWhitPenality, "A* - CentralityWhitPenality", out root);
            break;


        case "99":
            Console.Clear();
            break;

        case "00":
            executando = false;
            Console.WriteLine("Encerrando o aplicativo...");
            break;

        default:
            Console.WriteLine("Opção inválida. Tente novamente.");
            break;
    }
    Console.WriteLine();
}


/* A*
*/
//Console.WriteLine("Heuristica Centralidade, para o Aloritimo A*");
//Algorithm.AStarCentrality(board,out root);


//Helpers.ExportSearchTreeToDOT(root, "searchTreeCentrality.dot");
//Console.WriteLine("A árvore de busca foi exportada para 'searchTreeCentrality.dot'.");
/*
*/

//Console.WriteLine("Heuristica Centralidade, para o Aloritimo A* com peso `infinito`(Quanto maior melhor pelos meus testes)");
//Algorithm.AStarWeightedCentrality(board, Double.MaxValue);

/*Ordenada*/

//Console.WriteLine("heuristica centralidade, para o aloritimo ordenada");
//Algorithm.OrderedSearch(board, out root);

//Tem alguma coisa errada com o export para esse caso eu nao consegui achar
//Helpers.ExportSearchTreeToDOT(root, "searchTreeOrdered.dot");
//Console.WriteLine("A árvore de busca foi exportada para 'searchTreeOrdered.dot'.");

/*Gulosa*/

//Console.WriteLine("Heuristica Centralidade, para o Aloritimo Gulosa");
//Algorithm.GreedySearch(board);


//Backtracking
//Console.WriteLine("Busca Backtracking");
//Algorithm.SolveBacktracking(board, pegCount);

//Console.WriteLine("Algoritmo de profundidade");
//Algorithm.DepthFirstSearch(board, pegCount);

//Console.WriteLine("Algoritmo de largura");
//Algorithm.BreadthFirstSearch(board, pegCount);