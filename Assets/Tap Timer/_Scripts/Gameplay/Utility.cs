using System;

/// <summary>
/// Simple static event bus for Tap Timer. Follows the same pattern used
/// in the dustbin catcher / UFO abduction games for decoupled UI + gameplay.
/// </summary>
public static class GameEvents
{
    public static event Action<int> OnRoundStart;      // round number
    public static event Action<int> OnHit;              // new score
    public static event Action OnMiss;
    public static event Action<int, int> OnGameOver;     // finalScore, bestStreak/round reached
    public static Action OnGameStart;
    public static void RoundStart(int round) => OnRoundStart?.Invoke(round);
    public static void Hit(int newScore) => OnHit?.Invoke(newScore);
    public static void Miss() => OnMiss?.Invoke();
    public static void GameOver(int finalScore, int roundReached) => OnGameOver?.Invoke(finalScore, roundReached);
}