using UnityEngine;
using UnityEngine.AI;

public class MonsterScript : MonoBehaviour
{
    private enum State { Patrol, Engage }
    private State currentState = State.Patrol;

    [Header("Referências")]
    [SerializeField] private Transform target;
    [SerializeField] private Animator animator;
    private NavMeshAgent agent;

    [Header("Configurações de Movimento")]
    [SerializeField] private float chaseRadius = 6f;
    [SerializeField] private float patrolRadius = 8f;
    [SerializeField] private float patrolInterval = 3f;

    [Header("Zona Segura")]
    [SerializeField] private LayerMask safeZoneLayer; // <- defina no Inspector

    private float patrolTimer;

    void Update()
    {
        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                animator.SetBool("isMoving", true);
                animator.SetBool("isRunning", false);
                agent.speed = 2.8f;
                break;
            case State.Engage:
                Engage();
                animator.SetBool("isMoving", true);
                animator.SetBool("isRunning", true);
                agent.speed = 3.1f;
                break;
        }
    }

    private void Patrol()
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget <= chaseRadius && !IsPlayerInSafeZone())
        {
            currentState = State.Engage;
            return;
        }

        patrolTimer -= Time.deltaTime;
        if (!agent.pathPending && agent.remainingDistance <= 0.2f || patrolTimer <= 0f)
        {
            SetRandomPatrolDestination();
            patrolTimer = patrolInterval;
        }
    }

    private void Engage()
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        agent.SetDestination(target.position);

        if (distanceToTarget > chaseRadius || IsPlayerInSafeZone())
        {
            currentState = State.Patrol;
            SetRandomPatrolDestination();
            patrolTimer = patrolInterval;
        }
    }

    private void SetRandomPatrolDestination()
    {
        Vector3 center = transform.position;
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float r = Random.Range(0f, patrolRadius);
        Vector3 candidate = new Vector3(center.x + Mathf.Cos(angle) * r, center.y + Mathf.Sin(angle) * r, 0f);

        if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 1.5f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    private bool IsPlayerInSafeZone()
    {
        Vector2 playerPos2D = target.position;
        Collider2D hit = Physics2D.OverlapPoint(playerPos2D, safeZoneLayer);
        return hit != null;
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        //agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Start()
    {
        patrolTimer = patrolInterval;
        SetRandomPatrolDestination();
    }
}