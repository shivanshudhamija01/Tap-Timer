using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Draws the target zone as a radial-filled ring segment and provides
/// the hit-test used by TapInputHandler. Uses the same 0-360 clockwise
/// angle convention as NeedleController.
/// </summary>
public class ZoneController : MonoBehaviour
{
    [SerializeField] private Image zoneImage; // Image.Type = Filled, Fill Method = Radial 360, clockwise
    [SerializeField] private RectTransform zoneRect;

    [Header("Round caps (small solid circle sprites, optional)")]
    [SerializeField] private RectTransform capStart;
    [SerializeField] private RectTransform capEnd;
    [SerializeField] private float ringRadius = 116f; // distance from dial center to ring's stroke centerline

    public float ZoneCenter { get; private set; }
    public float ZoneWidth { get; private set; }

    public void SetZone(float centerDegrees, float widthDegrees)
    {
        ZoneCenter = centerDegrees;
        ZoneWidth = widthDegrees;

        zoneImage.fillAmount = widthDegrees / 360f;

        // Rotate the whole ring segment so its center sits at ZoneCenter.
        // Offset by half the width since fillOrigin starts the arc at rotation 0.
        float startAngle = centerDegrees - widthDegrees / 2f;
        zoneRect.localRotation = Quaternion.Euler(0f, 0f, -startAngle);

        PositionCaps(startAngle, startAngle + widthDegrees);
    }

    private void PositionCaps(float startAngle, float endAngle)
    {
        if (capStart == null || capEnd == null) return;

        capStart.anchoredPosition = AngleToPoint(startAngle);
        capEnd.anchoredPosition = AngleToPoint(endAngle);
    }

    private Vector2 AngleToPoint(float angleDegrees)
    {
        // 0 = up, clockwise, matches NeedleController's convention.
        float rad = angleDegrees * Mathf.Deg2Rad;
        return new Vector2(Mathf.Sin(rad), Mathf.Cos(rad)) * ringRadius;
    }

    public void RandomizeZone(float widthDegrees)
    {
        float center = Random.Range(0f, 360f);
        SetZone(center, widthDegrees);
    }

    // Here i need to add the width of the small cap end to the width of the ring,
    public bool IsAngleInZone(float angle)
    {
        Debug.Log("Angle is : " + angle);
        float diff = Mathf.Abs(Mathf.DeltaAngle(angle, ZoneCenter));
        return diff <= ZoneWidth / 2f;
    }
}