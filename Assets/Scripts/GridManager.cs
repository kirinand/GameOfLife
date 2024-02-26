using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Threading.Tasks;

public class GridManager : MonoBehaviour
{
    private static GridManager _instance;
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private Frame _framePrefab;
    [SerializeField] private Transform _camera;
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
    }


    public void GenerateBoard(sbyte[,] boardState)
    {   
        int size = boardState.GetLength(0);
        _camera.position = new Vector3((float)size / 2 - 0.5f, (float)size / 2 - 0.5f, -10);
        _tiles = new Tile[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                var tile = Instantiate(_tilePrefab, new Vector3(i, j), Quaternion.identity);
                tile.name = $"Tile {i} {j}";
                tile.Initialize(i, j, boardState[i, j]);
                _tiles[i, j] = tile;
            }
        }

        var frame = Instantiate(_framePrefab, new Vector3((float)size / 2 - 0.5f, (float)size / 2 - 0.5f), Quaternion.identity);
        frame.name = "Frame";
    }

    public async void UpdateBoard(sbyte[,] currState, sbyte[,] nextState, int[] score)
    {
        int size = currState.GetLength(0);

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {   
                int piece = nextState[i, j];

                if (piece != currState[i, j])
                    await _tiles[i, j].ChangeState(piece);
            }
        }

        GameManager.Instance.AdvanceTurn(score[0], score[1]);
    }

    public async Task UpdateTile(int x, int y, int piece)
    {
        await _tiles[x, y].ChangeState(piece, true);
    }
}
