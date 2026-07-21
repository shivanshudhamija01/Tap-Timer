using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI bestScoreText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    void Awake()
    {
        quitButton.onClick.AddListener(OnQuit);
        restartButton.onClick.AddListener(OnGameRestart);

    }
    private void OnEnable()
    {
        GameEvents.OnGameOver += UpdateScore;
    }
    private void OnDisable()
    {
        GameEvents.OnGameOver -= UpdateScore;
    }
    void UpdateScore(int score, int bestScore)
    {
        scoreText.text = score.ToString();
        bestScoreText.text = bestScore.ToString();
    }
    void OnGameRestart()
    {
        GameEvents.Restart();
    }
    void OnQuit()
    {
        Application.Quit();
    }

}
