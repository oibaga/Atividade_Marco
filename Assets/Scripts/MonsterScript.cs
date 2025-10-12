using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MonsterScript : MonoBehaviour
{
    private enum State { Patrol, Roar, Chase }
    private State currentState = State.Patrol;

    [Header("Referencias")]
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;
    private NavMeshAgent agent;

    [Header("Configurações de Movimento")]
    [SerializeField] private float patrolRadius = 10f;
    [SerializeField] private float patrolInterval = 4f;
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float detectionRadius = 8f;
    [SerializeField] private float loseDistance = 12f;
    [SerializeField] private float giveUpTime = 3f;

    private float patrolTimer;
    private Coroutine giveUpCoroutine;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        patrolTimer = patrolInterval;
        SetRandomDestination();
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Roar:
                break;
            case State.Chase:
                Chase();
                break;
        }
    }

    private void Patrol()
    {
        animator.SetBool("isWalking", true);
        animator.SetBool("isRunning", false);
        agent.speed = patrolSpeed;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRadius)
        {
            ChangeState(State.Roar);
            return;
        }
        patrolTimer -= Time.deltaTime;
        if (!agent.pathPending && agent.remainingDistance <= 0.3f || patrolTimer <= 0f)
        {
            SetRandomDestination();
            patrolTimer = patrolInterval;
        }
    }

    private void Roar()
    {
        animator.SetTrigger("Roar");
        agent.ResetPath();
        agent.speed = 0f;

        StartCoroutine(RoarCoroutine());
    }

    private IEnumerator RoarCoroutine()
    {
        yield return new WaitForSeconds(1.5f); 
        ChangeState(State.Chase);
    }

    private void Chase()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", true);
        agent.speed = chaseSpeed;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        agent.SetDestination(player.position);

        if (distanceToPlayer > loseDistance)
        {
            if (giveUpCoroutine == null)
                giveUpCoroutine = StartCoroutine(GiveUpTimer());
        }
        else
        {
            if (giveUpCoroutine != null)
            {
                StopCoroutine(giveUpCoroutine);
                giveUpCoroutine = null;
            }
        }
    }

    private IEnumerator GiveUpTimer()
    {
        yield return new WaitForSeconds(giveUpTime);

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > loseDistance)
        {
            ChangeState(State.Patrol);
        }

        giveUpCoroutine = null;
    }

    private void SetRandomDestination()
    {
        Vector3 randomDir = Random.insideUnitSphere * patrolRadius + transform.position;
        if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    private void ChangeState(State newState)
    {
        if (currentState == newState) return;

        currentState = newState;

        switch (newState)
        {
            case State.Patrol:
                SetRandomDestination();
                patrolTimer = patrolInterval;
                break;
            case State.Roar:
                Roar();
                break;
            case State.Chase:
                break;
        }
    }
}
