using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private Image imageSprite;
    private Queue<Dialog> dialogs;

    private void Start()
    {
        dialogs = new Queue<Dialog>();
    }

    public void StartConversation(Dialog[] dialogs, Boolean playerCanMove)
    {
        this.dialogs.Clear();

        foreach (Dialog dialog in dialogs)
        {
            this.dialogs.Enqueue(dialog);
        }

        ShowNextSentence();
        imageSprite.enabled = true;
    }

    private void ShowNextSentence()
    {
        if (dialogs.Count == 0)
        {
            EndDialogue();
            return;
        }

        Dialog dialog = dialogs.Dequeue();
        nameText.text = dialog.name;
        dialogText.text = dialog.sentence;
        imageSprite.sprite = dialog.image;
    }

    private void EndDialogue()
    {
        imageSprite.enabled = false;
    }
}
