using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Scripts.Pickup
{
    public class PickupSpawnManager : Singleton<PickupSpawnManager>
    {
        [SerializeField] private GameObject pickupPrefab;
        [SerializeField] private int maxActivePickups = 2;

        private readonly HashSet<Vector3> activePoints = new();
        private Vector3 lastPickupPosition = Vector3.positiveInfinity;
        private readonly HashSet<Vector3> spawnPoints = new();

        private void FixedUpdate()
        {
            if (activePoints.Count >= Math.Min(spawnPoints.Count, maxActivePickups))
                return;

            SpawnNewPickup();
        }

        public void TrackPoint(Vector3 position)
        {
            activePoints.Add(position);
        }

        public void UntrackPoint(Vector3 position)
        {
            activePoints.Remove(position);
            lastPickupPosition = position;
        }

        private void SpawnNewPickup()
        {
            var remaining = new HashSet<Vector3>(spawnPoints);
            remaining.ExceptWith(activePoints);
            remaining.Remove(lastPickupPosition);

            var point0 = remaining.ElementAt(Random.Range(0, remaining.Count));
            var point1 = remaining.ElementAt(Random.Range(0, remaining.Count)); // Chance of same element but idc
            var spawnPoint = Vector3.Distance(lastPickupPosition, point0) > Vector3.Distance(lastPickupPosition, point1) ? point0 : point1;

            Instantiate(pickupPrefab, spawnPoint, Quaternion.identity);
        }

        public void AddSpawnPoint(Vector3 position)
        {
            spawnPoints.Add(position);
        }
    }
}
