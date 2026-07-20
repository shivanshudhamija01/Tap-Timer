using UnityEngine;
using UnityEngine.UI;

public class MainMenuPanel : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button exitButton;

    private void Awake()
    {
        playButton.onClick.AddListener(OnPlayButtonClicked);
        musicButton.onClick.AddListener(OnMusicButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);
    }

    void OnPlayButtonClicked()
    {
        GameEvents.OnGameStart?.Invoke();
        gameObject.SetActive(false);
    }
    void OnMusicButtonClicked()
    {

    }

    void OnExitButtonClicked()
    {

    }
}
