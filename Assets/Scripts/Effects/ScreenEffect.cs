using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenEffect : MonoBehaviour
{
    public static ScreenEffect Instance;

    private Image effectImage;
    private Color originalColor;

    private Coroutine currentEffectCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        effectImage = GetComponent<Image>();

        if (effectImage == null)
        {
            return;
        }

        originalColor = effectImage.color;
        SetAlpha(0);
    }

    public void TriggerEffect(float maxAlpha, float duration, Color? effectColor = null)
    {
        if (effectColor.HasValue)
        {
            originalColor = effectColor.Value;
        }

        StopAllCoroutines();
        currentEffectCoroutine = StartCoroutine(EffectCoroutine(maxAlpha, duration));
    }

    public void CancelEffect()
    {
        if (currentEffectCoroutine != null)
        {
            StopCoroutine(currentEffectCoroutine);
            currentEffectCoroutine = null;
            SetAlpha(0f);
        }
    }

    private IEnumerator EffectCoroutine(float maxAlpha, float duration)
    {
        SetAlpha(maxAlpha);

        float timer = 0;
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(maxAlpha, 0, timer / duration);
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(0);
    }

    private void SetAlpha(float alpha)
    {
        Color newColor = originalColor;
        newColor.a = alpha;
        effectImage.color = newColor;
    }
}
