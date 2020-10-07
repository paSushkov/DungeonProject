using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace Dungeon.Temp
{
    public class EnemySpawner : MonoBehaviour
    {
        #region PrivateData

        [SerializeField] private List<GameObject> _enemyPrefabs = new List<GameObject>();
        [SerializeField] private int spawnOnStart;
        private IEnumerator _cachedDelay;
        private string _cachedCoroutineName;

        #endregion

        #region Fields

        public bool spawnActive = true;
        public float newEnemySpawnTime = 30f;
        public SpawnPointHolder spawnPointHolder;

        #endregion

        #region UnityMethods

        private void Start()
        {
            _cachedDelay = new WaitForSecondsRealtime(newEnemySpawnTime);
            _cachedCoroutineName = nameof(SpawnRandomEnemy);
            StartCoroutine(_cachedCoroutineName);
            for (int i = 0; i < spawnOnStart; i++)
            {
                SpawnEnemy();
            }
        }

        #endregion

        #region Methods

        private IEnumerator SpawnRandomEnemy()
        {
            yield return _cachedDelay;
            if (spawnActive)
            {
                SpawnEnemy();
                StartCoroutine(_cachedCoroutineName);
            }
            else
            {
                yield return null;
            }
        }

        private void SpawnEnemy()
        {
            var spawnPosition = spawnPointHolder.GetRandomSpawnPointPosition();
            var enemy = Instantiate(_enemyPrefabs[Random.Range(0, _enemyPrefabs.Capacity)], spawnPosition,
                Quaternion.identity);
            if (enemy.TryGetComponent(out NavMeshAgent agent))
                agent.avoidancePriority = 50 + Random.Range(0, 15);
        }

        #endregion
    }
}