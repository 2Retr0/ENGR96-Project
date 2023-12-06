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
            CompassManager.Instance.RemovePosition(position);
            PickupSpawnManager.Instance.UntrackPoint(position);
        }
    }
}
