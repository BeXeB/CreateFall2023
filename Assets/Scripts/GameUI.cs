using UnityEngine;
using TMPro;

public class GameUI : MonoBehaviour
{
    public static GameUI instance { get; private set; }
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text actionText;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        gameOverPanel.SetActive(false);
    }
    
    public void SetRemainingActionsText(int remainingActions, bool isWhiteTurn)
    {
        actionText.text = $"Remaining Actions: {remainingActions}\nTurn: {(isWhiteTurn ? "White" : "Black")}";
    }
}
