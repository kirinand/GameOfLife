using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public const int SIZE = 8;

    private static GridManager _instance;
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private Frame _framePrefab;
    [SerializeField] private Transform _camera;
    private int[,] _boardState;
    private Tile[,] _tiles;

    public static GridManager Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError("Grid Manager is null");

            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
        InitializeBoardState();
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateBoard();
    }


    void GenerateBoard()
    {
        _camera.position = new Vector3((float)SIZE / 2 - 0.5f, (float)SIZE / 2 - 0.5f, -10);
        _tiles = new Tile[SIZE, SIZE];

        for (int i = 0; i < SIZE; i++)
        {
            for (int j = 0; j < SIZE; j++)
            {
                var tile = Instantiate(_tilePrefab, new Vector3(i, j), Quaternion.identity);
                tile.name = $"Tile {i} {j}";
                tile.Initialize(i, j, _boardState[i, j]);
                _tiles[i, j] = tile;
            }
        }

        var frame = Instantiate(_framePrefab, new Vector3((float)SIZE / 2 - 0.5f, (float)SIZE / 2 - 0.5f), Quaternion.identity);
        frame.name = "Frame";
    }

    void InitializeBoardState() { 
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

    int GetNeighborCount(int x, int y) { 
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

    public void MakeMove(int x, int y, int newPiece) {
        _boardState[x, y] = newPiece;
        int[,] _nextState = _boardState.Clone() as int[,];

        int[] totalCount = { 0, 0 };

        for (int i = 0; i < SIZE; i++)
        {
            for (int j = 0; j < SIZE; j++)
            {
                int piece = _boardState[i, j];
                int neighborCount = GetNeighborCount(i, j);

                if (piece == -1) 
                {
                    if (neighborCount == 3)
                    {
                        _nextState[i, j] = newPiece;
                        totalCount[newPiece]++;
                    }
                }
                else {
                    if (neighborCount < 2 || neighborCount > 3) 
                        _nextState[i, j] = -1;
                    else 
                        totalCount[piece]++;
                }
                
                
            }
        }

        for (int i = 0; i < SIZE; i++)
        {
            for (int j = 0; j < SIZE; j++)
            {   
                int piece = _nextState[i, j];

                if (piece != _boardState[i, j])
                    _tiles[i, j].ChangeState(piece);
            }
        }

        _boardState = _nextState;
        GameManager.Instance.AdvanceTurn(totalCount[0], totalCount[1]);
    }
}
