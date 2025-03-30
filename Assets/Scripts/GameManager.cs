using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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

    [Header("UI Control")]
    public GameObject gameOverPanel;
    public Button restartButton;
    public TextMeshProUGUI scoreText;

    public GameObject nextWavePanel;
    public Button continueButton;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI waveTimerText;

    public bool IsGamePaused => Time.timeScale == 0f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        gameOverPanel?.SetActive(false);
        nextWavePanel?.SetActive(false);
    }

    private void Start()
    {
        restartButton?.onClick.AddListener(RestartGame);
        continueButton?.onClick.AddListener(StartNextWave);

        InitWaveSystem();
        StartNextWave();
    }

    private void Update()
    {
        if (isGameOver) return;

        UpdateScoreText();
        UpdateWaveTimerText();

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

        ClearAllEnemies();

        Time.timeScale = 1f;

        currentWave++;
        waveTimer = currentWaveDuration;
        waveText.text = $"WAVE {currentWave}";

        nextWavePanel?.SetActive(false);
        waveOngoing = true;

        if (spawnRoutine != null) StopCoroutine(spawnRoutine);
        spawnRoutine = StartCoroutine(SpawnLoop());

        UpdateWaveTimerText();
    }

    private void UpdateWaveTimerText()
    {
        if (waveTimerText == null) return;

        if (waveOngoing)
        {
            waveTimerText.text = $"{waveTimer:F1}s";
        }
        else
        {
            waveTimerText.text = "Clear";
        }
    }

    private void EndWave()
    {
        waveOngoing = false;
        Time.timeScale = 0f;
        nextWavePanel?.SetActive(true);

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

    void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
    }

    public void GameOver()
    {
        isGameOver = true;

        Time.timeScale = 0f;
        gameOverPanel?.SetActive(true);

        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);
    }

    public void RestartGame()
    {
        isGameOver = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
