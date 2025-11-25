using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.SceneManagement;

public class MonsterScript : MonoBehaviour
{
    private enum State { Roar, Chase, Attack, Stunned }
    private State currentState = State.Stunned;

    [Header("Referências")]
    [SerializeField] private PlayerMoviment player;
    [SerializeField] private CameramanAgentScript cameraman;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource stepAudioSource;
    [SerializeField] private AudioSource roarAudioSource;
    [SerializeField] private Transform handTransform;

    private NavMeshAgent agent;

    [Header("Configurações de Movimento")]
    [SerializeField] private float stunCooldown = 5f;
    [SerializeField] private float attackDistance = 2.5f;
    [SerializeField] private float chaseSpeed = 6f;

    private Coroutine giveUpCoroutine;
    private bool isHoldingPlayer = false;
    private bool isHoldingCameraman = false;
    private bool isStunned = false;
    private Coroutine stunCoroutine;
    private bool canStun = true;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = true;
        agent.updateUpAxis = true;
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Roar: break;
            case State.Chase: Chase(); break;
            case State.Attack: break;
            case State.Stunned: StunnedBehavior(); break;
        }

        if (isHoldingPlayer)
            player.transform.position = handTransform.position;

        if (isHoldingCameraman)
            cameraman.transform.position = handTransform.position;
    }

    private void StunnedBehavior()
    {
        agent.ResetPath();
        agent.speed = 0;
        animator.SetBool("isRunning", false);
    }

    private void Roar()
    {
        animator.SetTrigger("Roar");
        agent.ResetPath();
        agent.speed = 0f;

        StartCoroutine(RoarCoroutine());

        player.StartChase();
        cameraman.StartChase(); // NOVO
    }

    private IEnumerator RoarCoroutine()
    {
        yield return new WaitForSeconds(5.1f);
        animator.SetTrigger("StartRun");
        ChangeState(State.Chase);
    }

    public void ApplyStun(float duration)
    {
        if (isStunned || !canStun) return;

        canStun = false;
        isStunned = true;
        ChangeState(State.Stunned);

        if (duration <= 0f) return;

        if (stunCoroutine != null) StopCoroutine(stunCoroutine);
        stunCoroutine = StartCoroutine(StunTimer(duration));
    }

    private IEnumerator StunTimer(float duration)
    {
        yield return new WaitForSeconds(duration);

        isStunned = false;
        ChangeState(State.Chase);

        StartCoroutine(StunCooldown());
    }

    private IEnumerator StunCooldown()
    {
        yield return new WaitForSeconds(stunCooldown);
        canStun = true;
    }

    private void Chase()
    {
        if (isStunned) return;

        animator.SetBool("isRunning", true);
        agent.speed = chaseSpeed;

        // Persegue sempre o player
        agent.SetDestination(player.transform.position);

        float distToPlayer = Vector3.Distance(transform.position, player.transform.position);
        float distToCameraman = Vector3.Distance(transform.position, cameraman.transform.position);

        if (distToPlayer <= attackDistance || distToCameraman <= attackDistance)
        {
            ChangeState(State.Attack);
        }
    }

    private void Attack()
    {
        if (isStunned) return;

        animator.SetTrigger("Attack");
        agent.ResetPath();
        agent.speed = 0f;

        float distPlayer = Vector3.Distance(transform.position, player.transform.position);
        float distCamera = Vector3.Distance(transform.position, cameraman.transform.position);

        if (distPlayer <= distCamera)
            GrabPlayer();
        else
            GrabCameraman();

        StartCoroutine(AttackCoroutine());
    }

    private void GrabPlayer()
    {
        player.isGrabbed = true;
        isHoldingPlayer = true;
    }

    private void GrabCameraman()
    {
        cameraman.isGrabbed = true;  // ADICIONE ESTA VAR NO CAMERA SCRIPT
        isHoldingCameraman = true;
    }

    private IEnumerator AttackCoroutine()
    {
        yield return new WaitForSeconds(4.04f);
        SceneManager.LoadScene(1);
    }

    private void ChangeState(State newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;

        switch (newState)
        {
            case State.Roar: Roar(); break;
            case State.Chase: break;
            case State.Attack: Attack(); break;
            case State.Stunned:
                agent.ResetPath();
                agent.speed = 0f;
                animator.SetTrigger("Stunned");
                RoarSound();
                break;
        }
    }

    void OnEnable()
    {
        isStunned = false;
        isHoldingPlayer = false;
        isHoldingCameraman = false;

        agent.ResetPath();
        agent.speed = chaseSpeed;

        ChangeState(State.Roar);
    }

    public void RoarSound() => roarAudioSource.Play();
    private void Step() => stepAudioSource.Play();
}
