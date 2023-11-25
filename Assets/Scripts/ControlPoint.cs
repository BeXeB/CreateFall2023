using System;
using System.Collections.Generic;

[Serializable]
public class ControlPoint
{
    public Coordinates[] coordinates;
    [NonSerialized] public List<Tile> tiles = new();
    
    public void AddTiles(Tile tile)
    {
        this.tiles.Add(tile);
    }

    public bool IsControlledByWhite()
    {
        return tiles.TrueForAll(tile => tile.piece is not { isWhite: false }) &&
               tiles.Exists(tile => tile.piece is { isWhite: true });
    }
    
    public bool IsControlledByBlack()
    {
        return tiles.TrueForAll(tile => tile.piece is not { isWhite: true }) &&
               tiles.Exists(tile => tile.piece is { isWhite: false });
    }
}

[Serializable]
public struct Coordinates
{
    public int row;
    public int column;
}