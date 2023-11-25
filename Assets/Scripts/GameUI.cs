using System;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public static GameUI instance { get; private set; }
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text gameOverText;
    [SerializeField] private TMP_Text actionText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        gameOverPanel.SetActive(false);
    }

    private void OnEnable()
    {
        restartButton.onClick.AddListener(HandleRestartButtonClicked);
        quitButton.onClick.AddListener(HandleQuitButtonClicked);
    }
    
    private void OnDisable()
    {
        restartButton.onClick.RemoveListener(HandleRestartButtonClicked);
        quitButton.onClick.RemoveListener(HandleQuitButtonClicked);
    }

    public void SetRemainingActionsText(int remainingActions, bool isWhiteTurn)
    {
        actionText.text = $"Remaining Actions: {remainingActions}\nTurn: {(isWhiteTurn ? "Player 1" : "Player 2")}";
    }

    public void GameOver(bool isWhiteTurn)
    {
        gameOverText.text = $"Game Over, Won By {(isWhiteTurn ? "Player 1" : "Player 2")}";
        gameOverPanel.SetActive(true);
    }
    
    private void HandleRestartButtonClicked()
    {
        SceneManager.LoadScene(1);
    }
    
    private void HandleQuitButtonClicked()
    {
        SceneManager.LoadScene(0);
    }
}
