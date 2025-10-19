using UnityEngine.EventSystems;
using UnityEngine;

public class ClickableUi : EventTrigger
{
    [SerializeField] private MainMenuEventSystem mainMenuEventSystem;

    public override void OnPointerEnter(PointerEventData eventData)
    {
        mainMenuEventSystem.Clickable();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        mainMenuEventSystem.Default();
    }
}
