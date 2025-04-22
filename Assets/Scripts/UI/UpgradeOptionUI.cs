using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeOptionUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public GameObject selectionFrame;

    private UpgradeData upgradeData;
    private System.Action<UpgradeData, UpgradeOptionUI> onSelect;

    public void Setup(UpgradeData data, System.Action<UpgradeData, UpgradeOptionUI> selectCallback)
    {
        upgradeData = data;
        onSelect = selectCallback;

        nameText.text = data.upgradeName;
        descriptionText.text = data.description;
        selectionFrame.SetActive(false);
        SetSelected(false);

    }

    public void OnClick()
    {
        onSelect?.Invoke(upgradeData, this);
    }

    public void SetSelected(bool selected)
    {
        Debug.Log($"SetSelected called on {name} with state: {selected}");
        selectionFrame?.SetActive(selected);
    }
}