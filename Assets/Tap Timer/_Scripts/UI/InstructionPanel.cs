using UnityEngine;
using UnityEngine.EventSystems;

public class InstructionPanel : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        // GameEvents.OnGameStart();

        EventBus<StartGameRequestedEvent>.Raise(new StartGameRequestedEvent());
    }
}
