using UnityEngine;

public class Enemy : MonoBehaviour
{
    public CharacterStats stats;
    protected Transform player;
    private Rigidbody2D rb;

    public float damage = 10f; // 敌人造成的伤害
    public float stoppingDistance = 0.5f; // 防止敌人紧贴玩家

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }

    protected virtual void FixedUpdate()
    {
        MoveTowardsPlayer();
    }

    protected void MoveTowardsPlayer()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > stoppingDistance)
        {
            rb.velocity = direction * stats.speed;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    public void TakeDamage(float amount)
    {
        stats.health -= amount;
        if (stats.health <= 0)
        {
            GameManager.Instance.AddScore(10);
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player>().TakeDamage(damage);
        }
    }
}