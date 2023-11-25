using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Piece
{
    public PieceType type { get; set; }
    public int move { get; set; }
    public int attack { get; set; }
    public bool isWhite { get; set; }
}

public enum PieceType
{
    Pleb,
    Sniper,
    Barrel,
    Horse
}