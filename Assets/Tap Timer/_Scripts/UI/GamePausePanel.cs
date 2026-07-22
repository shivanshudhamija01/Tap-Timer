using System;
using UnityEngine;
using UnityEngine.UI;

public class GamePausePanel : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;
    private void Awake()
    {
        resumeButton.onClick.AddListener(ResumeGame);
        restartButton.onClick.AddListener(RestartGame);
        quitButton.onClick.AddListener(QuitGame);
    }
    void ResumeGame()
    {
        // GameEvents.Resume();
        EventBus<ButtonClicked>.Raise(new ButtonClicked());
        EventBus<ResumeRequestedEvent>.Raise(new ResumeRequestedEvent());
    }
    void RestartGame()
    {
        // GameEvents.Restart();
        EventBus<ButtonClicked>.Raise(new ButtonClicked());
        EventBus<RestartRequestedEvent>.Raise(new RestartRequestedEvent());
    }
    void QuitGame()
    {
        EventBus<ButtonClicked>.Raise(new ButtonClicked());
        Application.Quit();
    }
}
