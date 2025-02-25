# Algoritmo Best-First Search para o Resta Um

Este projeto implementa o algoritmo **Best-First Search** (Busca de Melhor Escolha) para resolver o jogo **Resta Um**. O objetivo do jogo é remover pinos do tabuleiro até que reste apenas um pino, seguindo as regras de movimentação.

## Como o Algoritmo Funciona

O Best-First Search é um algoritmo de busca que explora o espaço de estados com base em uma **função heurística**. Ele prioriza os estados que parecem mais promissores em direção ao objetivo. A função heurística usado foi simples: ela retorna o número de pinos restantes no tabuleiro. Quanto menor o número de pinos, mais próximo o estado está do objetivo.

### 1. Fila de Prioridade (Fronteira)
O algoritmo usa uma **fila de prioridade** para armazenar os nós a serem explorados. Cada nó é ordenado com base em seu **valor heurístico** (quanto menor o valor, maior a prioridade). Isso garante que o algoritmo sempre explore os estados mais promissores primeiro.

### 2. Conjunto de Explorados
Um **conjunto de explorados** é usado para manter o controle dos estados que já foram visitados. Isso evita que o algoritmo fique preso em loops, revisitando estados desnecessariamente. Cada estado é armazenado como uma **string única**, gerada a partir do tabuleiro.

### 3. Nó Inicial
O algoritmo começa criando o **nó inicial**, que representa o estado inicial do tabuleiro. Esse nó não tem um nó pai, pois é o primeiro estado. O custo do caminho (`PathCost`) começa em 0, e o valor heurístico é calculado com base no número de pinos restantes.

### 4. Adicionar o Nó Inicial à Fronteira
O nó inicial é adicionado à **fronteira** para ser explorado. Ele é priorizado com base em seu valor heurístico.

### 5. Explorar o Próximo Nó
A cada iteração, o algoritmo remove o nó com o **menor valor heurístico** da fronteira. Esse nó será o próximo a ser explorado.

### 6. Verificar se o Objetivo foi Alcançado
O objetivo do Resta Um é reduzir o número de pinos a **apenas um**. Se o valor heurístico do nó atual for 1, significa que o objetivo foi alcançado. O algoritmo imprime a solução e encerra.

### 7. Pular Estados Já Explorados
Antes de explorar um nó, o algoritmo verifica se ele já foi visitado. Se o estado já estiver no **conjunto de explorados**, ele é ignorado. Caso contrário, o estado é adicionado ao conjunto.

### 8. Gerar Movimentos Possíveis (Estados Sucessores)
Para cada estado, o algoritmo gera todos os **movimentos válidos**. Um movimento válido consiste em pular um pino adjacente e ocupar a posição vazia. Para cada movimento válido:
1. Um novo estado é criado.
2. O custo do caminho é atualizado (cada movimento custa 2 unidades).
3. Um novo nó é criado com o estado resultante.
4. O novo nó é adicionado à **fronteira** para ser explorado posteriormente.

### 9. Realizar o Movimento
O movimento é aplicado ao tabuleiro, atualizando as posições dos pinos.

### 10. Calcular o Novo Custo do Caminho
Cada movimento tem um custo fixo de **2 unidades**. O custo acumulado é armazenado no nó.

### 11. Criar o Novo Nó
Um novo nó é criado para representar o estado resultante do movimento. Ele armazena:
- O estado do tabuleiro.
- O nó pai.
- A ação realizada.
- O custo do caminho.
- O valor heurístico.

### 12. Adicionar o Novo Nó à Fronteira
O novo nó é adicionado à **fronteira** para ser explorado posteriormente. Ele é priorizado com base em seu valor heurístico.

## Resumo do Funcionamento
1. O algoritmo começa com o estado inicial do tabuleiro.
2. Ele explora os estados mais promissores primeiro, com base na função heurística.
3. Para cada estado, ele gera todos os movimentos válidos e adiciona os novos estados à fronteira.
4. O processo continua até que o objetivo seja alcançado ou todos os estados possíveis tenham sido explorados.

## Exemplo de Saída
Se o algoritmo encontrar uma solução, ele imprimirá o caminho completo, mostrando cada movimento e o estado do tabuleiro após cada ação.

## Como Executar o Projeto
1. Clone o repositório:
   ```bash
   git clone https://github.com/IgorWestermann/RestaUm
   cd PegSolitaireSolver
   dotnet run