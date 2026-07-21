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
        GameEvents.Resume();
    }
    void RestartGame()
    {
        GameEvents.Restart();
    }
    void QuitGame()
    {
        Application.Quit();
    }
}
