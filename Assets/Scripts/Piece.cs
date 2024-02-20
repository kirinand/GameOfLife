using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public enum PieceColor
    {
        White = 0,
        Black = 1
    }

    private PieceColor _color;

    public PieceColor Color 
    {
        get {
            return _color;
        }
        set {
            if (value == PieceColor.White)
            {
                GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/WhiteStone");
            }
            else
            {
                GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/BlackStone");
            }

            _color = value;
        }
    }
}
