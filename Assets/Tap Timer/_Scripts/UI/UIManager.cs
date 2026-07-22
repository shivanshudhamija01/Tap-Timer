using UnityEngine;

/// <summary>
/// Owns panel visibility only. Subscribes to notification events via the
/// generic EventBus. RoundStartedEvent covers both a fresh start and a
/// restart (both flow through GameManager.StartGame()), so there's a single
/// handler for both instead of separate GameStarted/GameRestart handlers.
/// </summary>
public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gamePlayPanel;
    [SerializeField] private GameObject instructionPanel;
    [SerializeField] private GameObject gamePausePanel;
    [SerializeField] private GameObject gameOverPanel;

    private void Awake()
    {
        mainMenuPanel.SetActive(true);
        gamePlayPanel.SetActive(false);
        instructionPanel.SetActive(false);
        gamePausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    private void OnEnable()
    {
        EventBus<PlayRequestedEvent>.Subscribe(HandlePlayRequested);
        EventBus<RoundStartedEvent>.Subscribe(HandleRoundStarted);
        EventBus<PausedEvent>.Subscribe(HandlePaused);
        EventBus<ResumedEvent>.Subscribe(HandleResumed);
        EventBus<MissEvent>.Subscribe(HandleMiss);
    }

    private void OnDisable()
    {
        EventBus<PlayRequestedEvent>.Unsubscribe(HandlePlayRequested);
        EventBus<RoundStartedEvent>.Unsubscribe(HandleRoundStarted);
        EventBus<PausedEvent>.Unsubscribe(HandlePaused);
        EventBus<ResumedEvent>.Unsubscribe(HandleResumed);
        EventBus<MissEvent>.Unsubscribe(HandleMiss);
    }

    private void HandlePlayRequested(PlayRequestedEvent e)
    {
        mainMenuPanel.SetActive(false);
        instructionPanel.SetActive(true);
    }

    private void HandleRoundStarted(RoundStartedEvent e)
    {
        instructionPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        gamePausePanel.SetActive(false);
        gamePlayPanel.SetActive(true);
    }

    private void HandlePaused(PausedEvent e) => gamePausePanel.SetActive(true);

    private void HandleResumed(ResumedEvent e) => gamePausePanel.SetActive(false);

    private void HandleMiss(MissEvent e) => gameOverPanel.SetActive(true);
}