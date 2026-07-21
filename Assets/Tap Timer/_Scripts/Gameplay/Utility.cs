using System;

/// <summary>
/// Static event bus for Tap Timer. UI panels subscribe to these instead of
/// polling GameManager state every frame.
/// </summary>
public static class GameEvents
{
    public static event Action<int> OnRoundStart;                     // round number
    public static event Action<int> OnHit;                             // new score
    public static event Action OnMiss;
    public static event Action<int, int> OnGameOver;        // finalScore, bestScore
    public static event Action OnPause;
    public static event Action OnResume;
    public static event Action OnGameRestart;
    public static event Action OnGameStarted;
    public static event Action OnPlay;

    public static void RoundStart(int round) => OnRoundStart?.Invoke(round);
    public static void Hit(int newScore) => OnHit?.Invoke(newScore);
    public static void Miss() => OnMiss?.Invoke();
    public static void GameOver(int finalScore, int bestScore) =>
        OnGameOver?.Invoke(finalScore, bestScore);
    public static void Pause() => OnPause?.Invoke();
    public static void Resume() => OnResume?.Invoke();
    public static void Restart() => OnGameRestart?.Invoke();
    public static void OnGameStart() => OnGameStarted?.Invoke();
    public static void OnPlayPressed() => OnPlay?.Invoke();
}