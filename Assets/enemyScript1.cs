using UnityEngine;
using UnityEngine.AI;

public class EnemyChase1 : MonoBehaviour
{
    public Transform player;
    public NavMeshAgent agent;
    public float chaseDistance = 10f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateUpAxis = false;
        agent.updateRotation = false;
    }

    void Update()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= chaseDistance)
        {
            agent.SetDestination(player.position);
        }
    }
}