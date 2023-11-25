using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Piece : ICloneable
{
    public PieceType type;
    public int move;
    public int attack;
    [NonSerialized] public bool isWhite;
    public Sprite sprite;
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