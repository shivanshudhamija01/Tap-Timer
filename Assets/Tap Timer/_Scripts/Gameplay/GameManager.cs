using UnityEngine;

public enum GameState { Idle, Playing, Paused, GameOver }
public enum GamePhase { SingleRing, MultiRing, Chaos }

/// <summary>
/// Orchestrates the layered ring gameplay: spawns rings 1-3 in sequence
/// (each new ring placed in the largest currently-uncovered gap), switches
/// earlier rings into Dynamic (revolve + color-flip) mode once a second ring
/// spawns, and enables continuous grow/shrink pulsing on all rings once all
/// 3 have been fully shrunk to minimum at least once.
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField] private NeedleController needle;
    [SerializeField] private RingController[] rings = new RingController[3]; // index order = spawn/sibling order
    [SerializeField] private TapInputHandler input;
    [SerializeField] private DifficultyConfig difficulty;
    [SerializeField] private float edgeForgivenessDegrees = 2f;

    private const int GapSamples = 360; // 1-degree resolution for empty-space scan
    private const string BestScoreKey = "TapTimer_BestScore";

    public int BestScore { get; private set; }

    public GameState CurrentState { get; private set; } = GameState.Idle;
    public GamePhase CurrentPhase { get; private set; } = GamePhase.SingleRing;

    private int activeRingCount;
    private int totalHits;
    private int score;

    // Here are all the methods that are for the game restart and resume and pause
    private void OnEnable()
    {
        GameEvents.OnGameStarted += OnGameStart;
        GameEvents.OnGameRestart += RestartGame;
        GameEvents.OnPause += PauseGame;
        GameEvents.OnResume += ResumeGame;
        input.OnTap += HandleTap;
    }
    private void OnDisable()
    {
        GameEvents.OnGameStarted -= OnGameStart;
        GameEvents.OnGameRestart -= RestartGame;
        GameEvents.OnPause -= PauseGame;
        GameEvents.OnResume -= ResumeGame;
        input.OnTap -= HandleTap;
    }

    private void Awake() => BestScore = PlayerPrefs.GetInt(BestScoreKey, 0);

    // TEMP: auto-start for testing. Remove once a Play button calls StartGame() instead.
    // private void Start() => StartGame();
    private void OnGameStart()
    {
        StartGame();
    }
    public void StartGame()
    {
        totalHits = 0;
        score = 0;
        CurrentPhase = GamePhase.SingleRing;
        CurrentState = GameState.Playing;

        needle.ResetAngle(0f);
        needle.Speed = difficulty.GetSpeedForRound(1);
        needle.SetRunning(true);

        foreach (var r in rings) r.Deactivate();
        activeRingCount = 0;
        ActivateNextRing();

        GameEvents.RoundStart(1);
    }

    private void HandleTap()
    {
        if (CurrentState != GameState.Playing) return;

        float needleAngle = needle.CurrentAngle;
        RingController topmostHit = null;

        // Check from topmost sibling (last spawned, renders on top) down to the bottom.
        for (int i = activeRingCount - 1; i >= 0; i--)
        {
            if (rings[i].IsAngleInZone(needleAngle, edgeForgivenessDegrees))
            {
                topmostHit = rings[i];
                break;
            }
        }

        if (topmostHit == null)
        {
            EndGame(); // tapped empty space, nothing to hit
            return;
        }

        if (topmostHit.Color == RingColorState.Red)
        {
            EndGame();
            return;
        }

        topmostHit.RegisterHit();
        totalHits++;
        score += 10;

        GameEvents.Hit(score);
        needle.Speed = difficulty.GetSpeedForRound(totalHits + 1);

        HandleRingProgression();
    }

    private void HandleRingProgression()
    {
        if (CurrentPhase == GamePhase.Chaos) return; // pulsing is continuous and self-managed now

        var currentRing = rings[activeRingCount - 1];
        if (!currentRing.AtMinWidth) return;

        if (activeRingCount < 3)
        {
            ActivateNextRing();

            for (int i = 0; i < activeRingCount; i++)
                rings[i].SwitchToDynamic();

            CurrentPhase = GamePhase.MultiRing;
        }
        else
        {
            CurrentPhase = GamePhase.Chaos;
            foreach (var ring in rings) ring.EnablePulse();
        }
    }

    private void ActivateNextRing()
    {
        float spawnAngle = FindEmptySpotAngle();
        activeRingCount++;
        rings[activeRingCount - 1].Activate(spawnAngle);
    }

    // Scans the circle at 1-degree resolution, finds the largest arc not
    // currently covered by any active ring's zone, and returns its midpoint.
    private float FindEmptySpotAngle()
    {
        if (activeRingCount == 0) return 0f;

        bool[] covered = new bool[GapSamples];
        for (int i = 0; i < activeRingCount; i++)
        {
            var ring = rings[i];
            float half = ring.CurrentWidth / 2f;
            for (int a = 0; a < GapSamples; a++)
            {
                if (Mathf.Abs(Mathf.DeltaAngle(a, ring.ZoneCenter)) <= half)
                    covered[a] = true;
            }
        }

        int bestStart = 0, bestLen = 0, curStart = -1, curLen = 0;
        for (int a = 0; a < GapSamples * 2; a++)
        {
            int idx = a % GapSamples;
            if (!covered[idx])
            {
                if (curStart == -1) curStart = idx;
                curLen++;
                if (curLen > bestLen)
                {
                    bestLen = curLen;
                    bestStart = curStart;
                }
            }
            else
            {
                curStart = -1;
                curLen = 0;
            }
            if (bestLen >= GapSamples) break; // fully uncovered circle, no need to keep scanning
        }

        if (bestLen == 0) return Random.Range(0f, 360f); // fully covered fallback
        return (bestStart + bestLen / 2f) % GapSamples;
    }

    public void PauseGame()
    {
        if (CurrentState != GameState.Playing) return;

        CurrentState = GameState.Paused;
        needle.SetRunning(false);
        for (int i = 0; i < activeRingCount; i++) rings[i].SetPaused(true);

        GameEvents.Pause();
    }

    public void ResumeGame()
    {
        if (CurrentState != GameState.Paused) return;

        CurrentState = GameState.Playing;
        needle.SetRunning(true);
        for (int i = 0; i < activeRingCount; i++) rings[i].SetPaused(false);

        GameEvents.Resume();
    }

    public void RestartGame() => StartGame(); // resets and unpauses everything

    private void EndGame()
    {
        CurrentState = GameState.GameOver;
        needle.SetRunning(false);

        // Freeze every active ring in place (stop revolve, color flip, and pulse).
        for (int i = 0; i < activeRingCount; i++) rings[i].SetPaused(true);

        bool isNewBest = score > BestScore;
        if (isNewBest)
        {
            BestScore = score;
            PlayerPrefs.SetInt(BestScoreKey, BestScore);
            PlayerPrefs.Save();
        }

        GameEvents.Miss();
        GameEvents.GameOver(score, BestScore);
    }
}