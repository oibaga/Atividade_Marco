using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    public Dialog[] dialogs;

    public void TriggerDialog()
    {
        FindFirstObjectByType< DialogManager >().StartConversation( dialogs, true );
    }
}
