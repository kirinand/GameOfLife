using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class PlayAgent : MonoBehaviour
{
    public const int SIZE = 8;
    private int[,] _boardState;
    private static PlayAgent _instance;

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
        InitializeBoardState();
    }

    private void Start()
    {
        GridManager.Instance.GenerateBoard(_boardState);
    }

    void InitializeBoardState()
    {
        _boardState = new int[SIZE, SIZE];

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

    public void MakeMove(int x, int y, int newPiece)
    {
        _boardState[x, y] = newPiece;
        int[,] _nextState = _boardState.Clone() as int[,];

        for (int i = 0; i < SIZE; i++)
        {
            for (int j = 0; j < SIZE; j++)
            {
                int piece = _boardState[i, j];
                int neighborCount = GetNeighborCount(i, j);

                if (piece == -1)
                {
                    if (neighborCount == 3)
                        _nextState[i, j] = newPiece;
                }
                else
                {
                    if (neighborCount < 2 || neighborCount > 3)
                        _nextState[i, j] = -1;
                }
            }
        }

        GridManager.Instance.AdvanceBoardState(_boardState, _nextState);
        _boardState = _nextState;
    }

    int GetNeighborCount(int x, int y)
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
                if (_boardState[i, j] != -1 && !(i == x && j == y))
                    count++;
            }
        }

        return count;
    }
}
