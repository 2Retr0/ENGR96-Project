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
        private bool shouldMove = true;
        
        
        private bool isAlive;
        private int health;

        public Animator animator;

        private enum State
        {
            Patrol, Investigate
        }

        // Start is called before the first frame update
        private void Start()
        {
            isAlive = true;
            health = 100;
            
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

        private void SetText(string text, float lifetime = 3)
        {
            Destroy(stateText);
            stateText = Instantiate(textMesh, transform.position, Quaternion.identity);
            stateText.GetComponent<TextMeshPro>().text = text;
            stateText.GetComponent<TextController>().lifetimeSeconds = lifetime;
        }

        private void OnCollisionEnter(Collision other)
        {
            if(other.transform.CompareTag(player.tag))
                shouldMove = false;
        }

        private void OnCollisionExit(Collision other)
        {
            shouldMove = true;
        }

        private void FixedUpdate()
        {
            var previousState = state;
            var self = transform;

            state = controller.DetectionProgress >= 1.0f || !shouldMove ? State.Investigate : State.Patrol;

            if (stateText)
                stateText.transform.position = self.position + 0.2f * self.up;

            switch (state)
            {
                case State.Patrol:
                    transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
                    impactLineMaterial.SetFloat("_Strength", 0.0f);
                    animator.SetFloat("Speed", 5f);

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

                    // Move if not colliding with player!
                    if (shouldMove)
                    {
                        transform.position += speed * Time.deltaTime * lookAt;
                        animator.SetFloat("Speed", 10f);
                    }
                    else
                    {
                        animator.SetFloat("Speed", 0f);
                    }

                    // Instantiate alert text
                    if (previousState == State.Patrol)
                        SetText("!", float.MaxValue);

                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void TakeDamage(int damage)
        {
            health += -damage;
            if (health < 1)
            {
                isAlive = false;
            }
            DestroyIfDead();
        }

        private void DestroyIfDead()
        {
            if (!isAlive)
            {
                Destroy(gameObject);
            }
        }
    }
}
