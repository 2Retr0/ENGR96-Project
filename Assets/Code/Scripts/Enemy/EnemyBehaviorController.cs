using System;
using System.Collections.Generic;
using System.Reflection;
using Code.Scripts.Text;
using Cyan;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;

namespace Code.Scripts.Enemy
{
    public class EnemyBehaviorController : MonoBehaviour
    {
        [SerializeField] private GameObject textMesh;

        public float rotationSpeed;
        public float speed = 5f;
        public GameObject player;


        private State state = State.Patrol;
        private VisionConeController controller;
        private Rigidbody rb;
        private Material impactLineMaterial;
        private GameObject stateText;

        Vector3 currentEulerAngles;
        
        public bool dead;

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

            dead = false;
        }

        private void SetText(string text, float lifetime = 3)
        {
            Destroy(stateText);
            stateText = Instantiate(textMesh, transform.position, Quaternion.identity);
            stateText.GetComponent<TextMeshPro>().text = text;
            stateText.GetComponent<TextController>().lifetimeSeconds = lifetime;
        }

        private void FixedUpdate()
        {
            var previousState = state;
            var self = transform;

            state = controller.DetectionProgress >= 1.0f ? State.Investigate : State.Patrol;

            if (stateText)
                stateText.transform.position = self.position + 0.2f * self.up;

            switch (state)
            {
                case State.Patrol:
                    transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
                    impactLineMaterial.SetFloat("_Strength", 0.0f);

                    // Instantiate patrol text
                    if (previousState == State.Investigate)
                        SetText("?");

                    break;

                case State.Investigate:
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

                    // Instantiate alert text
                    if (previousState == State.Patrol)
                        SetText("!", float.MaxValue);

                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }

        public bool checkIfDead() {
            dead = false;
            currentEulerAngles = transform.eulerAngles;
            if ((Mathf.Abs(currentEulerAngles.z) > 85 && Mathf.Abs(currentEulerAngles.z) < 95) || (Mathf.Abs(currentEulerAngles.z) > 265 && Mathf.Abs(currentEulerAngles.z) < 275))
            {
                dead = true;
                Destroy(gameObject);
            }
            return dead;
        }
    }
}
