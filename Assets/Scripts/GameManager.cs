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
    private int _ai; // 0 for white, 1 for black, -1 if no AI

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
        Time.timeScale = 1;
        InGameUIControl.Instance.UpdateTurnInfoUI(_currentTurn, INITAIL_SCORE, INITAIL_SCORE);
        _ai = 1;
        if (_ai == _currentTurn) PlayAI();
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

        if (_ai == _currentTurn) PlayAI();
    }

    private void PlayAI()
    {
        Time.timeScale = 0;
        PlayAgent.Instance.MakeAIMove(_ai);
        Time.timeScale = 1;
    }
}
