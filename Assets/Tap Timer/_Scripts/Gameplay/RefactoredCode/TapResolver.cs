/// <summary>
/// Pure logic for resolving a tap against the active rings. Stateless by
/// design — takes everything it needs as parameters, so it's trivial to
/// unit test without a scene.
/// </summary>
public static class TapResolver
{
    /// <summary>
    /// Returns the topmost active ring (checked from last-spawned/frontmost
    /// down to first) whose zone contains the given angle, or null if none do.
    /// </summary>
    public static RingController FindTopmostHit(RingController[] rings, int activeCount, float needleAngle, float forgivenessDegrees)
    {
        for (int i = activeCount - 1; i >= 0; i--)
        {
            if (rings[i].IsAngleInZone(needleAngle, forgivenessDegrees))
                return rings[i];
        }
        return null;
    }
}