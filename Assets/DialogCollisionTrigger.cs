using UnityEngine;

public class DialogCollisionTrigger : DialogTrigger
{
    [SerializeField] private Collider colliderTrigger;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            TriggerDialog();
        }
    }
}
