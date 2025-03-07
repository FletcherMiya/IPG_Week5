using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public CharacterStats stats = new CharacterStats(10, 20, 5f);
    public float attackRange = 5f;
    public float attackCooldown = 0.5f;
    private float lastAttackTime;

    public GameObject projectilePrefab;
    private Rigidbody2D rb;
    private Vector2 movementInput;

    private Vector2 minBounds, maxBounds;

    private bool isInvincible = false;
    public float invincibilityDuration = 1f;
    public float flashInterval = 0.1f;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.drag = 0;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        CalculateCameraBounds();
    }

    void Update()
    {
        HandleInput();
        Attack();
    }

    void FixedUpdate()
    {
        Move();
    }

    void HandleInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        movementInput = new Vector2(h, v).normalized;
    }

    void Move()
    {
        Vector2 newPosition = rb.position + movementInput * stats.speed * Time.fixedDeltaTime;
        newPosition = ClampToCameraBounds(newPosition);
        rb.MovePosition(newPosition);
    }

    void Attack()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearestEnemy = null;
        float minDist = attackRange;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy)
        {
            Vector2 dir = (nearestEnemy.transform.position - transform.position).normalized;
            FireProjectile(dir);
            lastAttackTime = Time.time;
        }
    }

    void FireProjectile(Vector2 direction)
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.GetComponent<Projectile>().SetDirection(direction);
    }

    void CalculateCameraBounds()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        Vector3 camPosition = cam.transform.position;
        minBounds = new Vector2(camPosition.x - halfWidth, camPosition.y - halfHeight);
        maxBounds = new Vector2(camPosition.x + halfWidth, camPosition.y + halfHeight);
    }

    Vector2 ClampToCameraBounds(Vector2 position)
    {
        float clampedX = Mathf.Clamp(position.x, minBounds.x, maxBounds.x);
        float clampedY = Mathf.Clamp(position.y, minBounds.y, maxBounds.y);
        return new Vector2(clampedX, clampedY);
    }

    public void TakeDamage(float damage)
    {
        if (isInvincible) return;

        stats.health -= damage;
        Debug.Log($"玩家受到 {damage} 点伤害，剩余血量: {stats.health}");

        DamageEffect.Instance.ShowDamageEffect();
        CameraShake.Instance.Shake();

        if (stats.health <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityCoroutine());
        }
    }

    void Die()
    {
        Debug.Log("游戏结束，玩家死亡！");
        Time.timeScale = 0;
    }

    IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        float elapsedTime = 0f;
        bool isVisible = true;

        while (elapsedTime < invincibilityDuration)
        {
            elapsedTime += flashInterval;
            isVisible = !isVisible;

            if (spriteRenderer != null)
            {
                Color newColor = originalColor;
                newColor.a = isVisible ? 1f : 0.2f;
                spriteRenderer.color = newColor;
            }

            yield return new WaitForSeconds(flashInterval);
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        isInvincible = false;
    }
}