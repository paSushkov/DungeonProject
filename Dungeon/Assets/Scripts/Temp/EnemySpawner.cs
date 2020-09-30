using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    public bool spawnActive = true;
    public float newEnemySpawnTime = 30f;
    public SpawnPointHolder spawnPointHolder;
    [SerializeField] private List<GameObject> enemyPrefabs = new List<GameObject>();
    private IEnumerator cachedDelay;
    private string cachedCoroutineName;

    private void Awake()
    {
        cachedDelay = new WaitForSecondsRealtime(newEnemySpawnTime);
        cachedCoroutineName = nameof(SpawnRandomEnemy);
        StartCoroutine(cachedCoroutineName);
        SpawnEnemy();
    }

    private IEnumerator SpawnRandomEnemy()
    {
        yield return cachedDelay;
        if (spawnActive)
        {
            SpawnEnemy();
            StartCoroutine(cachedCoroutineName);
        }
        else
        {
            yield return null;
        }
    }

    private void SpawnEnemy()
    {
        var spawnPosition = spawnPointHolder.GetRandomSpawnPointPosition();
        Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Capacity)], spawnPosition, Quaternion.identity);
    }
}