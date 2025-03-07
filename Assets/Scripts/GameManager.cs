using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject normalEnemyPrefab;
    public GameObject bossEnemyPrefab;
    public Transform player;

    public float spawnInterval = 2f;
    public float minSpawnDistance = 5f; 
    public float maxSpawnDistance = 10f;

    public float bossSpawnChance = 0.1f;

    private int score = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), 1f, spawnInterval);
    }

    private void SpawnEnemy()
    {
        if (player == null) return;

        Vector2 spawnPosition = GetRandomSpawnPosition();


        GameObject enemyPrefab = (Random.value < bossSpawnChance) ? bossEnemyPrefab : normalEnemyPrefab;
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }

    private Vector2 GetRandomSpawnPosition()
    {
        float distance = Random.Range(minSpawnDistance, maxSpawnDistance);
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

        Vector2 spawnOffset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;
        return (Vector2)player.position + spawnOffset;
    }

    public void AddScore(int amount)
    {
        score += amount;
        Debug.Log("当前分数: " + score);
    }
}