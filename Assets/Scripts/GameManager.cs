using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Enemy Spawning")]
    public GameObject normalEnemyPrefab;
    public GameObject bossEnemyPrefab;
    public Transform player;
    public float minSpawnDistance = 5f;
    public float maxSpawnDistance = 10f;
    public float bossSpawnChance = 0.1f;


    [Header("Wave Control")]
    public float initialWaveDuration = 30f;
    public float waveDurationIncrement = 5f;
    public float maxWaveDuration = 60f;

    public float initialSpawnInterval = 3f;
    public float spawnIntervalDecrement = 0.3f;
    public float minSpawnInterval = 0.5f;

    public int initialSpawnCount = 2;
    public int spawnCountIncrement = 1;

    private float currentWaveDuration;
    private float currentSpawnInterval;
    private int currentSpawnCount;
    private float waveTimer;

    private bool waveOngoing = false;
    private Coroutine spawnRoutine;
    private bool isGameOver = false;

    private int currentWave = 0;
    private int score = 0;
    public int Score => score;

    public bool IsGamePaused => Time.timeScale == 0f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

    }

    private void Start()
    {
        UIManager.Instance.InitializeUI(StartNextWave, RestartGame);

        InitWaveSystem();
        StartNextWave();
    }

    private void Update()
    {
        if (isGameOver) return;

        UIManager.Instance.UpdateScore(score);
        UIManager.Instance.UpdateWaveTimer(waveTimer, waveOngoing);

        if (waveOngoing)
        {
            waveTimer -= Time.unscaledDeltaTime;

            if (waveTimer <= 0f)
            {
                waveTimer = 0f;
                EndWave();
            }
        }
    }

    private void InitWaveSystem()
    {
        currentWave = 0;
        currentWaveDuration = initialWaveDuration;
        currentSpawnInterval = initialSpawnInterval;
        currentSpawnCount = initialSpawnCount;
    }

    private void StartNextWave()
    {
        if (isGameOver) return;

        UpgradeData selected = UIManager.Instance.GetSelectedUpgrade();
        if (selected != null)
        {
            ApplyUpgrade(selected);
        }

        ClearAllEnemies();

        Time.timeScale = 1f;

        currentWave++;
        waveTimer = currentWaveDuration;

        UIManager.Instance.UpdateWave(currentWave);
        UIManager.Instance.ShowNextWavePanel(false);
        UIManager.Instance.SetContinueButtonInteractable(false);

        waveOngoing = true;

        if (spawnRoutine != null) StopCoroutine(spawnRoutine);
        spawnRoutine = StartCoroutine(SpawnLoop());
    }

    private void EndWave()
    {
        waveOngoing = false;
        Time.timeScale = 0f;

        UIManager.Instance.ShowNextWavePanel(true);
        UIManager.Instance.SetContinueButtonInteractable(false);
        List<UpgradeData> upgradeChoices = UpgradeLibrary.GetRandomUpgrades(3);
        UIManager.Instance.ShowUpgradeChoices(upgradeChoices);

        currentWaveDuration = Mathf.Min(currentWaveDuration + waveDurationIncrement, maxWaveDuration);
        currentSpawnInterval = Mathf.Max(currentSpawnInterval - spawnIntervalDecrement, minSpawnInterval);
        currentSpawnCount += spawnCountIncrement;

        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);
    }

    private IEnumerator SpawnLoop()
    {
        while (waveOngoing)
        {
            for (int i = 0; i < currentSpawnCount; i++)
                SpawnEnemy();

            yield return new WaitForSeconds(currentSpawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        if (player == null) return;

        Vector2 spawnPosition = GetRandomSpawnPosition();
        GameObject enemyPrefab = (Random.value < bossSpawnChance) ? bossEnemyPrefab : normalEnemyPrefab;
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }

    private void ClearAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (var enemy in enemies)
        {
            Destroy(enemy);
        }
    }

    private Vector2 GetRandomSpawnPosition()
    {
        float distance = Random.Range(minSpawnDistance, maxSpawnDistance);
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;
        return (Vector2)player.position + offset;
    }

    public void AddScore(int amount)
    {
        score += amount;
    }

    public void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f;
        UIManager.Instance.ShowGameOver();

        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);
    }

    public void RestartGame()
    {
        isGameOver = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ApplyUpgrade(UpgradeData data)
    {
        data.effect?.Apply(player.gameObject);
    }
}
