using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [SerializeField] private int rows = 12;
    [SerializeField] private int columns = 8;
    [SerializeField] private GameObject tilePrefab;
    
    private bool _whiteTurn = true;
    
    private Tile[,] _board;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        
        DrawBoard();
    }
    
    private void DrawBoard()
    {
        _board = new Tile[rows,columns];

        //Instantiate Tiles and move to coords
        
    }
}
