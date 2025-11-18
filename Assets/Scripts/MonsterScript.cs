using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class MonsterScript : MonoBehaviour
{
    private enum State { Roar, Chase, Attack, Stunned }
    private State currentState = State.Stunned;

    [Header("Referências")]
    [SerializeField] private PlayerMoviment player;
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
    private Vector3 lastKnownPlayerPos = Vector3.zero;
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
            case State.Roar:
                break;
            case State.Chase:
                Chase();
                break;
            case State.Attack:
                break;
            case State.Stunned:
                StunnedBehavior();
                break;
        }

        if (isHoldingPlayer)
            player.transform.position = handTransform.position;
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
    }

    private IEnumerator RoarCoroutine()
    {
        yield return new WaitForSeconds(5.1f); // tempo da animação de rugido
        animator.SetTrigger("StartRun");
        ChangeState(State.Chase);
    }

    public void ApplyStun(float duration)
    {
        if (isStunned || !canStun) return;

        canStun = false;
        isStunned = true;
        ChangeState(State.Stunned);

        if (duration <= 0f) return; // a única coisa que termina o stun é o timer, se o timer não iniciar (is permanent), o stun nunca acaba

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

        agent.SetDestination(player.transform.position);

        if (Vector3.Distance(transform.position, player.transform.position) <= attackDistance)
        {
            ChangeState(State.Attack);
            return;
        }
    }

    private void Attack()
    {
        if (isStunned) return;

        animator.SetTrigger("Attack");
        player.gameObject.GetComponent<PlayerMoviment>().isGrabbed = true;
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

    private void ChangeState(State newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;

        switch (newState)
        {   
            case State.Roar:
                Roar();
                break;

            case State.Chase:
                break;

            case State.Attack:
                Attack();
                break;

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

        agent.ResetPath();
        agent.speed = chaseSpeed;

        ChangeState(State.Roar);
    }

    public void RoarSound() => roarAudioSource.Play();
    private void Step() => stepAudioSource.Play();
}
