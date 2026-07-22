using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GamePlayPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Button pauseButton;

    void Awake()
    {
        pauseButton.onClick.AddListener(OnGamePause);
    }
    void OnEnable()
    {
        // GameEvents.OnHit += UpdateScore;
        EventBus<HitEvent>.Subscribe(UpdateScore);
        // GameEvents.OnGameRestart += ResetScore;
        EventBus<RestartRequestedEvent>.Subscribe(ResetScore);
    }
    void OnDisable()
    {
        // GameEvents.OnHit -= UpdateScore;
        EventBus<HitEvent>.Unsubscribe(UpdateScore);
        // GameEvents.OnGameRestart -= ResetScore;
        EventBus<RestartRequestedEvent>.Unsubscribe(ResetScore);
    }
    void OnGamePause()
    {
        // GameEvents.Pause();
        EventBus<ButtonClicked>.Raise(new ButtonClicked());
        EventBus<PauseRequestedEvent>.Raise(new PauseRequestedEvent());
        Debug.Log("Game is Paused");
    }
    // void UpdateScore(int score)
    // {
    //     scoreText.text = score.ToString();
    // }
    void UpdateScore(HitEvent e)
    {
        scoreText.text = e.NewScore.ToString();
    }
    void ResetScore(RestartRequestedEvent e)
    {
        scoreText.text = "0";
    }
    // void ResetScore()
    // {
    //     scoreText.text = "0";
    // }
}
