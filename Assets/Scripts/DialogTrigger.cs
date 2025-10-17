using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    public Dialog[] dialogs;
    [SerializeField] private bool oneTrigger = false;
    [SerializeField] private bool canMove = true;
    [SerializeField] private bool hasBlackPanel = false;
    private bool canTrigger = true;

    public void TriggerDialog()
    {
        if (canTrigger)
        {
            FindFirstObjectByType<DialogManager>().StartConversation(dialogs, canMove, hasBlackPanel);

            canTrigger = !oneTrigger;
        }
    }
}
