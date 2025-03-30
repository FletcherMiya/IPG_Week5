using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 0.2f;

    private Vector3 originalPosition;

    private Coroutine shakeCoroutine;

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
    }

    void Start()
    {
        originalPosition = transform.localPosition;
    }

    public void Shake()
    {
        StopAllCoroutines();
        shakeCoroutine = StartCoroutine(ShakeCoroutine());
    }

    public void Shake(float magnitude, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(ShakeCoroutine(magnitude, duration));
    }

    public void StopShake()
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            shakeCoroutine = null;
            transform.localPosition = originalPosition;
        }
    }

    private IEnumerator ShakeCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;
            transform.localPosition = originalPosition + new Vector3(x, y, 0);

            yield return null;
        }

        transform.localPosition = originalPosition;
    }

    private IEnumerator ShakeCoroutine(float magnitude, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            transform.localPosition = originalPosition + new Vector3(x, y, 0);

            yield return null;
        }

        transform.localPosition = originalPosition;
    }
}
