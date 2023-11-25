using UnityEngine;

public class Tile : MonoBehaviour
{
    public Piece piece { get; private set; }
    public EquipmentType pickUp { get; set; } = EquipmentType.None;
    public int row { get; set; }
    public int column { get; set; }
    public TileState state { get; private set; }
    
    private SpriteRenderer highlight;
    private SpriteRenderer pieceSprite;
    
    private void Awake()
    {
        highlight = gameObject.GetComponentsInChildren<SpriteRenderer>()[1];
        pieceSprite = gameObject.GetComponentsInChildren<SpriteRenderer>()[2];
    }
    
    public void SetPiece(Piece piece, bool isWhite, EquipmentType? equipmentType = null)
    {
        this.piece = piece;
        this.piece.equippedChanged += HandleEquipmentChanged;
        this.piece.isWhite = isWhite;
        if (equipmentType != null)
            piece.Equip((EquipmentType)equipmentType);
        pieceSprite.sprite = piece.sprite;
    }
    
    private void HandleEquipmentChanged()
    {
        if (piece != null)
            pieceSprite.sprite = piece.sprite;
    }
    
    public void ClearPiece(bool drop = false)
    {
        if (drop && piece != null && piece.equipmentType != EquipmentType.None)
        {
            pickUp = piece.equipmentType;
        }
        piece = null;
        pieceSprite.sprite = null;
    }
    
    public void SetState(TileState state)
    {
        this.state = state;
        highlight.color = GameManager.instance.GetHighlightColor(state);
    }
}

public enum TileState
{
    Default,
    Highlighted,
    Attack,
    Reload
}
