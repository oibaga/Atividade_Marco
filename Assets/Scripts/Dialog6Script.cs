using UnityEngine;

public class Dialog6Script : DialogTrigger
{
    [SerializeField] private GameObject monster;
    [SerializeField] private GameObject fire;
    [SerializeField] private GameObject safeLight;
    public override void EndedDialog()
    {
        monster.SetActive(true);
        fire.SetActive(false);
        safeLight.SetActive(true);
    }
}
