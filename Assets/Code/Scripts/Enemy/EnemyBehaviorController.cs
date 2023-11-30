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
        private bool isTouchingPlayer = false;
        private bool isMoving = false;
        
        private bool isAlive;
        private int health;

        // public Animator animator;

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
            if (stateText) Destroy(stateText);
            stateText = Instantiate(textMesh, transform.position, Quaternion.identity);
            stateText.GetComponent<TextMeshPro>().text = text;
            stateText.GetComponent<TextController>().lifetimeSeconds = lifetime;
        }

        private void OnCollisionEnter(Collision other)
        {
            if(other.transform.CompareTag(player.tag))
                isTouchingPlayer = true;
        }

        private void OnCollisionExit(Collision other)
        {
            isTouchingPlayer = false;
        }

        private void FixedUpdate()
        {
            var previousState = state;

            state = controller.DetectionProgress >= 1.0f || isTouchingPlayer ? State.Investigate : State.Patrol;

            UpdateMovement(transform);
            UpdateText(previousState, transform);
        }


        private void UpdateText(State previousState, Transform self)
        {
            if (stateText)
                stateText.transform.position = self.position + 0.5f * self.up;

            switch (state)
            {
                case State.Patrol:
                    if (previousState == State.Investigate)
                        SetText("?");
                    break;
                case State.Investigate:
                    if (previousState == State.Patrol)
                        SetText("!", float.MaxValue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdateMovement(Transform self)
        {
            switch (state)
            {
                case State.Patrol:
                    transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
                    PostManager.Instance.SetImpactStrength(0, gameObject);

                    break;

                case State.Investigate:
                    // --- Move towards player ---
                    var position = self.position;
                    var playerPosition = player.transform.position;
                    var lookAt = playerPosition - position;
                    lookAt.y = 0;
                    lookAt.Normalize();

                    transform.rotation = Quaternion.RotateTowards(
                        self.rotation, Quaternion.LookRotation(lookAt), rotationSpeed * Time.deltaTime);

                    var strength = Mathf.SmoothStep(1f, 0f, Vector3.Distance(position, playerPosition) * 0.1f);
                    PostManager.Instance.SetImpactStrength(strength, gameObject);

                    // Move if not colliding with player!
                    if (!isTouchingPlayer)
                        transform.position += speed * Time.deltaTime * lookAt;
                    // animator.SetFloat("Speed", isTouchingPlayer ? 0 : 10);
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
                OnKill();
            }
        }

        private void OnKill()
        {
            Destroy(stateText);
            Destroy(gameObject);
        }
    }
}
