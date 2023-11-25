using System;
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
    [SerializeField] private Piece[] pieces;
    [SerializeField] private ControlPoint[] controlPoints;
    
    private Camera mainCamera;
    private Controls controls;
    
    private bool whiteTurn = true;
    private int whiteMove;
    private int blackMove;
    private Tile selectedTile;
    
    private Tile[,] board;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        mainCamera = Camera.main;
        controls = new Controls();
        DrawBoard();
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
                    GameUI.instance.GameOver(whiteTurn);
                    controls.Disable();
                    //Debug.Log($"Game Over, won by {(whiteTurn ? "white" : "black")}");
                }
                
                GameUI.instance.SetRemainingActionsText(2, whiteTurn);
                
                break;
        }
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
        
        selectedTile.ClearPiece();
        tileToMoveTo.ClearPiece();
        tileToMoveTo.SetPiece(piece, piece.isWhite);
        piece.movedThisTurn = true;
        ClearSelection();
    }
    
    private void AttackPiece(Tile tileToAttack)
    {
        var piece = selectedTile.piece;
        piece.attacked = true;
        var attackedPiece = tileToAttack.piece;
        if (attackedPiece == null) return;
        switch (attackedPiece.type)
        {
            case PieceType.Barrel:
                tileToAttack.SetPiece((Piece)pieces.First(p => p.type == PieceType.Pleb).Clone(), attackedPiece.isWhite);
                break;
            default:
                tileToAttack.ClearPiece();
                break;
        }
        piece.movedThisTurn = true;
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
        
        board[0, 2].SetPiece((Piece)pieces.First(p => p.type == PieceType.Horse).Clone(), true);
        board[0, 5].SetPiece((Piece)pieces.First(p => p.type == PieceType.Horse).Clone(), true);
        board[0, 3].SetPiece((Piece)pieces.First(p => p.type == PieceType.Sniper).Clone(), true);
        board[0, 4].SetPiece((Piece)pieces.First(p => p.type == PieceType.Sniper).Clone(), true);
        board[1, 2].SetPiece((Piece)pieces.First(p => p.type == PieceType.Barrel).Clone(), true);
        board[1, 5].SetPiece((Piece)pieces.First(p => p.type == PieceType.Barrel).Clone(), true);
        board[1, 3].SetPiece((Piece)pieces.First(p => p.type == PieceType.Pleb).Clone(), true);
        board[1, 4].SetPiece((Piece)pieces.First(p => p.type == PieceType.Pleb).Clone(), true);
        
        board[11, 2].SetPiece((Piece)pieces.First(p => p.type == PieceType.Horse).Clone(), false);
        board[11, 5].SetPiece((Piece)pieces.First(p => p.type == PieceType.Horse).Clone(), false);
        board[11, 3].SetPiece((Piece)pieces.First(p => p.type == PieceType.Sniper).Clone(), false);
        board[11, 4].SetPiece((Piece)pieces.First(p => p.type == PieceType.Sniper).Clone(), false);
        board[10, 2].SetPiece((Piece)pieces.First(p => p.type == PieceType.Barrel).Clone(), false);
        board[10, 5].SetPiece((Piece)pieces.First(p => p.type == PieceType.Barrel).Clone(), false);
        board[10, 3].SetPiece((Piece)pieces.First(p => p.type == PieceType.Pleb).Clone(), false);
        board[10, 4].SetPiece((Piece)pieces.First(p => p.type == PieceType.Pleb).Clone(), false);
        
        GameUI.instance.SetRemainingActionsText(1, whiteTurn);
    }
}
