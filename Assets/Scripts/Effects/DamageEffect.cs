using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DamageEffect : MonoBehaviour
{
    public static DamageEffect Instance; // 让其他脚本能调用它

    private Image damageImage;
    public float fadeDuration = 0.5f;
    public float maxAlpha = 0.5f;

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

        damageImage = GetComponent<Image>();

        if (damageImage == null)
        {
            Debug.LogError("DamageOverlay 需要 Image 组件！");
            return;
        }

        Color c = damageImage.color;
        c.a = 0;
        damageImage.color = c;
    }

    public void ShowDamageEffect()
    {
        StopCoroutine(FadeDamageEffect());
        StartCoroutine(FadeDamageEffect());
    }

    private IEnumerator FadeDamageEffect()
    {
        Color c = damageImage.color;
        c.a = maxAlpha;
        damageImage.color = c;

        float timer = 0;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            c.a = Mathf.Lerp(maxAlpha, 0, timer / fadeDuration);
            damageImage.color = c;
            yield return null;
        }

        c.a = 0;
        damageImage.color = c;
    }
}