using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuPanel : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Sprite muteIcon;
    [SerializeField] private Sprite bgmIcon;
    [SerializeField] private TextMeshProUGUI bestScoreTxt;
    private Image musicButtonImage;

    private bool isMuted = false;


    private const string BestScoreKey = "TapTimer_BestScore";

    private void Awake()
    {
        musicButtonImage = musicButton.GetComponent<Image>();
        playButton.onClick.AddListener(OnPlayButtonClicked);
        musicButton.onClick.AddListener(OnMusicButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);
    }

    private void Start()
    {
        int bestScore = PlayerPrefs.GetInt(BestScoreKey, 0);
        bestScoreTxt.text = bestScore.ToString();
    }

    private void OnPlayButtonClicked()
    {
        EventBus<ButtonClicked>.Raise(new ButtonClicked());
        EventBus<PlayRequestedEvent>.Raise(new PlayRequestedEvent());
    }

    private void OnMusicButtonClicked()
    {
        EventBus<ButtonClicked>.Raise(new ButtonClicked());

        isMuted = !isMuted;

        if (isMuted)
        {
            musicButtonImage.sprite = muteIcon;
            EventBus<OnBGMToggle>.Raise(new OnBGMToggle
            {
                Volume = 0f
            });
        }
        else
        {
            musicButtonImage.sprite = bgmIcon;
            EventBus<OnBGMToggle>.Raise(new OnBGMToggle
            {
                Volume = 1f
            });
        }
    }

    private void OnExitButtonClicked()
    {
        EventBus<ButtonClicked>.Raise(new ButtonClicked());
        Application.Quit();
    }
}