using Unity.VisualScripting;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Transform playerTransform;

    private void Update()
    {
        Vector3 posTarget = new Vector3(target.position.x, transform.position.y, target.position.z);
        Vector3 posPlayer = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);

        transform.position = (posTarget + posPlayer) / 2;
    }
}