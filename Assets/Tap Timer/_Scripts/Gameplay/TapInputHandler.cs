using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TapInputHandler : MonoBehaviour
{
    [Header("UI Layers that block gameplay taps")]
    [SerializeField] private LayerMask blockingLayers;

    private InputAction tapAction;

    public event System.Action OnTap;

    // Reuse these to avoid GC allocations
    private readonly List<RaycastResult> raycastResults = new();
    private PointerEventData pointerEventData;

    private int lastTapFrame = -1;

    private void Awake()
    {
        tapAction = new InputAction("Tap", binding: "<Pointer>/press");
        tapAction.performed += OnTapPerformed;

        if (EventSystem.current != null)
            pointerEventData = new PointerEventData(EventSystem.current);
    }

    private void OnEnable() => tapAction.Enable();

    private void OnDisable() => tapAction.Disable();

    private void OnTapPerformed(InputAction.CallbackContext ctx)
    {
        // <Pointer>/press matches every Pointer-type device (Mouse, Pen, Touchscreen).
        // A single physical tap can be reported by more than one of these at once,
        // firing this callback twice in the same frame. Only the first counts.
        if (Time.frameCount == lastTapFrame)
            return;
        lastTapFrame = Time.frameCount;

        if (ShouldBlockTap())
            return;

        OnTap?.Invoke();
    }

    private bool ShouldBlockTap()
    {
        if (EventSystem.current == null)
            return false;

        if (pointerEventData == null)
            pointerEventData = new PointerEventData(EventSystem.current);

        pointerEventData.position = Pointer.current.position.ReadValue();

        raycastResults.Clear();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        foreach (RaycastResult hit in raycastResults)
        {
            if (((1 << hit.gameObject.layer) & blockingLayers.value) != 0)
            {
                return true;
            }
        }

        return false;
    }
}