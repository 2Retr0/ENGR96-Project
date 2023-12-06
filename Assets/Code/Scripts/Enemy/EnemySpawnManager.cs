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
        [SerializeField] private int maxActiveEnemies = 2;

        private readonly HashSet<Vector3> activePoints = new();
        private Vector3 lastPickupPosition = Vector3.positiveInfinity;
        private readonly HashSet<Vector3> spawnPoints = new();

        private void FixedUpdate()
        {
            if (activePoints.Count >= Math.Min(spawnPoints.Count, maxActiveEnemies))
                return;

            SpawnNewPickup();
        }

        private void SpawnNewPickup()
        {
            var remaining = new HashSet<Vector3>(spawnPoints);
            remaining.ExceptWith(activePoints);
            remaining.Remove(lastPickupPosition);

            var point0 = remaining.ElementAt(Random.Range(0, remaining.Count));
            var point1 = remaining.ElementAt(Random.Range(0, remaining.Count)); // Chance of same element but idc
            var spawnPoint = Vector3.Distance(lastPickupPosition, point0) > Vector3.Distance(lastPickupPosition, point1) ? point0 : point1;

            Instantiate(enemyPrefab, spawnPoint, Quaternion.identity);
        }

        public void AddSpawnPoint(Vector3 position)
        {
            spawnPoints.Add(position);
        }
    }
}
