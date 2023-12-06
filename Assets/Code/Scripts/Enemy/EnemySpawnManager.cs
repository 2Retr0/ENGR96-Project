using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Scripts.Enemy
{
    public class EnemySpawnManager : Singleton<EnemySpawnManager>
    {
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] public int maxActiveEnemies = 2;
        [SerializeField] private Camera playerCamera;

        private int numEnemies = 0;
        private Vector3 lastSpawnPoint = Vector3.positiveInfinity;
        private readonly HashSet<Vector3> spawnPoints = new();

        private void Start()
        {
            if (!playerCamera) playerCamera = Camera.main;
        }

        private void FixedUpdate()
        {
            // print(numEnemies);
            if (numEnemies < maxActiveEnemies)
                SpawnNewEnemy();
        }

        public void TrackEnemy()
        {
            numEnemies++;
        }

        public void UntrackEnemy()
        {
            numEnemies--;
        }

        private void SpawnNewEnemy()
        {
            var spawnPoint = spawnPoints.ElementAt(Random.Range(0, spawnPoints.Count));
            var viewportPos = playerCamera.WorldToViewportPoint(spawnPoint);

            if (((viewportPos.x is > -1f and < 2f) && (viewportPos.y is > -1f and < 2f)) || (spawnPoints.Count != 1 && spawnPoint == lastSpawnPoint))
                return;

            Instantiate(enemyPrefab, spawnPoint, Quaternion.identity);
            lastSpawnPoint = spawnPoint;
        }

        public void AddSpawnPoint(Vector3 position)
        {
            spawnPoints.Add(position);
        }
    }
}
