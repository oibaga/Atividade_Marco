using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    public Dialog[] dialogs;
    public bool oneTrigger = false;
    public bool canMove = true;
    public bool hasBlackPanel = false;
    private bool canTrigger = true;

    public virtual void TriggerDialog()
    {
        if (canTrigger)
        {
            FindFirstObjectByType<DialogManager>().StartConversation(this);

            canTrigger = !oneTrigger;
        }
    }

    public virtual void SetCurrentIndex(int index) {}

    public virtual void EndedDialog() {}
}
