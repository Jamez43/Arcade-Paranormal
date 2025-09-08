using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    private Transform player;
    [SerializeField] private EnemyStats stats;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position += (Vector3)direction * stats.Speed * Time.deltaTime;
        }
    }
}