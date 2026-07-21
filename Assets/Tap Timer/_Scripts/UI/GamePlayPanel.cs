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
        GameEvents.OnHit += UpdateScore;
        GameEvents.OnGameRestart += ResetScore;
    }
    void OnDisable()
    {
        GameEvents.OnHit -= UpdateScore;
        GameEvents.OnGameRestart -= ResetScore;
    }
    void OnGamePause()
    {
        GameEvents.Pause();
        Debug.Log("Game is Paused");
    }
    void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }
    void ResetScore()
    {
        scoreText.text = "0";
    }
}
