using System;
using System.Collections;
using System.Collections.Generic;
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
            _highlight.SetActive(true);
        }
    }

    private void OnMouseExit()
    {
        _highlight.SetActive(false);
    }

    private void OnMouseDown()
    { 
        if (!_piece.activeSelf)
        {   

            int turn = GameManager.Instance.CurrentTurn;
            ActivatePiece(turn);
            _highlight.SetActive(false);
            GridManager.Instance.MakeMove(X, Y, turn);
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

    public void ChangeState(int state) {
        if (state == -1)
            _piece.SetActive(false);
        else 
            ActivatePiece(state);
    }
}
