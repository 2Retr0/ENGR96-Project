using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace Code.Scripts.Pickup
{
    public class PickupController : MonoBehaviour
    {
        private void Start()
        {
            CompassManager.Instance.AddPosition(transform.position);
            PickupSpawnManager.Instance.TrackPoint(transform.position);
        }

        public void OnDestroy()
        {
            var position = transform.position;
            if (CompassManager.Instance)
                CompassManager.Instance.RemovePosition(position);
            if (PickupSpawnManager.Instance)
                PickupSpawnManager.Instance.UntrackPoint(position);
        }
    }
}
