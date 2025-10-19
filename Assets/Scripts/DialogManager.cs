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
    [SerializeField] private TextMeshProUGUI skipText;
    [SerializeField] private Image imageSprite;
    [SerializeField] private Image dialogPanel;
    [SerializeField] private AudioSource textAudioSource;
    [SerializeField] private AudioSource customAudioSource;
    private Queue<Dialog> dialogs;
    private DialogTrigger currentDialogTrigger;

    private void Awake()
    {
        dialogs = new Queue<Dialog>();
    }

    public void StartConversation(DialogTrigger dialogTrigger)
    {
        if (dialogTrigger == currentDialogTrigger) return;
        this.dialogs.Clear();

        currentDialogTrigger = dialogTrigger;
        for (int i = 0; i < currentDialogTrigger.dialogs.Length; i++)
        {
            currentDialogTrigger.dialogs[i].index = i;
            this.dialogs.Enqueue( currentDialogTrigger.dialogs[i] );
        }

        ShowNextSentence();

        dialogPanel.GetComponent<Image>().enabled = dialogTrigger.hasBlackPanel;

        if (!dialogTrigger.canMove)
        {
            FindFirstObjectByType<PlayerMoviment>().BlockMoviment();
        }

        dialogPanel.gameObject.SetActive(true);
    }

    public void ShowNextSentence()
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

        if (dialog.textAudios.Length > 0)
        {
            textAudioSource.clip = dialog.textAudios[UnityEngine.Random.Range(0, dialog.textAudios.Length)];
        }
        else
        {
            textAudioSource.clip = null;
        }
        if (dialog.customAudio)
        {
            customAudioSource.clip = dialog.customAudio;
        }
        else
        {
            customAudioSource.clip = null;
        }
        if (textAudioSource.clip) textAudioSource.Play();

        if (customAudioSource.clip) customAudioSource.Play();

        currentDialogTrigger.SetCurrentIndex( dialog.index );
    }

    private void EndDialogue()
    {
        dialogPanel.gameObject.SetActive(false);
        textAudioSource.Stop();
        customAudioSource.Stop();

        this.dialogs.Clear();
        currentDialogTrigger?.EndedDialog();
        currentDialogTrigger = null;

        if ( !FindFirstObjectByType<PlayerMoviment>().isInspecting ) FindFirstObjectByType<PlayerMoviment>().UnlockMoviment();
    }
}
