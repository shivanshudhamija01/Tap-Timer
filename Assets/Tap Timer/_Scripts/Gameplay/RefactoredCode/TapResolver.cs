public static class TapResolver
{

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