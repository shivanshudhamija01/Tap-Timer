using System.Collections.Generic;
using UnityEngine;

public enum GameState { Idle, Playing, GameOver }
public enum GamePhase { SingleRing, MultiRing, Chaos }

/// <summary>
/// Orchestrates the layered ring gameplay: spawns rings 1-3 in sequence,
/// switches earlier rings into Dynamic (revolve + color-flip) mode once a
/// second ring spawns, and drives the endless "chaos" reshuffle loop once
/// all 3 rings have reached minimum width.
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField] private NeedleController needle;
    [SerializeField] private RingController[] rings = new RingController[3]; // assign 3 in Inspector, outer to inner
    [SerializeField] private TapInputHandler input;
    [SerializeField] private DifficultyConfig difficulty;
    [SerializeField] private float edgeForgivenessDegrees = 2f;

    public GameState CurrentState { get; private set; } = GameState.Idle;
    public GamePhase CurrentPhase { get; private set; } = GamePhase.SingleRing;

    private int activeRingCount;
    private int totalHits;
    private int score;

    private void OnEnable() => input.OnTap += HandleTap;
    private void OnDisable() => input.OnTap -= HandleTap;

    // TEMP: auto-start for testing. Remove once a Play button calls StartGame() instead.
    private void Start() => StartGame();

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
        activeRingCount = 1;
        rings[0].Activate();

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
        if (CurrentPhase == GamePhase.Chaos)
        {
            if (AllActiveRingsAtMin()) ReshuffleChaos();
            return;
        }

        var currentRing = rings[activeRingCount - 1];
        if (!currentRing.AtMinWidth) return;

        if (activeRingCount < 3)
        {
            activeRingCount++;
            rings[activeRingCount - 1].Activate();

            for (int i = 0; i < activeRingCount; i++)
                rings[i].SwitchToDynamic();

            CurrentPhase = GamePhase.MultiRing;
        }
        else
        {
            CurrentPhase = GamePhase.Chaos;
            ReshuffleChaos();
        }
    }

    private bool AllActiveRingsAtMin()
    {
        for (int i = 0; i < activeRingCount; i++)
            if (!rings[i].AtMinWidth) return false;
        return true;
    }

    private void ReshuffleChaos()
    {
        int redCount = Random.Range(0, activeRingCount + 1); // 0..activeRingCount reds, inclusive

        var indices = new List<int>();
        for (int i = 0; i < activeRingCount; i++) indices.Add(i);
        for (int i = indices.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (indices[i], indices[j]) = (indices[j], indices[i]);
        }

        for (int i = 0; i < activeRingCount; i++)
        {
            bool forceRed = indices.IndexOf(i) < redCount;
            rings[i].ReshuffleRandom(forceRed);
        }
    }

    private void EndGame()
    {
        CurrentState = GameState.GameOver;
        needle.SetRunning(false);
        GameEvents.Miss();
        GameEvents.GameOver(score, totalHits);
    }
}