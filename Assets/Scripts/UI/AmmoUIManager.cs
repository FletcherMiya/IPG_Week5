using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AmmoUIManager : MonoBehaviour
{
    public GameObject bulletSegmentPrefab;
    public Weapon weapon;

    private List<GameObject> bulletSegments = new List<GameObject>();
    private float containerWidth;

    public TextMeshProUGUI ammoText;

    void Start()
    {
        if (weapon == null)
            weapon = FindObjectOfType<Weapon>();

        containerWidth = ((RectTransform)transform).rect.width;

        BuildSegments();
        UpdateSegments();
        UpdateAmmoText();
    }

    void Update()
    {
        if (weapon != null)
        {
            UpdateSegments();
            UpdateAmmoText();
        }
    }

    [SerializeField] private float spacing = 5f;

    void BuildSegments()
    {
        foreach (var seg in bulletSegments)
            Destroy(seg);
        bulletSegments.Clear();

        if (weapon == null || bulletSegmentPrefab == null) return;

        int maxAmmo = weapon.MaxAmmo;

        float totalSpacing = spacing * (maxAmmo - 1);
        float segmentWidth = (containerWidth - totalSpacing) / maxAmmo;
        float segmentHeight = ((RectTransform)transform).rect.height;

        for (int i = 0; i < maxAmmo; i++)
        {
            GameObject seg = Instantiate(bulletSegmentPrefab, transform);
            RectTransform rect = seg.GetComponent<RectTransform>();

            rect.anchorMin = new Vector2(0, 0.5f);
            rect.anchorMax = new Vector2(0, 0.5f);
            rect.pivot = new Vector2(0, 0.5f);
            rect.sizeDelta = new Vector2(segmentWidth, segmentHeight);

            float x = containerWidth - (i + 1) * segmentWidth - i * spacing;
            rect.anchoredPosition = new Vector2(x, 0);

            bulletSegments.Add(seg);
        }
    }


    void UpdateSegments()
    {
        int currentAmmo = weapon.CurrentAmmo;

        for (int i = 0; i < bulletSegments.Count; i++)
        {
            int reverseIndex = bulletSegments.Count - 1 - i;
            bulletSegments[reverseIndex].SetActive(i < currentAmmo);
        }

    }

    void UpdateAmmoText()
    {
        if (ammoText == null || weapon == null) return;

        if (weapon.IsReloading)
        {
            ammoText.text = "Reloading";
        }
        else
        {
            ammoText.text = $"{weapon.CurrentAmmo}";
        }
    }
}