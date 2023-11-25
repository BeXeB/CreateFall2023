using UnityEngine;

public class Tile : MonoBehaviour
{
    public Piece piece { get; private set; }
    public int row { get; set; }
    public int column { get; set; }
    public TileState state { get; private set; }
    public void SetPiece(Piece piece, bool isWhite)
    {
        this.piece = piece;
        piece.isWhite = isWhite;
        gameObject.GetComponentsInChildren<SpriteRenderer>()[2].sprite = piece.sprite;
    }
    
    public void ClearPiece()
    {
        piece = null;
        gameObject.GetComponentsInChildren<SpriteRenderer>()[2].sprite = null;
    }
    
    public void SetState(TileState state)
    {
        this.state = state;
        gameObject.GetComponentsInChildren<SpriteRenderer>()[1].color = GameManager.instance.GetHighlightColor(state);
    }
}

public enum TileState
{
    Default,
    Highlighted,
    Attack,
    Reload
}
