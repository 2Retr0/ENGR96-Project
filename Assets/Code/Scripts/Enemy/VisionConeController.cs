using System;
using UnityEngine;

namespace Code.Scripts.Enemy
{
    public class VisionConeController : MonoBehaviour
    {
        [SerializeField] private float detectionRateSeconds = 1.0f;
        [SerializeField] public float range = 5f;
        [SerializeField] public float arcAngle = 45f;

        [NonSerialized] public float DetectionProgress = 0.0f;

        // Start is called before the first frame update
        void Start()
        {
        }

        public void ResetDetectionProgress()
        {
            DetectionProgress = 0.0f;
        }

        /** Assumed to be called *once* per `FixedUpdate()`*/
        public void UpdateDetectionProgress(bool hasDetectedPlayer)
        {
            DetectionProgress += Time.deltaTime / detectionRateSeconds * (hasDetectedPlayer ? 1 : -0.5f);
            DetectionProgress = Mathf.Clamp(DetectionProgress, 0.0f, 1.0f);
        }
    }
}
