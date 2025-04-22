using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Game UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI waveTimerText;

    [Header("Panels")]
    public GameObject gameOverPanel;
    public GameObject nextWavePanel;
    public GameObject upgradePanel;

    [Header("Buttons")]
    public Button restartButton;
    public Button continueButton;

    [Header("Upgrade UI")]
    public GameObject upgradeOptionPrefab;
    public Transform upgradeOptionsContainer;

    private Action onContinuePressed;
    private UpgradeData selectedUpgrade;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        gameOverPanel?.SetActive(false);
        nextWavePanel?.SetActive(false);
    }

    public void InitializeUI(Action continueCallback, Action restartCallback)
    {
        onContinuePressed = continueCallback;

        restartButton?.onClick.AddListener(() => restartCallback?.Invoke());
        continueButton?.onClick.AddListener(() =>
        {
            if (selectedUpgrade != null)
                onContinuePressed?.Invoke();
        });

        gameOverPanel?.SetActive(false);
        nextWavePanel?.SetActive(false);
        upgradePanel?.SetActive(false);
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
    }

    public void UpdateWave(int waveNumber)
    {
        if (waveText != null)
            waveText.text = $"WAVE {waveNumber}";
    }

    public void UpdateWaveTimer(float time, bool isWaveOngoing)
    {
        if (waveTimerText == null) return;
        waveTimerText.text = isWaveOngoing ? $"{time:F1}s" : "Clear";
    }

    public void ShowGameOver()
    {
        gameOverPanel?.SetActive(true);
    }

    public void ShowNextWavePanel(bool active)
    {
        nextWavePanel?.SetActive(active);
        upgradePanel?.SetActive(active);
    }

    public void SetContinueButtonInteractable(bool interactable)
    {
        continueButton.interactable = interactable;
    }

    public UpgradeData GetSelectedUpgrade()
    {
        return selectedUpgrade;
    }

    public void ShowUpgradeChoices(List<UpgradeData> upgrades)
    {
        ClearUpgradeOptions();
        upgradePanel?.SetActive(true);
        SetContinueButtonInteractable(false);

        foreach (var data in upgrades)
        {
            GameObject go = Instantiate(upgradeOptionPrefab, upgradeOptionsContainer);
            var ui = go.GetComponent<UpgradeOptionUI>();
            ui.Setup(data, OnSelectUpgrade);
            go.GetComponent<Button>().onClick.AddListener(ui.OnClick);
        }
    }

    private void OnSelectUpgrade(UpgradeData data, UpgradeOptionUI ui)
    {
        selectedUpgrade = data;
        SetContinueButtonInteractable(true);

        foreach (Transform child in upgradeOptionsContainer)
        {
            var option = child.GetComponent<UpgradeOptionUI>();
            option?.SetSelected(option == ui);
        }
    }

    private void ClearUpgradeOptions()
    {
        foreach (Transform child in upgradeOptionsContainer)
        {
            Destroy(child.gameObject);
        }
        selectedUpgrade = null;
    }
}