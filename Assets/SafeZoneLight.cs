using Unity.VisualScripting;
using UnityEngine;

public class SafeZoneLight : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<MonsterScript>()?.ApplyStun(0f);
    }
}
