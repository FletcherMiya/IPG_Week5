using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    public CharacterStats stats = new CharacterStats(100, 20, 5f);
    public float attackRange = 5f;
    public float attackCooldown = 0.5f;
    private float lastAttackTime;

    public GameObject projectilePrefab;
    private Rigidbody2D rb;
    private Vector2 movementInput;

    private Vector2 minBounds, maxBounds;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.drag = 0;

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
}