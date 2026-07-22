using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuPanel : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private TextMeshProUGUI bestScoreTxt;
    private const string BestScoreKey = "TapTimer_BestScore";

    private void Awake()
    {
        playButton.onClick.AddListener(OnPlayButtonClicked);
        musicButton.onClick.AddListener(OnMusicButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);
    }
    void Start()
    {
        int bestScore = PlayerPrefs.GetInt(BestScoreKey, 0);
        bestScoreTxt.text = bestScore.ToString();
    }

    void OnPlayButtonClicked()
    {
        GameEvents.OnPlayPressed();
        gameObject.SetActive(false);
    }
    void OnMusicButtonClicked()
    {

    }

    void OnExitButtonClicked()
    {

    }
}
