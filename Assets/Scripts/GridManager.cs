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

    // Start is called before the first frame update
    void Start()
    {
        GenerateBoard();
    }


    void GenerateBoard()
    {
        _camera.position = new Vector3((float)SIZE / 2 - 0.5f, (float)SIZE / 2 - 0.5f, -10);

        for (int i = 0; i < SIZE; i++)
        {
            for (int j = 0; j < SIZE; j++)
            {
                var tile = Instantiate(_tilePrefab, new Vector3(i, j), Quaternion.identity);
                tile.name = $"Tile {i} {j}";
            }
        }

        var frame = Instantiate(_framePrefab, new Vector3((float)SIZE / 2 - 0.5f, (float)SIZE / 2 - 0.5f), Quaternion.identity);
        frame.name = "Frame";
    }
}
