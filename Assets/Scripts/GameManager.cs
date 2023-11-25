using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    
    [SerializeField] private int rows = 12;
    [SerializeField] private int columns = 8;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Color highlightTileColor;
    [SerializeField] private Color highlightAttackColor;
    [SerializeField] private Color defaultHighlightColor;
    [SerializeField] private Color controlPointColor;
    [SerializeField] private Color highlightReloadColor;
    [SerializeField] private Piece pieceBaseStat;
    [SerializeField] private ControlPoint[] controlPoints;
    [SerializeField] private EquipmentSprite[] equipmentSprites;
    [SerializeField] private float player1Time = 3*60;
    [SerializeField] private float player1TimeIncrement = 2f;
    [SerializeField] private float player2Time = 3*60;
    [SerializeField] private float player2TimeIncrement = 2f;
    [SerializeField] private List<Sprite> tileSprites;
    
    private float player1TimeRemaining;
    private float player2TimeRemaining;
    
    private Camera mainCamera;
    private Controls controls;
    
    private bool whiteTurn = true;
    private int whiteMove;
    private int blackMove;
    private Tile selectedTile;
    private bool gameOver;
    
    private Tile[,] board;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        mainCamera = Camera.main;
        controls = new Controls();
        player1TimeRemaining = player1Time;
        player2TimeRemaining = player2Time;
        DrawBoard();
    }
    
    public Sprite GetEquipmentSprite(EquipmentType equipmentType, bool isWhite)
    {
        var sprite = equipmentSprites.First(e => e.type == equipmentType);
        return isWhite ? sprite.spriteTeam1 : sprite.spriteTeam2;
    }
    
    public Sprite GetPickUpSprite(EquipmentType equipmentType)
    {
        return equipmentSprites.First(e => e.type == equipmentType).spritePickUp;
    }
    
    public Color GetHighlightColor(TileState state)
    {
        return state switch
        {
            TileState.Default => defaultHighlightColor,
            TileState.Highlighted => highlightTileColor,
            TileState.Attack => highlightAttackColor,
            TileState.Reload => highlightReloadColor,
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        };
    }
    
    private void OnEnable()
    {
        controls.Enable();
        controls.Chess.LClick.performed += CheckIfMouseIsOverTile;
        controls.Chess.RClick.performed += HandleClearInput;
    }
    
    private void OnDisable()
    {
        controls.Chess.LClick.performed -= CheckIfMouseIsOverTile;
        controls.Chess.RClick.performed -= HandleClearInput;
        controls.Disable();
    }

    private void Start()
    {
        GameUI.instance.SetRemainingTimeText(player1TimeRemaining, true);
        GameUI.instance.SetRemainingTimeText(player2TimeRemaining, false);
    }

    private void CheckIfMouseIsOverTile(InputAction.CallbackContext context)
    {
        var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        
        if (!Physics.Raycast(ray, out var hit, Mathf.Infinity)) return;
        
        var tile = hit.collider.GetComponent<Tile>();
        if (tile == null) return;
        
        if (selectedTile == null)
        {
            SelectTile(tile);
            return;
        }
        
        switch(tile.state)
        {
            case TileState.Default: 
                ClearSelection(); 
                break;
            case TileState.Highlighted: 
                MovePiece(tile); 
                MoveMade();
                break;
            case TileState.Attack:
                AttackPiece(tile);
                MoveMade();
                break;
            case TileState.Reload:
                ReloadPiece(tile);
                MoveMade();
                break;
            default: 
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private void Update()
    {
        if (whiteMove == 0 && blackMove == 0) return;
        if (gameOver) return;
        if (whiteTurn)
        {
            player1TimeRemaining -= Time.deltaTime;
            GameUI.instance.SetRemainingTimeText(player1TimeRemaining, whiteTurn);
            if (!(player1TimeRemaining <= 0)) return;
            player1TimeRemaining = 0;
            GameOver(false);
        }
        else
        {
            player2TimeRemaining -= Time.deltaTime;
            GameUI.instance.SetRemainingTimeText(player2TimeRemaining, whiteTurn);
            if (!(player2TimeRemaining <= 0)) return;
            player2TimeRemaining = 0;
            GameOver(true);
        }
    }

    private void MoveMade()
    {
        if (whiteTurn) whiteMove++;
        else blackMove++;
        
        switch (whiteTurn)
        {
            case true when whiteMove % 2 == 0:
            case false when blackMove % 2 == 1:
                GameUI.instance.SetRemainingActionsText(1, whiteTurn);
                break;
            case true when whiteMove % 2 == 1:
            case false when blackMove % 2 == 0:
                foreach (var tileScript in board)
                {   
                    if (tileScript.piece == null) continue;
                    tileScript.piece.movedThisTurn = false;
                }

                whiteTurn = !whiteTurn;

                if (whiteTurn)
                {
                    player1TimeRemaining += player1TimeIncrement;
                    GameUI.instance.SetRemainingTimeText(player1TimeRemaining, whiteTurn);
                }
                else
                {
                    player2TimeRemaining += player2TimeIncrement;
                    GameUI.instance.SetRemainingTimeText(player2TimeRemaining, whiteTurn);
                }

                var controlled = true;
                foreach (var point in controlPoints)
                {
                    if (!(whiteTurn ? point.IsControlledByWhite() : point.IsControlledByBlack()))
                    {
                        controlled = false;
                    }
                }
                
                if (controlled)
                {
                    GameOver(whiteTurn);
                    return;
                }
                
                GameUI.instance.SetRemainingActionsText(2, whiteTurn);
                
                break;
        }
    }

    private void GameOver(bool whiteTurn)
    {
        GameUI.instance.GameOver(whiteTurn);
        controls.Disable();
        gameOver = true;
    }

    private void SelectTile(Tile tile)
    {
        ClearSelection();
        
        if (tile.piece == null) return;
        
        var piece = tile.piece;
        
        if (piece.isWhite != whiteTurn) return;
        
        if (piece.movedThisTurn) return;
        
        selectedTile = tile;
        HighlightMoves(piece, tile);
        if (piece.attacked)
        {
            tile.SetState(TileState.Reload);
        }
    }

    private void HighlightMoves(Piece piece, Tile tile)
    {
        var (x, y) = (tile.row, tile.column);
        var moves = piece.move;
        var attacks = piece.attack;
        
        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < columns; col++)
            { 
                var manhattanDistance = Mathf.Abs(row - x) + Mathf.Abs(col - y);
                var tileScript = board[row, col];
                
                if (tileScript.piece != null && tileScript.piece.isWhite == piece.isWhite) continue;
                
                if (manhattanDistance <= moves)
                {
                    tileScript.SetState(TileState.Highlighted);
                    continue;
                }
                if (!piece.attacked && manhattanDistance <= attacks)
                {
                    tileScript.SetState(TileState.Attack);
                }
            }
        }
    }

    private void MovePiece(Tile tileToMoveTo)
    {
        var piece = selectedTile.piece;
        tileToMoveTo.ClearPiece(drop: true);
        tileToMoveTo.SetPiece(piece, piece.isWhite);
        selectedTile.ClearPiece();
        if (tileToMoveTo.pickUp != EquipmentType.None)
        {
            var oldEquipment = piece.equipmentType;
            piece.Unequip();
            piece.Equip(tileToMoveTo.pickUp);
            tileToMoveTo.pickUp = oldEquipment;
        }
        piece.movedThisTurn = true;
        ClearSelection();
    }
    
    private void AttackPiece(Tile tileToAttack)
    {
        var piece = selectedTile.piece;
        
        piece.attacked = true;
        
        var attackedPiece = tileToAttack.piece;
        if (attackedPiece == null) return;
        switch (attackedPiece.equipmentType)
        {
            case EquipmentType.Barrel:
                tileToAttack.piece.Unequip();
                break;
            default:
                tileToAttack.ClearPiece(drop: true);
                break;
        }
        piece.movedThisTurn = true;
        
        if (piece.equipmentType == EquipmentType.Gun)
        {
            piece.Unequip();
        }
        
        ClearSelection();
    }

    private void ReloadPiece(Tile tile)
    {
        var piece = tile.piece;
        piece.attacked = false;
        piece.movedThisTurn = true;
        ClearSelection();
    }
    
    private void HandleClearInput(InputAction.CallbackContext context)
    {
        ClearSelection();
    }
    
    private void ClearSelection()
    {
        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < columns; col++)
            { 
                var tileScript = board[row, col];
                tileScript.SetState(TileState.Default);
            }
        }
        selectedTile = null;
    }

    private void DrawBoard()
    {
        board = new Tile[rows,columns];
        
        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < columns; col++)
            { 
                var tile = Instantiate(tilePrefab, transform);
                tile.transform.position = new Vector3(col, row, 0);
                var tileScript = tile.GetComponent<Tile>();
                tileScript.row = row;
                tileScript.column = col;
                board[row, col] = tileScript;
                
                var spriteRenderer = tile.GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = tileSprites[UnityEngine.Random.Range(0, tileSprites.Count)];
            }
        }

        var cameraTransform = mainCamera.transform;
        cameraTransform.position = new Vector3(columns/2f-.5f, rows/2f-.5f, cameraTransform.position.z);
        
       foreach (var controlPoint in controlPoints)
        {
            foreach (var coordinate in controlPoint.coordinates)
            {
                board[coordinate.row, coordinate.column].GetComponent<SpriteRenderer>().color = controlPointColor;
                controlPoint.tiles.Add(board[coordinate.row, coordinate.column]);
            }
        }
        
        board[0, 2].SetPiece((Piece)pieceBaseStat.Clone(), true, EquipmentType.Horse);
        board[0, 5].SetPiece((Piece)pieceBaseStat.Clone(), true, EquipmentType.Horse);
        board[1, 1].SetPiece((Piece)pieceBaseStat.Clone(), true, EquipmentType.None);
        board[1, 6].SetPiece((Piece)pieceBaseStat.Clone(), true, EquipmentType.None);
        board[1, 2].SetPiece((Piece)pieceBaseStat.Clone(), true, EquipmentType.Barrel);
        board[1, 5].SetPiece((Piece)pieceBaseStat.Clone(), true, EquipmentType.Barrel);
        board[1, 3].SetPiece((Piece)pieceBaseStat.Clone(), true, EquipmentType.Sniper);
        board[1, 4].SetPiece((Piece)pieceBaseStat.Clone(), true, EquipmentType.Sniper);
        
        board[11, 2].SetPiece((Piece)pieceBaseStat.Clone(), false, EquipmentType.Horse);
        board[11, 5].SetPiece((Piece)pieceBaseStat.Clone(), false, EquipmentType.Horse);
        board[10, 1].SetPiece((Piece)pieceBaseStat.Clone(), false, EquipmentType.None);
        board[10, 6].SetPiece((Piece)pieceBaseStat.Clone(), false, EquipmentType.None);
        board[10, 2].SetPiece((Piece)pieceBaseStat.Clone(), false, EquipmentType.Barrel);
        board[10, 5].SetPiece((Piece)pieceBaseStat.Clone(), false, EquipmentType.Barrel);
        board[10, 3].SetPiece((Piece)pieceBaseStat.Clone(), false, EquipmentType.Sniper);
        board[10, 4].SetPiece((Piece)pieceBaseStat.Clone(), false, EquipmentType.Sniper);
        
        GameUI.instance.SetRemainingActionsText(1, whiteTurn);
    }
}


[Serializable]
public struct EquipmentSprite
{
    public EquipmentType type;
    public Sprite spriteTeam1;
    public Sprite spriteTeam2;
    public Sprite spritePickUp;
}