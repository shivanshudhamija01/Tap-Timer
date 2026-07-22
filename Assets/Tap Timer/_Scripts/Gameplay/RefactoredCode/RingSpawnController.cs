using UnityEngine;

public enum GamePhase { SingleRing, MultiRing, Chaos }

public class RingSpawnController : MonoBehaviour
{
    [SerializeField] private RingController[] rings = new RingController[3]; // index order = spawn/sibling order

    private const int GapSamples = 360; // 1-degree resolution for empty-space scan

    public RingController[] Rings => rings;
    public int ActiveRingCount { get; private set; }
    public GamePhase CurrentPhase { get; private set; } = GamePhase.SingleRing;

    public void ResetAndActivateFirst()
    {
        foreach (var r in rings) r.Deactivate();
        ActiveRingCount = 0;
        CurrentPhase = GamePhase.SingleRing;
        ActivateNextRing();
    }

    public void SetAllPaused(bool paused)
    {
        for (int i = 0; i < ActiveRingCount; i++) rings[i].SetPaused(paused);
    }

    /// <summary>Call after every successful hit to check whether a new ring should spawn or chaos should begin.</summary>
    public void HandleRingProgression()
    {
        if (CurrentPhase == GamePhase.Chaos) return; // pulsing is continuous and self-managed from here on

        var currentRing = rings[ActiveRingCount - 1];
        if (!currentRing.AtMinWidth) return;

        if (ActiveRingCount < 3)
        {
            ActivateNextRing();

            for (int i = 0; i < ActiveRingCount; i++)
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
        ActiveRingCount++;
        rings[ActiveRingCount - 1].Activate(spawnAngle);
    }

    // Scans the circle at 1-degree resolution, finds the largest arc not
    // currently covered by any active ring's zone, and returns its midpoint.
    private float FindEmptySpotAngle()
    {
        if (ActiveRingCount == 0) return 0f;

        bool[] covered = new bool[GapSamples];
        for (int i = 0; i < ActiveRingCount; i++)
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
}