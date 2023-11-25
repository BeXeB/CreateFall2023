using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    
    [SerializeField] private int rows = 12;
    [SerializeField] private int columns = 8;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Color highlightTileColor;
    [SerializeField] private Color highlightAttackColor;
    [SerializeField] private Color defaultHighlightColor;
    [SerializeField] private Piece[] pieces;
    
    private bool whiteTurn = true;
    private int whiteMove = 0;
    private int blackMove = 0;
    private Camera camera;
    private Controls controls;
    
    private Tile[,] board;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        camera = Camera.main;
        controls = new Controls();
        DrawBoard();
    }
    
    private void OnEnable()
    {
        controls.Enable();
        controls.Chess.Click.performed += CheckIfMouseIsOverTile;
    }
    
    private void OnDisable()
    {
        controls.Chess.Click.performed -= CheckIfMouseIsOverTile;
        controls.Disable();
    }

    private void CheckIfMouseIsOverTile(InputAction.CallbackContext context)
    {
        var ray = camera.ScreenPointToRay(Input.mousePosition);
        
        if (!Physics.Raycast(ray, out var hit, Mathf.Infinity)) return;
        
        var tile = hit.collider.GetComponent<Tile>();
        if (tile == null) return;
        
        SelectTile(tile);
    }
    
    private void SelectTile(Tile tile)
    {
        if (tile.piece == null) return;
        
        var piece = tile.piece;
        
        if (piece.isWhite != whiteTurn) return;
        
        HighlightMoves(piece, tile);
    }

    private void HighlightMoves(Piece piece, Tile tile)
    {
        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < columns; col++)
            { 
                var tileScript = board[row, col];
                tileScript.GetComponentsInChildren<SpriteRenderer>()[1].color = defaultHighlightColor;
            }
        }
        
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
                    tileScript.GetComponentsInChildren<SpriteRenderer>()[1].color = highlightTileColor;
                    continue;
                }
                if (manhattanDistance <= attacks)
                {
                    tileScript.GetComponentsInChildren<SpriteRenderer>()[1].color = highlightAttackColor;
                }
            }
        }
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
        
        // White pieces
        // 0,2 0,5 horse
        // 0,3 0,4 sniper
        // 1,2 1,5 barrel
        // 1,3 1,4 plebs
        
        // Black pieces
        // 11,2 10,5 horse
        // 11,3 10,4 sniper
        // 10,2 11,5 barrel
        // 10,3 11,4 plebs
        
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
    }
    
}
