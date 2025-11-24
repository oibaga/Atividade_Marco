using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;
using UnityEngine.UIElements;

public class CameramanAgentScript : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    [SerializeField] float longestDistance = 3.5f;
    [SerializeField] float closestDistance = 1f;
    [SerializeField] float runSpeed = 5f;

    [Header("Lanterna")]
    [SerializeField] private float stunRayDistance = 5f;
    [SerializeField] private float angle = 25f;        // abertura do cone
    [SerializeField] private LayerMask enemyMask;      // camada do monstro

    [Header("Referências")]
    [SerializeField] Transform target;
    [SerializeField] Camera mainCamera;
    [SerializeField] NavMeshAgent navMeshAgent;
    [SerializeField] Animator animator;
    [SerializeField] Transform handCamera;
    [SerializeField] Transform spotlight;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private AudioSource stepAudioSource;
    public bool isGrabbed = false;

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
        if (isGrabbed)
        {
            navMeshAgent.ResetPath();
            animator.SetBool("isMoving", false);
            animator.SetBool("isRunning", false);
            direction = 0;
            return;   // Cameraman não faz nada enquanto está preso
        }

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
            Vector3 toTarget = (target.position - transform.position).normalized;

            Vector3 rightDir = Vector3.Cross(Vector3.up, toTarget).normalized;
            Vector3 leftDir = -rightDir;

            bool rightFree = !Physics.Raycast(transform.position, rightDir, 1.5f, groundLayer);
            bool leftFree = !Physics.Raycast(transform.position, leftDir, 1.5f, groundLayer);

            Vector3 sideStepDir = rightFree ? rightDir : (leftFree ? leftDir : -toTarget);
            Vector3 sideStepPos = transform.position + sideStepDir * 1.5f;

            navMeshAgent.SetDestination(sideStepPos);
            isMoving = true;
            direction = -1;
        }
        else
        {
            navMeshAgent.ResetPath();
            isMoving = false;
            direction = 0;
        }

        Ray ray = mainCamera.ScreenPointToRay(
        new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCamera.nearClipPlane)
);

        if (Physics.Raycast(ray, out RaycastHit hit, 25f, groundLayer))
        {
            Vector3 lookDirection = hit.point - transform.position;
            lookDirection.y = 0f;

            if (lookDirection.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(lookDirection);
            }
        }


        CheckLightRaycasts();

        spotlight.position = new Vector3(handCamera.position.x, spotlight.position.y, handCamera.position.z);
        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isRunning", inChase);
        animator.SetInteger("movementDirection", direction);
    }

    private void CheckLightRaycasts()
    {
        Vector3 forward = transform.forward;
        Vector3 left = Quaternion.AngleAxis(-angle, Vector3.up) * forward;
        Vector3 right = Quaternion.AngleAxis(angle, Vector3.up) * forward;

        Vector3 origin = transform.position;
        origin.y = 5f;

        // RAYCAST CENTRAL
        if (Physics.Raycast(transform.position, forward, out RaycastHit hitCenter, stunRayDistance, enemyMask))
        {
            hitCenter.collider.GetComponent<MonsterScript>()?.ApplyStun(3f);
        }

        // RAYCAST ESQUERDA
        if (Physics.Raycast(transform.position, left, out RaycastHit hitLeft, stunRayDistance, enemyMask))
        {
            hitLeft.collider.GetComponent<MonsterScript>()?.ApplyStun(3f);
        }

        // RAYCAST DIREITA
        if (Physics.Raycast(transform.position, right, out RaycastHit hitRight, stunRayDistance, enemyMask))
        {
            hitRight.collider.GetComponent<MonsterScript>()?.ApplyStun(3f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        Vector3 origin = transform.position;
        origin.y = 5f;
        Vector3 forward = transform.forward;

        // Ray central
        Vector3 centralDir = forward;
        Gizmos.DrawLine(origin, origin + centralDir * stunRayDistance);

        // Ray da esquerda
        Vector3 leftDir = Quaternion.Euler(0, -angle, 0) * forward;
        Gizmos.DrawLine(origin, origin + leftDir * stunRayDistance);

        // Ray da direita
        Vector3 rightDir = Quaternion.Euler(0, angle, 0) * forward;
        Gizmos.DrawLine(origin, origin + rightDir * stunRayDistance);
    }

    private void Step()
    {
        stepAudioSource.Play();
    }

    public void StartChase()
    {
        inChase = true;
        longestDistance = 1f;
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

    public void Grab()
    {
        isGrabbed = true;

        navMeshAgent.ResetPath();
        navMeshAgent.isStopped = true;

        animator.SetBool("isMoving", false);
        animator.SetBool("isRunning", false);
    }

    public void Release()
    {
        isGrabbed = false;
        navMeshAgent.isStopped = false;
    }

}
