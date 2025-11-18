using Unity.VisualScripting;
using UnityEngine;

public class SafeZoneLight : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        MonsterScript monster = other.GetComponent<MonsterScript>();
        if (monster != null)
        {
            monster.ApplyStun(0f);
        }
    }
}
