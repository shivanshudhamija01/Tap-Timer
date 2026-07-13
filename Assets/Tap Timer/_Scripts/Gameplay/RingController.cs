using UnityEngine;
using UnityEngine.UI;

public enum RingColorState { Green, Red }

// Static: fixed center, always green, re-randomizes per hit (phase 1 behavior).
// Dynamic: continuously revolves and flips color at random intervals (phase 2+).
public enum RingMode { Static, Dynamic }

/// <summary>
/// One shrinking/revolving target ring. Multiple instances (up to 3) are
/// orchestrated by GameManager to build the layered ring gameplay.
/// </summary>
public class RingController : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private Image zoneImage; // Type=Filled, Fill Method=Radial 360, Clockwise
    [SerializeField] private RectTransform zoneRect;
    [SerializeField] private RectTransform capStart;
    [SerializeField] private RectTransform capEnd;
    [SerializeField] private float ringRadius = 116f;

    [Header("Width bounds (degrees)")]
    [SerializeField] private float maxWidth = 70f;
    [SerializeField] private float minWidth = 18f;
    [SerializeField] private float widthShrinkPerHit = 6f;

    [Header("Dynamic mode tuning")]
    [SerializeField] private float revolveSpeedMin = 20f; // deg/sec
    [SerializeField] private float revolveSpeedMax = 50f;
    [SerializeField] private float colorFlipIntervalMin = 1.2f;
    [SerializeField] private float colorFlipIntervalMax = 3f;

    private static readonly Color GreenColor = new Color(0.11f, 0.62f, 0.46f);
    private static readonly Color RedColor = new Color(0.85f, 0.24f, 0.18f);

    public bool IsActive { get; private set; }
    public RingMode Mode { get; private set; } = RingMode.Static;
    public RingColorState Color { get; private set; } = RingColorState.Green;
    public float CurrentWidth { get; private set; }
    public float ZoneCenter { get; private set; }
    public bool AtMinWidth => CurrentWidth <= minWidth + 0.01f;

    private float revolveSpeed;
    private float colorFlipTimer;

    public void Activate()
    {
        IsActive = true;
        gameObject.SetActive(true);
        Mode = RingMode.Static;
        Color = RingColorState.Green;
        CurrentWidth = maxWidth;
        SetZone(Random.Range(0f, 360f), CurrentWidth);
        ApplyColor();
    }

    public void Deactivate()
    {
        IsActive = false;
        gameObject.SetActive(false);
    }

    public void SwitchToDynamic()
    {
        Mode = RingMode.Dynamic;
        revolveSpeed = Random.Range(revolveSpeedMin, revolveSpeedMax) * (Random.value < 0.5f ? -1f : 1f);
        colorFlipTimer = Random.Range(colorFlipIntervalMin, colorFlipIntervalMax);
    }

    public void RegisterHit()
    {
        CurrentWidth = Mathf.Max(minWidth, CurrentWidth - widthShrinkPerHit);

        if (Mode == RingMode.Static)
            SetZone(Random.Range(0f, 360f), CurrentWidth); // re-randomize center each hit, phase 1 only
        else
            SetZone(ZoneCenter, CurrentWidth); // keep current center, revolve handles movement
    }

    public void ReshuffleRandom(bool forceRed)
    {
        CurrentWidth = Random.Range(minWidth, maxWidth);
        SetZone(Random.Range(0f, 360f), CurrentWidth);
        Color = forceRed ? RingColorState.Red : RingColorState.Green;
        ApplyColor();
        colorFlipTimer = Random.Range(colorFlipIntervalMin, colorFlipIntervalMax);
    }

    public bool IsAngleInZone(float angle, float forgivenessDegrees)
    {
        float diff = Mathf.Abs(Mathf.DeltaAngle(angle, ZoneCenter));
        return diff <= CurrentWidth / 2f + forgivenessDegrees;
    }

    private void Update()
    {
        if (!IsActive || Mode != RingMode.Dynamic) return;

        ZoneCenter = (ZoneCenter + revolveSpeed * Time.deltaTime + 360f) % 360f;
        SetZone(ZoneCenter, CurrentWidth);

        colorFlipTimer -= Time.deltaTime;
        if (colorFlipTimer <= 0f)
        {
            Color = Color == RingColorState.Green ? RingColorState.Red : RingColorState.Green;
            ApplyColor();
            colorFlipTimer = Random.Range(colorFlipIntervalMin, colorFlipIntervalMax);
        }
    }

    private void ApplyColor()
    {
        Color32 c = Color == RingColorState.Green ? (Color32)GreenColor : (Color32)RedColor;
        zoneImage.color = c;
        if (capStart) capStart.GetComponent<Image>().color = c;
        if (capEnd) capEnd.GetComponent<Image>().color = c;
    }

    // private void SetZone(float centerDegrees, float widthDegrees)
    // {
    //     ZoneCenter = centerDegrees;
    //     zoneImage.fillAmount = widthDegrees / 360f;

    //     float startAngle = centerDegrees - widthDegrees / 2f;
    //     zoneRect.localRotation = Quaternion.Euler(0f, 0f, -startAngle);

    //     PositionCaps(startAngle, startAngle + widthDegrees);
    // }

    // private void PositionCaps(float startAngle, float endAngle)
    // {
    //     if (capStart == null || capEnd == null) return;
    //     capStart.anchoredPosition = AngleToPoint(startAngle);
    //     capEnd.anchoredPosition = AngleToPoint(endAngle);

    // }
    private void SetZone(float centerDegrees, float widthDegrees)
    {
        ZoneCenter = centerDegrees;
        zoneImage.fillAmount = widthDegrees / 360f;

        float startAngle = centerDegrees - widthDegrees / 2f;
        zoneRect.localRotation = Quaternion.Euler(0f, 0f, -startAngle);

        // Pass the absolute angles to PositionCaps
        PositionCaps(startAngle, startAngle + widthDegrees);
    }

    private void PositionCaps(float startAngle, float endAngle)
    {
        if (capStart == null || capEnd == null) return;

        // Because the parent (Ring0) is already rotated by -startAngle,
        // we subtract startAngle from the target positions to compensate.
        float localStartAngle = startAngle - startAngle; // This will always be 0
        float localEndAngle = endAngle - startAngle;     // This will always be widthDegrees

        capStart.anchoredPosition = AngleToPoint(localStartAngle);
        capEnd.anchoredPosition = AngleToPoint(localEndAngle);

        // Optional: Rotate the cap graphics themselves so they face along the arc
        capStart.localRotation = Quaternion.Euler(0f, 0f, -localStartAngle);
        capEnd.localRotation = Quaternion.Euler(0f, 0f, -localEndAngle);
    }

    private Vector2 AngleToPoint(float angleDegrees)
    {
        float rad = angleDegrees * Mathf.Deg2Rad;
        return new Vector2(Mathf.Sin(rad), Mathf.Cos(rad)) * ringRadius;
    }
}