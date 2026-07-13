using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Single-button input for Tap Timer. Uses a raw InputAction field directly
/// (no generated wrapper class) — same pattern adopted for Rush Rider after
/// the BicycleInputActions naming collision.
/// </summary>
public class TapInputHandler : MonoBehaviour
{
    private InputAction tapAction;

    public event System.Action OnTap;

    private void Awake()
    {
        tapAction = new InputAction("Tap", binding: "<Pointer>/press");
        tapAction.performed += ctx => OnTap?.Invoke();
    }

    private void OnEnable() => tapAction.Enable();
    private void OnDisable() => tapAction.Disable();
}