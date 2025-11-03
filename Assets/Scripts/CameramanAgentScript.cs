using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class CameramanAgentScript : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    [SerializeField] float longestDistance = 3.5f;
    [SerializeField] float closestDistance = 1f;
    [SerializeField] float runSpeed = 5f;

    [Header("Referências")]
    [SerializeField] Transform target;
    [SerializeField] Camera mainCamera;
    [SerializeField] NavMeshAgent navMeshAgent;
    [SerializeField] Animator animator;
    [SerializeField] Transform handCamera;
    [SerializeField] Transform spotlight;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private AudioSource stepAudioSource;

    private bool isMoving = false;
    private int direction = 0;
    private float defaultLongestDistance = 3.5f;
    private float defaultClosestDistance = 1f;
    private float defaultNavMeshSpeed = 3f;
    private bool inChase = false;

    private void Awake()
    {
        navMeshAgent.updateRotation = false;

        defaultLongestDistance = longestDistance;
        defaultClosestDistance = closestDistance;
        defaultNavMeshSpeed = navMeshAgent.speed;
    }

    void FixedUpdate()
    {
        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > longestDistance)
        {
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            Vector3 destination = target.position - directionToTarget * closestDistance;
            navMeshAgent.SetDestination(destination);
            isMoving = true;
            direction = 1;
        }
        else if (distance <= closestDistance)
        {
            Vector3 directionFromTarget = (transform.position - target.position).normalized;
            Vector3 retreatPosition = target.position + directionFromTarget * closestDistance;
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

        spotlight.position = new Vector3(handCamera.position.x, spotlight.position.y, handCamera.position.z);
        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isRunning", inChase);
        animator.SetInteger("movementDirection", direction);
    }

    private void Step()
    {
        stepAudioSource.Play();
    }

    public void StartChase()
    {
        inChase = true;
        longestDistance = 2f;
        closestDistance = 1f;
        navMeshAgent.speed = runSpeed;
    }

    public void StopChase()
    {
        inChase = false;
        longestDistance = defaultLongestDistance;
        closestDistance = defaultClosestDistance;
        navMeshAgent.speed = defaultNavMeshSpeed;
    }
}
