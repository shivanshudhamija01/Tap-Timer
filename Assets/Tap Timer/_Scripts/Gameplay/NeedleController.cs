using UnityEngine;

/// <summary>
/// Rotates a UI needle (RectTransform) around the dial center at a given speed.
/// Angle convention: 0 = up (12 o'clock), increases clockwise, wraps 0-360.
/// </summary>
public class NeedleController : MonoBehaviour
{
    [SerializeField] private RectTransform needleRect;

    public float CurrentAngle { get; private set; }
    public float Speed { get; set; } // degrees per second

    private bool isRunning;

    public void SetRunning(bool running) => isRunning = running;

    public void ResetAngle(float angle = 0f)
    {
        CurrentAngle = angle;
        ApplyRotation();
    }

    private void Update()
    {
        if (!isRunning) return;

        CurrentAngle = (CurrentAngle + Speed * Time.deltaTime) % 360f;
        ApplyRotation();
    }

    private void ApplyRotation()
    {
        // Unity's Z rotation is counter-clockwise positive, so negate for clockwise sweep.
        needleRect.localRotation = Quaternion.Euler(0f, 0f, -CurrentAngle);
    }
}