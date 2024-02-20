using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{   

    [SerializeField] private GameObject _highlight;
    [SerializeField] private GameObject _piece;

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

            var pieceScript = _piece.GetComponent<Piece>();
            int turn = GameManager.Instance.CurrentTurn;
            pieceScript.Color = turn == 0 ? Piece.PieceColor.White : Piece.PieceColor.Black;
            _piece.SetActive(true);
            _highlight.SetActive(false);
        }
    }
}
