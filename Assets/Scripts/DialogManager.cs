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

    private void Awake()
    {
        dialogs = new Queue<Dialog>();
    }

    public void StartConversation(Dialog[] dialogs, Boolean playerCanMove = true, Boolean hasBlackPanel = false, Boolean canSkip = false)
    {
        this.dialogs.Clear();

        foreach (Dialog dialog in dialogs)
        {
            this.dialogs.Enqueue(dialog);
        }

        ShowNextSentence();

        skipText.gameObject.SetActive(canSkip);
        dialogPanel.GetComponent<Image>().enabled = hasBlackPanel;

        if (!playerCanMove)
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

        textAudioSource.clip = dialog.textAudios[ UnityEngine.Random.Range(0, dialog.textAudios.Length) ];
        customAudioSource.clip = dialog.customAudio;
        textAudioSource.Play();
        if (customAudioSource.clip) customAudioSource.Play();
    }

    private void EndDialogue()
    {
        dialogPanel.gameObject.SetActive(false);
        textAudioSource.Stop();
        customAudioSource.Stop();

        this.dialogs.Clear();

        FindFirstObjectByType<PlayerMoviment>().UnlockMoviment();
    }
}
