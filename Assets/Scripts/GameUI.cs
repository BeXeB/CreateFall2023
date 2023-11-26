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
    [SerializeField] private TMP_Text player1TimeText;
    [SerializeField] private TMP_Text player2TimeText;
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
        actionText.text = $"Remaining Actions: {remainingActions}\nTurn: {(isWhiteTurn ? "Red" : "Blue")}";
    }
    
    public void SetRemainingTimeText(float remainingTime, bool whiteTurn)
    {
        if (whiteTurn)
        {
            player1TimeText.text = $"(Red Time)\n{remainingTime:0.00}";
        }
        else
        {
            player2TimeText.text = $"(Blue Time)\n{remainingTime:0.00}";
        }
    }

    public void GameOver(bool isWhiteTurn)
    {
        gameOverText.text = $"Game Over, Won By {(isWhiteTurn ? "Red" : "Blue")}";
        gameOverPanel.SetActive(true);
    }
    
    private void HandleRestartButtonClicked()
    {
        AudioMananger.instance.PlayAudioClip("Shoot");
        SceneManager.LoadScene(1);
    }
    
    private void HandleQuitButtonClicked()
    {
        AudioMananger.instance.PlayAudioClip("Shoot");
        AudioMananger.instance.PlayMusicClip("Menu");
        SceneManager.LoadScene(0);
    }
}
