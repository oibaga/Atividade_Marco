using UnityEngine;

public class EnemyChase1 : MonoBehaviour
{
    public Transform player;
    public float speed = 3f;
    public float chaseDistance = 5f;

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance < chaseDistance)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position += (Vector3)direction * speed * Time.deltaTime;
        }
    }
}
