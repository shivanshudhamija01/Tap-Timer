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
        EventBus<HitEvent>.Subscribe(UpdateScore);
        EventBus<RestartRequestedEvent>.Subscribe(ResetScore);
    }
    void OnDisable()
    {
        EventBus<HitEvent>.Unsubscribe(UpdateScore);
        EventBus<RestartRequestedEvent>.Unsubscribe(ResetScore);
    }
    void OnGamePause()
    {
        EventBus<ButtonClicked>.Raise(new ButtonClicked());
        EventBus<PauseRequestedEvent>.Raise(new PauseRequestedEvent());
        Debug.Log("Game is Paused");
    }

    void UpdateScore(HitEvent e)
    {
        scoreText.text = e.NewScore.ToString();
    }
    void ResetScore(RestartRequestedEvent e)
    {
        scoreText.text = "0";
    }

}
