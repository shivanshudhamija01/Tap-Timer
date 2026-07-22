
using UnityEngine;
using UnityEngine.UI;

public enum RingColorState { Green, Red }

public enum RingMode { Static, Dynamic }


public enum PulseState { Idle, Cooldown, Growing, Shrinking }

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

    [Header("Chaos pulse tuning")]
    [SerializeField] private float pulseGrowSpeed = 25f;   // deg/sec
    [SerializeField] private float pulseShrinkSpeed = 20f; // deg/sec
    [SerializeField] private float pulseCooldownMin = 0.5f;
    [SerializeField] private float pulseCooldownMax = 2f;

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

    private bool pulseEnabled;
    private PulseState pulseState = PulseState.Idle;
    private float pulseCooldownTimer;
    private float pulseTargetWidth;

    private bool isPaused;

    public void Activate(float centerDegrees)
    {
        IsActive = true;
        isPaused = false;
        gameObject.SetActive(true);
        Mode = RingMode.Static;
        Color = RingColorState.Green;
        CurrentWidth = maxWidth;
        pulseEnabled = false;
        pulseState = PulseState.Idle;
        SetZone(centerDegrees, CurrentWidth);
        ApplyColor();
    }

    public void SetPaused(bool paused)
    {
        isPaused = paused;
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

    public void EnablePulse()
    {
        pulseEnabled = true;
        pulseState = PulseState.Idle; // will move to Cooldown next frame since already at min width
    }

    public void RegisterHit()
    {
        CurrentWidth = Mathf.Max(minWidth, CurrentWidth - widthShrinkPerHit);

        if (Mode == RingMode.Static)
            SetZone(Random.Range(0f, 360f), CurrentWidth); // re-randomize center each hit, phase 1 only
        else
            SetZone(ZoneCenter, CurrentWidth); // keep current center, revolve handles movement

        if (pulseEnabled && AtMinWidth)
            pulseState = PulseState.Idle; // let the pulse cycle restart cleanly
    }

    public bool IsAngleInZone(float angle, float forgivenessDegrees)
    {
        float diff = Mathf.Abs(Mathf.DeltaAngle(angle, ZoneCenter));
        return diff <= CurrentWidth / 2f + forgivenessDegrees;
    }

    private void Update()
    {
        if (!IsActive || isPaused) return;

        bool dirty = false;

        if (Mode == RingMode.Dynamic)
        {
            ZoneCenter = (ZoneCenter + revolveSpeed * Time.deltaTime + 360f) % 360f;
            dirty = true;

            colorFlipTimer -= Time.deltaTime;
            if (colorFlipTimer <= 0f)
            {
                Color = Color == RingColorState.Green ? RingColorState.Red : RingColorState.Green;
                ApplyColor();
                colorFlipTimer = Random.Range(colorFlipIntervalMin, colorFlipIntervalMax);
            }
        }

        if (pulseEnabled)
        {
            dirty |= UpdatePulse();
        }

        if (dirty)
        {
            SetZone(ZoneCenter, CurrentWidth);
        }
    }

    private bool UpdatePulse()
    {
        switch (pulseState)
        {
            case PulseState.Idle:
                if (AtMinWidth)
                {
                    pulseState = PulseState.Cooldown;
                    pulseCooldownTimer = Random.Range(pulseCooldownMin, pulseCooldownMax);
                }
                return false;

            case PulseState.Cooldown:
                pulseCooldownTimer -= Time.deltaTime;
                if (pulseCooldownTimer <= 0f)
                {
                    pulseTargetWidth = Random.Range(minWidth + 20f, maxWidth);
                    pulseState = PulseState.Growing;
                }
                return false;

            case PulseState.Growing:
                CurrentWidth = Mathf.Min(pulseTargetWidth, CurrentWidth + pulseGrowSpeed * Time.deltaTime);
                if (CurrentWidth >= pulseTargetWidth - 0.01f) pulseState = PulseState.Shrinking;
                return true;

            case PulseState.Shrinking:
                CurrentWidth = Mathf.Max(minWidth, CurrentWidth - pulseShrinkSpeed * Time.deltaTime);
                if (AtMinWidth) pulseState = PulseState.Idle;
                return true;
        }
        return false;
    }

    private void ApplyColor()
    {
        Color32 c = Color == RingColorState.Green ? (Color32)GreenColor : (Color32)RedColor;
        zoneImage.color = c;
        if (capStart) capStart.GetComponent<Image>().color = c;
        if (capEnd) capEnd.GetComponent<Image>().color = c;
    }


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
