using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class CameramanAgentScript : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Camera mainCamera;
    [SerializeField] float maxDistance = 3.5f;
    [SerializeField] float minDistance = 1f;
    [SerializeField] NavMeshAgent navMeshAgent;
    [SerializeField] Animator animator;
    [SerializeField] Transform handCamera;
    [SerializeField] Transform spotlight;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 0, 0);
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private AudioSource stepAudioSource;
    private bool isMoving = false;
    private int direction = 0;

    private void Awake()
    {
        navMeshAgent.updateRotation = false;
    }

    void FixedUpdate()
    {
        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > maxDistance)
        {
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            Vector3 destination = target.position - directionToTarget * minDistance;
            navMeshAgent.SetDestination(destination);
            isMoving = true;
            direction = 1;
        }
        else if (distance <= minDistance)
        {
            Vector3 directionFromTarget = (transform.position - target.position).normalized;
            Vector3 retreatPosition = target.position + directionFromTarget * minDistance;
            navMeshAgent.SetDestination(retreatPosition);
            isMoving = true;
            direction = -1;
        }
        else
        {
            navMeshAgent.ResetPath();
            isMoving = false;
            direction = 0;
        }

        Vector3 lookDirection = (target.position - transform.position);
        lookDirection.y = 0;
        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        spotlight.position = new Vector3 (handCamera.position.x + cameraOffset.x, spotlight.position.y + cameraOffset.y, handCamera.position.z + cameraOffset.z);
        animator.SetBool("isMoving", isMoving);
        animator.SetInteger("movementDirection", direction);
    }
    private void Step()
    {
        stepAudioSource.Play();
    }
}
