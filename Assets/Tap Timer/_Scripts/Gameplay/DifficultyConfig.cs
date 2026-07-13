using UnityEngine;

[CreateAssetMenu(fileName = "DifficultyConfig", menuName = "TapTimer/Difficulty Config")]
public class DifficultyConfig : ScriptableObject
{
    [Header("Needle Speed (degrees/sec)")]
    public float baseSpeed = 132f;      // 2.2 deg/frame * 60fps from the prototype
    public float speedPerRound = 15f;
    public float maxSpeed = 360f;

    [Header("Zone Width (degrees)")]
    public float baseZoneWidth = 70f;
    public float zoneShrinkPerRound = 6f;
    public float minZoneWidth = 18f;

    public float GetSpeedForRound(int round)
    {
        float speed = baseSpeed + speedPerRound * (round - 1);
        return Mathf.Min(speed, maxSpeed);
    }

    public float GetZoneWidthForRound(int round)
    {
        float width = baseZoneWidth - zoneShrinkPerRound * (round - 1);
        return Mathf.Max(width, minZoneWidth);
    }
}