using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    private int _currentTurn = 0;
    private int _totalTurns = 0;
    private int _whiteScore = 2;
    private int _blackScore = 2;

    public static GameManager Instance {
        get { 
            if (_instance == null)
                Debug.LogError("Game Manager is null");

            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    public int CurrentTurn
    {
        get => _currentTurn;
    }

    public int TotalTurns
    {
        get => _totalTurns;
    }

    public int WhiteScore
    {
        get => _whiteScore;
    }

    public int BlackScore
    {
        get => _blackScore;
    }

    public void AdvanceTurn(int whiteScore, int blackScore) { 
        _currentTurn = 1 - _currentTurn;
        _totalTurns++;
        _whiteScore = whiteScore;
        _blackScore = blackScore;
    }
}
