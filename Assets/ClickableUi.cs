using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class ClickableUi : EventTrigger
{
    [SerializeField] private MainMenuEventSystem mainMenuEventSystem;
    private bool isSelected;

    private void Update()
    {
        if (mainMenuEventSystem != null)
        {
            if ((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space)) && isSelected)
            {
                GetComponent<Button>().onClick.Invoke();
            }
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        mainMenuEventSystem.Clickable();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        mainMenuEventSystem.Default();
    }

    public override void OnSelect(BaseEventData eventData)
    {
        isSelected = true;
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        isSelected = false;
    }
}
