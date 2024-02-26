using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Tile : MonoBehaviour
{   

    [SerializeField] private GameObject _highlight;
    [SerializeField] private GameObject _piece;
    public int X { get; private set; }
    public int Y { get; private set; }

    private void OnMouseEnter()
    {   
        if (!_piece.activeSelf)
        {   
            if (Time.timeScale > 0)
                _highlight.SetActive(true);
        }
    }

    private void OnMouseExit()
    {
        if (Time.timeScale > 0)
            _highlight.SetActive(false);
    }

    private async void OnMouseDown()
    { 
        if (Time.timeScale > 0 && !_piece.activeSelf)
        {   

            int turn = GameManager.Instance.CurrentTurn;
            // prevent player input to the board
            Time.timeScale = 0;
            await ChangeState(turn, true);
            PlayAgent.Instance.MakeMove(X, Y, turn);
        }
    }

    private void ActivatePiece(int color)
    {
        var pieceScript = _piece.GetComponent<Piece>();
        pieceScript.Color = (Piece.PieceColor)color;  
        _piece.SetActive(true);
    }

    public void Initialize(int x, int y, int state=-1)
    {
        X = x;
        Y = y;
        if (state != -1) ActivatePiece(state);
    }

    public async Task ChangeState(int state, bool isMove=false) {
        if (state == -1)
        {
            _piece.SetActive(false);
        }
        else 
        { 
            ActivatePiece(state);

            if (isMove) 
            {   
                _highlight.SetActive(true);
                await Task.Delay(500);
                _highlight.SetActive(false);
            }
        } 
    }
}
