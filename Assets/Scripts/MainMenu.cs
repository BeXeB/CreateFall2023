using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject audioSettingsPanel;
    [SerializeField] private GameObject mainMenuPanel;
    
    [SerializeField] private Button playButton;
    [SerializeField] private Button audioSettingsButton;
    [SerializeField] private Button quitButton;
    
    [SerializeField] private Button audioSettingsBackButton;
    [SerializeField] private Slider masterVolumeSlider;
    
    private void Awake()
    {
        mainMenuPanel.SetActive(true);
        audioSettingsPanel.SetActive(false);
        
        masterVolumeSlider.value = AudioListener.volume;
    }

    private void OnEnable()
    {
        playButton.onClick.AddListener(HandlePlayButtonClicked);
        audioSettingsButton.onClick.AddListener(HandleAudioSettingsButtonClicked);
        quitButton.onClick.AddListener(HandleQuitButtonClicked);
        
        audioSettingsBackButton.onClick.AddListener(HandleAudioSettingsBackButtonClicked);
        masterVolumeSlider.onValueChanged.AddListener(HandleMasterVolumeSliderValueChanged);
    }
    
    private void OnDisable()
    {
        playButton.onClick.RemoveListener(HandlePlayButtonClicked);
        audioSettingsButton.onClick.RemoveListener(HandleAudioSettingsButtonClicked);
        quitButton.onClick.RemoveListener(HandleQuitButtonClicked);
        
        audioSettingsBackButton.onClick.RemoveListener(HandleAudioSettingsBackButtonClicked);
        masterVolumeSlider.onValueChanged.RemoveListener(HandleMasterVolumeSliderValueChanged);
    }

    private void HandlePlayButtonClicked()
    {
        AudioMananger.instance.PlayAudioClip("Shoot");
        AudioMananger.instance.PlayMusicClip("Game");
        SceneManager.LoadScene("Chess");
    }
    
    private void HandleAudioSettingsButtonClicked()
    {
        AudioMananger.instance.PlayAudioClip("Shoot");
        audioSettingsPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }
    
    private void HandleQuitButtonClicked()
    {
        AudioMananger.instance.PlayAudioClip("Shoot");
        Application.Quit();
    }
    
    private void HandleAudioSettingsBackButtonClicked()
    {
        AudioMananger.instance.PlayAudioClip("Shoot");
        audioSettingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
    
    private void HandleMasterVolumeSliderValueChanged(float value)
    {
        AudioListener.volume = value;
    }
}
