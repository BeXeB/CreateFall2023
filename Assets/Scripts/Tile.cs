using UnityEngine;

public class Tile : MonoBehaviour
{
    public Piece piece { get; private set; }
    public EquipmentType pickUp { get; set; } = EquipmentType.None;
    public int row { get; set; }
    public int column { get; set; }
    public TileState state { get; private set; }
    public void SetPiece(Piece piece, bool isWhite, EquipmentType? equipmentType = null)
    {
        this.piece = piece;
        piece.isWhite = isWhite;
        if (equipmentType != null)
            piece.Equip((EquipmentType)equipmentType);
        gameObject.GetComponentsInChildren<SpriteRenderer>()[2].sprite = piece.sprite;
    }
    
    public void ClearPiece(bool drop = false)
    {
        if (drop && piece != null && piece.equipmentType != EquipmentType.None)
        {
            pickUp = piece.equipmentType;
        }
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
