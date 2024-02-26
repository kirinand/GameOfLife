using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;

public class PlayAgent : MonoBehaviour
{
    public const int SIZE = 8;
    private sbyte[,] _boardState;
    private static PlayAgent _instance;
    private CancellationTokenSource _cts;

    public static PlayAgent Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError("Play Agent is null");

            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
        _cts = new CancellationTokenSource();
        InitializeBoardState();
    }

    private void Start()
    {
        GridManager.Instance.GenerateBoard(_boardState);
    }

    public void OnDestroy()
    {
        _cts.Cancel();
    }

    void InitializeBoardState()
    {
        _boardState = new sbyte[SIZE, SIZE];

        for (int i = 0; i < SIZE; i++)
        {
            for (int j = 0; j < SIZE; j++)
            {
                _boardState[i, j] = -1;
            }
        }

        int d = SIZE / 2 - 1;
        int e = SIZE / 2;

        _boardState[d, d] = 1;
        _boardState[d, e] = 0;
        _boardState[e, d] = 0;
        _boardState[e, e] = 1;
    }

    public void MakeMove(int x, int y, int turn)
    {
        _boardState[x, y] = (sbyte)turn;
        sbyte[,] currState = _boardState.Clone() as sbyte[,];
        (sbyte[,] nextState, int[] score) = CalculateNextState(currState, turn);
        _boardState = nextState;
        GridManager.Instance.UpdateBoard(currState, nextState, score);
    }

    public static (sbyte[,] nextState, int[] score) CalculateNextState(sbyte[,] currState, int turn) 
    {
        sbyte[,] nextState = currState.Clone() as sbyte[,];
        int[] score = { 0, 0 }; // { white, black }

        for (int i = 0; i < SIZE; i++)
        {
            for (int j = 0; j < SIZE; j++)
            {
                int piece = currState[i, j];
                int neighborCount = GetNeighborCount(i, j, currState);

                if (piece == -1)
                {
                    if (neighborCount == 3)
                        nextState[i, j] = (sbyte)turn;
                }
                else
                {
                    if (neighborCount < 2 || neighborCount > 3)
                        nextState[i, j] = -1;
                }

                if (nextState[i, j] != -1)
                    score[nextState[i, j]]++;
            }
        }

        return (nextState, score);
    }
    
    public static int GetNeighborCount(int x, int y, sbyte[,] boardState)
    {
        int count = 0;

        int lx = Mathf.Max(x - 1, 0);
        int ly = Mathf.Max(y - 1, 0);
        int ux = Mathf.Min(x + 2, SIZE);
        int uy = Mathf.Min(y + 2, SIZE);

        for (int i = lx; i < ux; i++)
        {
            for (int j = ly; j < uy; j++)
            {
                if (boardState[i, j] != -1 && !(i == x && j == y))
                    count++;
            }
        }

        return count;
    }

    public async void MakeAIMove(int turn)
    {   
        (int x, int y)  = await SearchNextMove(turn, _cts.Token);
        await GridManager.Instance.UpdateTile(x, y, turn, _cts.Token);
        MakeMove(x, y, turn);
    }

    async Task<(int x, int y)> SearchNextMove(int turn, CancellationToken ct) 
    {   
        return await Task.Run(() =>
        {
            MCTS mcts = new MCTS(_boardState, turn);
            (int, int) move = mcts.Search(ct);
            return move;
        },  ct);
    }
}


/// <summary>
/// Use Monte Carlo Tree Search to find the next move.
/// 1. Select: Start from root node (current state) and select the child node with the highest UCT value until a leaf node (any node with a potential child to run simulation) is reached.
/// 2. Expand: Unless the leaf node is a winning move, expand the node by adding all child nodes to the tree.
/// 3. Simulate: Choose one child node from the newly added ones and run 5 simulations until the game ends or the depth reaches 10.
/// 4. Backpropagate: Update the nodes from the selected node to the root node with the result of the simulation.
/// </summary>
class MCTS
{
    public const float Explore = 1.41f;
    public const int NumSimPerNode = 3;
    public const int MaxSimDepth = 10;
    public const int MaxSimPerMove = 500;
    public const float WinScore = 1;
    public const float LoseScore = 0;
    public const float DrawScore = 0.5f;
    public const int TimeOut = 10000;
    private readonly System.Random Random = new();
    private sbyte[,] _rootState;
    private Node _root;
    private sbyte _agentTurn; // 0 for white, 1 for black
    private int _size;


