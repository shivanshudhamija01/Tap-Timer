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
        // GameEvents.OnGameOver += UpdateScore;
        EventBus<GameOverEvent>.Subscribe(UpdateScore);

    }
    private void OnDisable()
    {
        // GameEvents.OnGameOver -= UpdateScore;
        EventBus<GameOverEvent>.Unsubscribe(UpdateScore);
    }
    // void UpdateScore(int score, int bestScore)
    // {
    //     scoreText.text = score.ToString();
    //     bestScoreText.text = bestScore.ToString();
    // }
    void UpdateScore(GameOverEvent e)
    {
        scoreText.text = e.FinalScore.ToString();
        bestScoreText.text = e.BestScore.ToString();
    }
    void OnGameRestart()
    {
        // GameEvents.Restart();
        EventBus<ButtonClicked>.Raise(new ButtonClicked());
        EventBus<RestartRequestedEvent>.Raise(new RestartRequestedEvent());
    }
    void OnQuit()
    {
        EventBus<ButtonClicked>.Raise(new ButtonClicked());
        Application.Quit();
    }

}
