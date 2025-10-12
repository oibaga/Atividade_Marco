using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyFSM_PatrolPoints : MonoBehaviour
{
    private enum State { Patrol, Roar, Chase, Attack }
    private State currentState = State.Patrol;

    [Header("Referências")]
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform[] patrolPoints;

    private NavMeshAgent agent;
    private int currentPatrolIndex = -1;
    private float currentWaitTime = 0f;
    private float maxWaitTime = 0f;

    [Header("Configurações de Movimento")]
    [SerializeField] private float detectionRadius = 8f;
    [SerializeField] private float loseDistance = 12f;
    [SerializeField] private float giveUpTime = 3f;
    [SerializeField] private float patrolSpeed = 2.5f;
    [SerializeField] private float chaseSpeed = 4f;

    private Coroutine giveUpCoroutine;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = true;
        agent.updateUpAxis = true;

        GoToNextPatrolPoint();
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
            case State.Attack:
                break;
        }
    }
    private void Patrol()
    {
        animator.SetBool("isRunning", false);
        agent.speed = patrolSpeed;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRadius)
        {
            ChangeState(State.Roar);
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= 0.3f)
        {
            if (maxWaitTime == 0)
                maxWaitTime = Random.Range(3, 5);

            currentWaitTime += Time.deltaTime;

            if (currentWaitTime >= maxWaitTime)
            {
                currentWaitTime = 0;
                maxWaitTime = 0;
                animator.SetBool("isWalking", true);
                GoToNextPatrolPoint();
            }
            else
            {
                animator.SetBool("isWalking", false);
            }
        }
    }

    private void Roar()
    {
        animator.SetTrigger("Roar");
        agent.ResetPath();
        agent.speed = 0f;
        StartCoroutine( RoarCoroutine() );
    }
    private void Attack() 
    {
        animator.SetTrigger("Attack");
        agent.ResetPath();
        agent.speed = 0f;
        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator RoarCoroutine()
    {
        yield return new WaitForSeconds(5.1f); // tempo da animação de rugido
        animator.SetTrigger("StartRun");
        ChangeState(State.Chase);
    }
    private IEnumerator AttackCoroutine()
    {
        yield return new WaitForSeconds(4.04f);
        ChangeState(State.Patrol);
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

    private void GoToNextPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return;

        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
    }

    private void ChangeState(State newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;

        switch (newState)
        {
            case State.Patrol:
                GoToNextPatrolPoint();
                break;
            case State.Roar:
                Roar();
                break;
            case State.Chase:
                break;
            case State.Attack:
                Attack();
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, loseDistance);
    }
}
