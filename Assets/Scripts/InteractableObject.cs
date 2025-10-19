using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [SerializeField] private int objectIndex;
    [SerializeField] private GameObject inspectionableObject;
    [SerializeField] private bool canPickable = false;
    [SerializeField] private DialogTrigger initialDialogTrigger;
    [SerializeField] private DialogTrigger endDialogTrigger;
    [SerializeField] private PlayerMoviment player;
    [SerializeField] private TextMeshProUGUI interactionText;
    private Material outline;
    private float outlineThickness = 0.03f;

    private void Awake()
    {
        outline = this.gameObject.GetComponent<MeshRenderer>().materials[^1];
        outlineThickness = outline.GetFloat("_OutlineThickness");
    }

    public void Inspect()
    {
        FindFirstObjectByType<Object_Interact>().StartInspection(inspectionableObject);
        initialDialogTrigger?.TriggerDialog();
    }

    public void PickUp(Queue<int> inventoryIndexes)
    {
        if (!canPickable) return;

        inventoryIndexes.Enqueue(objectIndex);

        endDialogTrigger?.TriggerDialog();

        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        InteractableObject playerClosestObj = player.closestItem;

        if (playerClosestObj == this)
        {
            outline.SetFloat("_OutlineThickness", outlineThickness);
            interactionText.enabled = true;
        }
        else
        {
            outline.SetFloat("_OutlineThickness", 0f);
            interactionText.enabled = false;
        }
    }
}
