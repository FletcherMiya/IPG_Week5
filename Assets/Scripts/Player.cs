using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour, IDamageable
{
    public CharacterStats stats = new CharacterStats(10, 20, 5f);

    private Rigidbody2D rb;
    private Vector2 movementInput;

    private Vector2 minBounds, maxBounds;

    private bool isInvincible = false;
    public float invincibilityDuration = 1f;
    public float flashInterval = 0.1f;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private bool isDead = false;



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
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector2 movementInput = new Vector2(h, v).normalized;

        rb.velocity = rb.velocity * 0.9f + movementInput * stats.speed * 0.1f;
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
        if (isInvincible || isDead) return;

        stats.health -= damage;
        Debug.Log($"Player took {damage} damage, hp remaining: {stats.health}");

        ScreenEffect.Instance.TriggerEffect(0.5f, 0.5f, Color.red);
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

    public bool checkDead()
    {
        return (isDead);
    }


    private void Die()
    {
        if (isDead) return;
        isDead = true;

        StopAllCoroutines();
        rb.velocity = Vector2.zero;

        ScreenEffect.Instance.CancelEffect();

        CameraShake.Instance.StopShake();

        GameManager.Instance.GameOver();
    }

    IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);

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

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);

        isInvincible = false;
    }
}