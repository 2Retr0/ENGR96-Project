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
        }

        private void OnDestroy()
        {
            CompassManager.Instance.RemovePosition(transform.position);
        }
    }
}
