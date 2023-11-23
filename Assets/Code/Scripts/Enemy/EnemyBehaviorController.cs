using System;
using System.Collections.Generic;
using System.Reflection;
using Cyan;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Events;

namespace Code.Scripts.Enemy
{
    public class EnemyBehaviorController : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float speed = 5f;
        [SerializeField] private GameObject player;

        private State state = State.Patrol;
        private VisionConeController controller;
        private Rigidbody rb;
        private Material impactLineMaterial;

        Vector3 currentEulerAngles;
        public UnityEvent onEnemyDeath;

        private enum State
        {
            Patrol, Investigate
        }

        // Start is called before the first frame update
        private void Start()
        {
            controller = GetComponentInChildren<VisionConeController>();
            rb = GetComponent<Rigidbody>();

            if (!player) player = GameObject.FindGameObjectWithTag("Player");

            // Get impact line material
            var renderer = (GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset).GetRenderer(0);
            var property = typeof(ScriptableRenderer).GetProperty("rendererFeatures", BindingFlags.NonPublic | BindingFlags.Instance);

            List<ScriptableRendererFeature> features = property.GetValue(renderer) as List<ScriptableRendererFeature>;

            foreach (var feature in features)
            {
                if (feature.GetType() == typeof(Blit))
                {
                    impactLineMaterial = (feature as Blit).settings.blitMaterial;
                    impactLineMaterial.SetFloat("_Strength", 0.0f);
                }
            }
        }

        private void FixedUpdate()
        {
            state = controller.DetectionProgress >= 1.0f ? State.Investigate : State.Patrol;

            switch (state)
            {
                case State.Patrol:
                    transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
                    impactLineMaterial.SetFloat("_Strength", 0.0f);
                    break;

                case State.Investigate:
                    var self = transform;
                    var position = self.position;
                    var playerPosition = player.transform.position;
                    var lookAt = playerPosition - position;
                    lookAt.y = 0;
                    lookAt.Normalize();

                    transform.rotation = Quaternion.RotateTowards(
                        self.rotation, Quaternion.LookRotation(lookAt), rotationSpeed * Time.deltaTime);

                    var dist = Mathf.SmoothStep(1f, 0f, Vector3.Distance(position, playerPosition) * 0.1f);
                    impactLineMaterial.SetFloat("_Strength", dist);


                    rb.MovePosition(position + speed * Time.deltaTime * lookAt);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            currentEulerAngles = transform.eulerAngles;
            Debug.Log(Mathf.Abs(currentEulerAngles.z));
            if ((Mathf.Abs(currentEulerAngles.z) > 85 && Mathf.Abs(currentEulerAngles.z) < 95) || (Mathf.Abs(currentEulerAngles.z) > 265 && Mathf.Abs(currentEulerAngles.z) < 275)) {
                onEnemyDeath?.Invoke();
                Destroy(gameObject); 
            }
            

        }
    }
}
