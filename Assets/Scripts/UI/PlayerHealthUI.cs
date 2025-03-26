using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public Image healthFill;
    private Player player;

    public float transitionDuration = 0.3f;
    public float decelerationFactor = 2f;

    private float displayFill;
    private float targetFill;
    private float transitionTime;

    void Start()
    {
        player = FindObjectOfType<Player>();
        if (player != null)
        {
            targetFill = player.stats.health / player.stats.maxHealth;
            displayFill = targetFill;
            healthFill.fillAmount = displayFill;
        }
    }

    void Update()
    {
        if (player == null || player.stats == null) return;

        float currentTarget = Mathf.Clamp01(player.stats.health / player.stats.maxHealth);

        if (!Mathf.Approximately(currentTarget, targetFill))
        {
            targetFill = currentTarget;
            transitionTime = Time.time;
        }

        float elapsed = (Time.time - transitionTime) / transitionDuration;
        float smoothedProgress = Mathf.SmoothStep(0f, 1f, Mathf.Pow(elapsed, decelerationFactor));
        displayFill = Mathf.Lerp(displayFill, targetFill, smoothedProgress);

        healthFill.fillAmount = displayFill;
    }
}