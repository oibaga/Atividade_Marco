using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CameramanAgentScript : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float maxDistance = 2f;
    [SerializeField] NavMeshAgent navMeshAgent;
    [SerializeField] Animator animator;
    private bool isMoving = false;

    void FixedUpdate()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance > maxDistance)
        {
            navMeshAgent.SetDestination(target.position);
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
        animator.SetBool("isMoving", isMoving);
    }
}