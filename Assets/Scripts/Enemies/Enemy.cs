using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public CharacterStats stats;
    protected Transform player;
    private Rigidbody2D rb;

    public float damage = 10f;
    public float stoppingDistance = 0.5f;

    private AITurretController aiController;

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;

        aiController = GetComponentInChildren<AITurretController>();
        if (aiController == null)
        {
            Debug.LogError("Enemy missing AITurretController on child.");
        }
    }

    protected virtual void FixedUpdate()
    {
        MoveTowardsAITarget();
    }

    protected void MoveTowardsAITarget()
    {
        if (aiController == null || aiController.moveTarget == null) return;

        Vector2 direction = (aiController.moveTarget.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, aiController.moveTarget.position);

        if (distance > stoppingDistance)
        {
            rb.velocity = direction * stats.speed;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    public void TakeDamage(float damage)
    {
        stats.health -= damage;

        AITurretController ai = GetComponentInChildren<AITurretController>();
        if (ai != null)
        {
            ai.AlertFromDamage();
        }

        if (stats.health <= 0)
        {
            GameManager.Instance.AddScore(10);
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            if (damageable is Player)
            {
                damageable.TakeDamage(damage);
            }
        }
    }
}