    public MCTS(sbyte[,] rootState, int turn)
    {
        _rootState = rootState.Clone() as sbyte[,];
        _agentTurn = (sbyte)turn;
        _root = new Node(_rootState, true, null);
        _size = rootState.GetLength(0);
    }


    public (int x, int y) Search(CancellationToken ct) {
        int startTime = Environment.TickCount;

        while (true) 
        {   
            ct.ThrowIfCancellationRequested();

            (Node node, bool isNextMoveFound) = Select();

            if (isNextMoveFound) 
            { 
                int idx = Random.Next(0, node.Moves.Length);
                Debug.Log(node.Moves[idx]);
                return node.Moves[idx];
            }
            (node, isNextMoveFound) = Expand(node);

            if (isNextMoveFound)
            {
                int idx = Random.Next(0, node.Moves.Length);
                Debug.Log(node.Moves[idx]);
                return node.Moves[idx];
            }

            Simulate(node);
            Backpropagate(node);

            if (Environment.TickCount - startTime > TimeOut)
            {
                Debug.Log("Search timeout exceeded");
                return GetNextMove();
            }
        }
    }


    (Node node, bool isNextMoveFound) Select()
    {
        Node node = _root;

        if (node.Children != null) 
        {
            List<Node> maxNodes = GetNodesToSearch(node);
            List<Node> maxNodesWithinLimit = maxNodes.FindAll(n => n.N <= MaxSimPerMove);

            if (maxNodesWithinLimit.Count == 0)
            {   
                Debug.Log("Search limit reached");
                return (maxNodes[Random.Next(0, maxNodes.Count)], true);
            }
            else
            {
                maxNodes = maxNodesWithinLimit;
                node = maxNodes[Random.Next(0, maxNodes.Count)];
            }

            while (node.Children != null)
            {
                maxNodes = GetNodesToSearch(node);
                node = maxNodes[Random.Next(0, maxNodes.Count)];
            }
        }

        return (node, false);
    }

    (Node node, bool isNextMoveFound) Expand(Node node)
    {   
        if (node.Terminal > -1) return (node, false);

        List<(int x, int y, int count)> potentialMoves = GetPotentialMoves(node.State);
        Dictionary<BigInteger, (Node node, List<(sbyte, sbyte)> moves)> childStates = new();

        foreach ((int x, int y, int neighborCount) in potentialMoves)
        {
            sbyte[,] state = node.State.Clone() as sbyte[,];
            int turn = node.Turn ? _agentTurn : 1 - _agentTurn;
            state[x, y] = (sbyte)turn;
            (sbyte[,] childState, int[] score) = PlayAgent.CalculateNextState(state, turn);
            BigInteger code = EncodeState(childState);

            if (childStates.ContainsKey(code))
            {
                childStates[code].moves.Add(((sbyte)x, (sbyte)y));
            }
            else
            {
                Node childNode = new Node(childState, !node.Turn, node);

                if (score[_agentTurn] == 0)
                {
                    if (score[1 - _agentTurn] == 0)
                    {
                        childNode.Terminal = 2;
                        childNode.W = DrawScore;
                    }

                    else
                    {
                        childNode.Terminal = 1;
                        childNode.W = LoseScore;
                    }

                }
                else if (score[1 - _agentTurn] == 0)
                {
                    childNode.Terminal = 0;
                    childNode.W = WinScore;
                }

                childStates[code] = (childNode, new List<(sbyte, sbyte)> { ((sbyte)x, (sbyte)y) });
            }
        }

        List<Node> children = new();

        foreach ((Node child, List<(sbyte, sbyte)> moves) in childStates.Values)
        {
            child.Moves = moves.ToArray();
            children.Add(child);
        }

        if (node == _root)
        {
            List<Node> winningNodes = children.FindAll(n => n.Terminal == 0);
            if (winningNodes.Count > 0)
            {
                Debug.Log("Found winning move");
                return (winningNodes[Random.Next(0, winningNodes.Count)], true);
            }
        }

        node.Children = children.ToArray();
        return (node.Children[Random.Next(0, node.Children.Length)], false);
    }

