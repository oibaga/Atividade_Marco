using UnityEngine;

public class IntegerConditionalCollisionDialogTrigger : DialogCollisionTrigger
{
    [SerializeField] private int conditionNumber = 0;
    [SerializeField] private bool contains = true;
    public override void TriggerDialog()
    {
        if (contains)
        {
            if (FindFirstObjectByType<PlayerMoviment>().inventoryIndexes.Contains(conditionNumber))
            {
                base.TriggerDialog();
            }
        }
        else
        {
            if (!FindFirstObjectByType<PlayerMoviment>().inventoryIndexes.Contains(conditionNumber))
            {
                base.TriggerDialog();
            }
        }
    }
}