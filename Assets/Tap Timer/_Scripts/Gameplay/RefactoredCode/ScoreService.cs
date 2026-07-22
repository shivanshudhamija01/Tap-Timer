using UnityEngine;
public class ScoreService
{
    private const string BestScoreKey = "TapTimer_BestScore";

    public int Score { get; private set; }
    public int TotalHits { get; private set; }
    public int BestScore { get; private set; }

    public ScoreService()
    {
        BestScore = PlayerPrefs.GetInt(BestScoreKey, 0);
    }

    public void ResetRun()
    {
        Score = 0;
        TotalHits = 0;
    }

    public void RegisterHit()
    {
        Score += 10;
        TotalHits++;
    }

    /// <summary>Call once when a run ends. Returns true if this run set a new best.</summary>
    public bool FinalizeRun()
    {
        bool isNewBest = Score > BestScore;
        if (isNewBest)
        {
            BestScore = Score;
            PlayerPrefs.SetInt(BestScoreKey, BestScore);
            PlayerPrefs.Save();
        }
        return isNewBest;
    }
}