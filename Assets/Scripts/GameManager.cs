using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    private static int INITAIL_SCORE = 2;
    private int _currentTurn = 0;
    private int _totalTurns = 0;
    private int _whiteScore = INITAIL_SCORE;
    private int _blackScore = INITAIL_SCORE;

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

    private void Start()
    {
        InGameUIControl.Instance.UpdateTurnInfoUI(_currentTurn, INITAIL_SCORE, INITAIL_SCORE);
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

        InGameUIControl.Instance.UpdateTurnInfoUI(_currentTurn, whiteScore, blackScore);

        if (whiteScore == 0) 
        {
            InGameUIControl.Instance.GameOver(1, _totalTurns);
        }
        else if (blackScore == 0)
        {
            if (whiteScore == 0)
                InGameUIControl.Instance.GameOver(2, _totalTurns);
            else
                InGameUIControl.Instance.GameOver(0, _totalTurns);
        }
    }
}
