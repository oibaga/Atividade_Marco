using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.SceneManagement;

public class EnemyFSM_PatrolPoints : MonoBehaviour
{
    private enum State { Patrol, Roar, Chase, Attack }
    private State currentState = State.Patrol;

    [Header("Referências")]
    [SerializeField] private PlayerMoviment player;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private AudioSource stepAudioSource;
    [SerializeField] private AudioSource roarAudioSource;
    [SerializeField] private Transform handTransform;

    private NavMeshAgent agent;
    private int currentPatrolIndex = -1;
    private float currentWaitTime = 0f;
    private float maxWaitTime = 0f;

    [Header("Configurações de Movimento")]
    [SerializeField] private float detectionRadius = 8f;
    [SerializeField] private float attackDistance = 2.5f;
    [SerializeField] private float loseDistance = 12f;
    [SerializeField] private float giveUpTime = 3f;
    [SerializeField] private float patrolSpeed = 2.5f;
    [SerializeField] private float chaseSpeed = 6f;

    [Header("Raycast de visão")]
    [SerializeField] private float eyeHeight = 1.5f;
    [SerializeField] private LayerMask visionMask;

    private Coroutine giveUpCoroutine;
    private bool isHoldingPlayer = false;
    private Vector3 lastKnownPlayerPos = Vector3.zero;

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

        if (isHoldingPlayer)
            player.transform.position = handTransform.position;
    }

    private void Patrol()
    {
        animator.SetBool("isRunning", false);
        agent.speed = patrolSpeed;

        if (CanSeePlayer())
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

        StartCoroutine(RoarCoroutine());

        player.StartChase();
    }

    private IEnumerator RoarCoroutine()
    {
        yield return new WaitForSeconds(5.1f); // tempo da animação de rugido
        animator.SetTrigger("StartRun");
        ChangeState(State.Chase);
    }

    private void Chase()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", true);
        agent.speed = chaseSpeed;

        bool seesPlayerNow = CanSeePlayer();

        if (seesPlayerNow)
        {
            lastKnownPlayerPos = player.transform.position;

            if (giveUpCoroutine != null)
            {
                StopCoroutine(giveUpCoroutine);
                giveUpCoroutine = null;
            }

            agent.SetDestination(player.transform.position);

            if (Vector3.Distance(transform.position, player.transform.position) <= attackDistance)
            {
                ChangeState(State.Attack);
                return;
            }
        }
        else
        {
            // Ir para última posição conhecida
            agent.SetDestination(lastKnownPlayerPos);

            // Se ainda não tem timer rodando, inicia
            if (giveUpCoroutine == null)
                giveUpCoroutine = StartCoroutine(GiveUpTimer());
        }
    }

    private IEnumerator GiveUpTimer()
    {
        yield return new WaitForSeconds(giveUpTime);
      
        if (!CanSeePlayer())
        {
            ChangeState(State.Patrol);
            player.StopChase();
        }
            
        giveUpCoroutine = null;
    }

    private void Attack()
    {
        animator.SetTrigger("Attack");
        player.gameObject.GetComponent<PlayerMoviment>().canMove = false;
        isHoldingPlayer = true;
        agent.ResetPath();
        agent.speed = 0f;
        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        yield return new WaitForSeconds(4.04f);
        SceneManager.LoadScene(1);
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

    private bool CanSeePlayer()
    {
        Vector3 origin = transform.position + Vector3.up * eyeHeight;
        Vector3 direction = (player.transform.position + Vector3.up * 1.0f) - origin;
        float distance = direction.magnitude;

        if (distance > detectionRadius)
            return false;

        if (Physics.Raycast(origin, direction.normalized, out RaycastHit hit, detectionRadius, visionMask))
        {
            if (hit.transform.CompareTag("Player"))
            {
                Debug.DrawLine(origin, hit.point, Color.green);
                return true;
            }
            else
            {
                Debug.DrawLine(origin, hit.point, Color.red);
            }
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, loseDistance);
    }

    private void RoarSound() => roarAudioSource.Play();
    private void Step() => stepAudioSource.Play();
}
