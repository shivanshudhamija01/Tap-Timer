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
        EventBus<GameOverEvent>.Subscribe(UpdateScore);

    }
    private void OnDisable()
    {
        EventBus<GameOverEvent>.Unsubscribe(UpdateScore);
    }
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
