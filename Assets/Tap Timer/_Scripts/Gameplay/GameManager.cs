using UnityEngine;

public enum GameState { Idle, Playing, GameOver }

/// <summary>
/// Drives the Tap Timer game loop. First miss ends the run (no lives),
/// matching the state-machine pattern used in the dustbin catcher game.
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField] private NeedleController needle;
    [SerializeField] private ZoneController zone;
    [SerializeField] private TapInputHandler input;
    [SerializeField] private DifficultyConfig difficulty;

    public GameState CurrentState { get; private set; } = GameState.Idle;

    private int round;
    private int score;

    private void OnEnable() => input.OnTap += HandleTap;
    private void OnDisable() => input.OnTap -= HandleTap;

    private void Start()
    {
        StartGame();
    }
    public void StartGame()
    {
        round = 1;
        score = 0;
        CurrentState = GameState.Playing;

        StartRound();
    }

    private void StartRound()
    {
        needle.ResetAngle(0f);
        needle.Speed = difficulty.GetSpeedForRound(round);
        zone.RandomizeZone(difficulty.GetZoneWidthForRound(round));
        needle.SetRunning(true);

        GameEvents.RoundStart(round);
    }

    private void HandleTap()
    {
        if (CurrentState != GameState.Playing) return;

        if (zone.IsAngleInZone(needle.CurrentAngle))
        {
            score += 10;
            GameEvents.Hit(score);

            round++;
            StartRound();
        }
        else
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        CurrentState = GameState.GameOver;
        needle.SetRunning(false);
        GameEvents.Miss();
        GameEvents.GameOver(score, round);
    }
}