using UnityEngine;

public class Enemy : MonoBehaviour
{
    public CharacterStats stats;
    protected Transform player;

    public float damage = 10f;

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected virtual void Update()
    {
        MoveTowardsPlayer();
    }

    protected void MoveTowardsPlayer()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.position, stats.speed * Time.deltaTime);
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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().TakeDamage(damage);
        }
    }
}