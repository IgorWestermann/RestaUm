using RestaUm.Helpers;
using RestaUm;

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