    void Simulate(Node node)
    {
        if (node.Terminal > -1)
        {
            node.N++;
            return;
        }

        List<float> evaluations = new();

        for (int i = 0; i < NumSimPerNode; i++)
        {
            sbyte[,] state = node.State.Clone() as sbyte[,];
            bool isAgentTurn = node.Turn;
            int[] eval = null;
            int depth = 0;

            while (depth < MaxSimDepth)
            {
                List<(int, int, int)> moves = GetPotentialMoves(state);
                (int x, int y, _) = moves[Random.Next(0, moves.Count)];
                sbyte turn = (sbyte)(isAgentTurn ? _agentTurn : 1 - _agentTurn);
                state[x, y] = turn;
                (sbyte[,] nextState, int[] score) = PlayAgent.CalculateNextState(state, turn);

                if (score[_agentTurn] == 0)
                {
                    if (score[1 - _agentTurn] == 0)
                    {
                        evaluations.Add(DrawScore);
                        eval = null;
                        break;
                    }
                    else
                    {
                        evaluations.Add(LoseScore);
                        eval = null;
                        break;
                    }
                }
                else if (score[1 - _agentTurn] == 0)
                {
                    evaluations.Add(WinScore);
                    eval = null;
                    break;
                }

                eval = score;
                isAgentTurn = !isAgentTurn;
                state = nextState;
                depth++;
            }

            if (eval != null)
                evaluations.Add(DrawScore * eval[_agentTurn] / (eval[_agentTurn] + eval[1 - _agentTurn]) + WinScore / 4f);
        }


        node.W = node.W == 0 ? evaluations.Average() : (node.W + evaluations.Average()) / 2f;
        node.N++;
    }

    void Backpropagate(Node node) 
    {
        node = node.Parent;

        while (node != null)
        {
            node.N++;
            List<float> ws = new();

            foreach (Node child in node.Children)
                ws.Add(child.W);

            node.W = ws.Average();
            node = node.Parent;
        }
    }

    List<Node> GetNodesToSearch(Node node)
    {
        double maxScore = 0;
        List<Node> maxNodes = new();

        foreach (Node child in node.Children)
        {
            double priority = child.GetUCT(Explore);

            if (priority > maxScore)
            {
                maxScore = priority;
                maxNodes.Clear();
                maxNodes.Add(child);
            }
            else if (priority == maxScore)
            {
                maxNodes.Add(child);
            }
        }

        return maxNodes;
    }

    (int, int) GetNextMove() 
    {
        float maxW = _root.Children.Max(n => n.W);
        List<Node> maxWNodes = _root.Children.Where(n => n.W == maxW).ToList();
        Node node = maxWNodes[Random.Next(0, maxWNodes.Count)];
        return node.Moves[Random.Next(0, node.Moves.Length)];
    }

    BigInteger EncodeState(sbyte[,] state)
    {
        BigInteger code = 0;
        int offset = 0;

        for (int i = 0; i < _size; i++)
        {
            for (int j = 0; j < _size; j++)
            {
                code |= (state[i, j] + 1) << offset;
                offset += 2;
            }
        }

        return code;
    }

    List<(int x, int y, int neighborCount)> GetPotentialMoves(sbyte[,] state)
    {
        List<(int x, int y, int neighborCount)> potentialMoves = new();
        List<(int x, int y, int neighborCount)> passMoves = new();

        for (int i = 0; i < _size; i++)
        {
            for (int j = 0; j < _size; j++)
            {
                if (state[i, j] == -1)
                {
                    int neighborCount = PlayAgent.GetNeighborCount(i, j, state);
                    if (neighborCount < 2)
                        passMoves.Add((i, j, neighborCount));
                    else
                        potentialMoves.Add((i, j, neighborCount));
                }
            }
        }

        if (passMoves.Count > 0)
            potentialMoves.Add(passMoves[Random.Next(0, passMoves.Count)]);

        return potentialMoves;
    }
}


class Node
{
    public sbyte[,] State;
    public bool Turn; // true for agent, false for opponent
    public float W;
    public int N;
    public Node Parent;
    public Node[] Children;
    public (sbyte x, sbyte y)[] Moves;
    public sbyte Terminal; // -1 for not terminal, 0 for win, 1 for lose, 2 for draw

    public Node(sbyte[,] state, bool turn, Node parent)
    {
        State = state.Clone() as sbyte[,];
        Turn = turn;
        W = 0;
        N = 0;
        Parent = parent;
        Children = null;
        Moves = null;
        Terminal = -1;
    }
    
    public double GetUCT(float explore)
    {
        if (N == 0) 
            return explore == 0 ? 0 : float.MaxValue;

        return W / N + explore * Mathf.Sqrt(Mathf.Log(Parent.N) / N);
    }
}