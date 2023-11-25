using System;
using UnityEngine;

[Serializable]
public class Piece : ICloneable
{
    public PieceType type;
    public int move;
    public int attack;
    [NonSerialized] public bool isWhite;
    public Sprite sprite;
    [NonSerialized] public bool attacked = false;
    [NonSerialized] public bool movedThisTurn = false;
    public object Clone()
    {
        return new Piece
        {
            type = this.type,
            move = this.move,
            attack = this.attack,
            isWhite = this.isWhite,
            sprite = this.sprite
        };
    }
}

public enum PieceType
{
    Pleb,
    Sniper,
    Barrel,
    Horse
}