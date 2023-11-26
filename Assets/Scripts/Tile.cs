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
    private SpriteRenderer pickUpSprite;
    
    private void Awake()
    {
        highlight = gameObject.GetComponentsInChildren<SpriteRenderer>()[1];
        pieceSprite = gameObject.GetComponentsInChildren<SpriteRenderer>()[2];
        pickUpSprite = gameObject.GetComponentsInChildren<SpriteRenderer>()[3];
    }
    
    public void SetPiece(Piece piece, bool isWhite, EquipmentType? equipmentType = null)
    {
        pickUpSprite.color = new Color(255, 255, 255, 0);
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
    
    public void UpdatePickUpSprite()
    {
        pickUpSprite.sprite = GameManager.instance.GetPickUpSprite(pickUp);
    }
    
    public void ClearPiece(bool drop = false)
    {
        if (drop && piece != null && piece.equipmentType != EquipmentType.None)
        {
            pickUp = piece.equipmentType;
            pickUpSprite.sprite = GameManager.instance.GetPickUpSprite(piece.equipmentType);
        }
        piece = null;
        pieceSprite.sprite = null;
        pickUpSprite.color = new Color(255, 255, 255, 255);
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
