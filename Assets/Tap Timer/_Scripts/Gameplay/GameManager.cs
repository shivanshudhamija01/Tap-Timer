using UnityEngine;

public enum GameState { Idle, Playing, Paused, GameOver }

public class GameManager : MonoBehaviour
{
    [SerializeField] private NeedleController needle;
    [SerializeField] private RingSpawnController ringSpawner;
    [SerializeField] private TapInputHandler input;
    [SerializeField] private DifficultyConfig difficulty;
    [SerializeField] private float edgeForgivenessDegrees = 2f;

    private ScoreService scoreService;

    public GameState CurrentState { get; private set; } = GameState.Idle;
    public int BestScore => scoreService.BestScore;

    private void Awake()
    {
        scoreService = new ScoreService();
    }

    private void OnEnable()
    {
        EventBus<StartGameRequestedEvent>.Subscribe(OnStartGameRequested);
        EventBus<RestartRequestedEvent>.Subscribe(OnRestartRequested);
        EventBus<PauseRequestedEvent>.Subscribe(OnPauseRequested);
        EventBus<ResumeRequestedEvent>.Subscribe(OnResumeRequested);
        input.OnTap += HandleTap;
    }

    private void OnDisable()
    {
        EventBus<StartGameRequestedEvent>.Unsubscribe(OnStartGameRequested);
        EventBus<RestartRequestedEvent>.Unsubscribe(OnRestartRequested);
        EventBus<PauseRequestedEvent>.Unsubscribe(OnPauseRequested);
        EventBus<ResumeRequestedEvent>.Unsubscribe(OnResumeRequested);
        input.OnTap -= HandleTap;
    }

    private void OnStartGameRequested(StartGameRequestedEvent e) => StartGame();
    private void OnRestartRequested(RestartRequestedEvent e) => RestartGame();
    private void OnPauseRequested(PauseRequestedEvent e) => PauseGame();
    private void OnResumeRequested(ResumeRequestedEvent e) => ResumeGame();

    public void StartGame()
    {
        scoreService.ResetRun();
        CurrentState = GameState.Playing;

        needle.ResetAngle(0f);
        needle.Speed = difficulty.GetSpeedForRound(1);
        needle.SetRunning(true);

        ringSpawner.ResetAndActivateFirst();

        EventBus<RoundStartedEvent>.Raise(new RoundStartedEvent { Round = 1 });
    }

    private void HandleTap()
    {
        if (CurrentState != GameState.Playing) return;

        var hitRing = TapResolver.FindTopmostHit(
            ringSpawner.Rings, ringSpawner.ActiveRingCount, needle.CurrentAngle, edgeForgivenessDegrees);

        if (hitRing == null || hitRing.Color == RingColorState.Red)
        {
            EndGame();
            return;
        }

        hitRing.RegisterHit();
        scoreService.RegisterHit();

        EventBus<HitEvent>.Raise(new HitEvent { NewScore = scoreService.Score });
        needle.Speed = difficulty.GetSpeedForRound(scoreService.TotalHits + 1);

        ringSpawner.HandleRingProgression();
    }

    public void PauseGame()
    {
        if (CurrentState != GameState.Playing) return;

        CurrentState = GameState.Paused;
        needle.SetRunning(false);
        ringSpawner.SetAllPaused(true);

        EventBus<PausedEvent>.Raise(new PausedEvent());
    }

    public void ResumeGame()
    {
        if (CurrentState != GameState.Paused) return;

        CurrentState = GameState.Playing;
        needle.SetRunning(true);
        ringSpawner.SetAllPaused(false);

        EventBus<ResumedEvent>.Raise(new ResumedEvent());
    }

    public void RestartGame() => StartGame();

    private void EndGame()
    {
        CurrentState = GameState.GameOver;
        needle.SetRunning(false);
        ringSpawner.SetAllPaused(true);

        scoreService.FinalizeRun();

        EventBus<MissEvent>.Raise(new MissEvent());
        EventBus<GameOverEvent>.Raise(new GameOverEvent
        {
            FinalScore = scoreService.Score,
            BestScore = scoreService.BestScore
        });
    }
}