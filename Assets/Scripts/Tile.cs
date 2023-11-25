using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Piece piece { get; private set; }
    public int row { get; set; }
    public int column { get; set; }

    public void SetPiece(Piece piece, bool isWhite)
    {
        this.piece = piece;
        piece.isWhite = isWhite;
        gameObject.GetComponentsInChildren<SpriteRenderer>()[2].sprite = piece.sprite;
    }
}
