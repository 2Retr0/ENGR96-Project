using System;
using System.Collections.Generic;
using System.Reflection;
using Code.Scripts.Player;
using Code.Scripts.Text;
using Cyan;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;
using Random = UnityEngine.Random;

namespace Code.Scripts.Enemy
{
    public class EnemyBehaviorController : MonoBehaviour
    {
        [SerializeField] private GameObject textMesh;
        [SerializeField] public GameObject player;
        [SerializeField] public float speed = 5f;

        public float rotationSpeed;

        // --- State Tracking Fields ---
        private State state = State.Patrol;
        private State previousState = State.Patrol;
        private bool didStateJustChange;
        private float lastStateChangeTime;
        private GameObject stateText;

        // --- Patrol State Fields ---
        private Vector3 lastSeenPlayerPosition;
        private float nextLookTime;
        private float lookAngle;

        // --- Investigate State Fields ---
        private float nextDashTime;
        private float visionArcAngle;
        private float visionRange;

        private VisionConeController controller;
        private Material impactLineMaterial;
        private bool isTouchingPlayer;


        private bool isAlive;
        private int health;

        // public Animator animator;

        private enum State
        {
            Patrol, Investigate, CatchUp, Sussed, Dash
        }

        // Start is called before the first frame update
        private void Start()
        {
            isAlive = true;
            health = 100;

            controller = GetComponentInChildren<VisionConeController>();

            if (!player) player = FindObjectOfType<PlayerController>().gameObject;

            lastSeenPlayerPosition = transform.position;
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
            UpdateState(transform);
        }

        private void Update()
        {
            UpdateMovement(transform);
            UpdateText(transform);
        }

        private void UpdateState(Transform self)
        {
            var currentState = state;
            switch (state)
            {
                case State.Patrol:
                    var t = Time.fixedTime - lastStateChangeTime;
                    // Get next look angle
                    if (Time.fixedTime >= nextLookTime)
                    {
                        // Look around more often right after losing track of a player
                        var spanFactor = Mathf.SmoothStep(0.2f, 1f, t / 6f);
                        nextLookTime += Random.Range(2f, 4f) * spanFactor;
                        lookAngle += Random.Range(-100f, 100f) * spanFactor;
                    }

                    if (controller.CanSeePlayer)
                        state = State.Sussed;
                    break;

                case State.Investigate:
                    var strength = Mathf.SmoothStep(1f, 0f, Vector3.Distance(self.position, player.transform.position) * 0.1f);
                    PostManager.Instance.SetImpactStrength(strength, gameObject);

                    if (Time.fixedTime >= nextDashTime)
                    {
                        state = State.Dash;
                        visionArcAngle = controller.arcAngle;
                        visionRange = controller.range;
                    }
                    else if (!controller.CanSeePlayer && !isTouchingPlayer)
                        state = State.CatchUp;
                    break;

                case State.CatchUp:
                    if (Vector3.Distance(self.position, lastSeenPlayerPosition) < 0.05f * speed)
                        state = State.Patrol;

                    PostManager.Instance.SetImpactStrength(0, gameObject);
                    break;

                case State.Sussed:
                    if (controller.DetectionProgress >= 1.0f)
                        state = State.Investigate;
                    else if (controller.DetectionProgress < 0.5f && !controller.CanSeePlayer)
                        state = State.Patrol;
                    break;

                case State.Dash:
                    if (Time.fixedTime - lastStateChangeTime > 2.25f)
                        state = State.Investigate;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            didStateJustChange = currentState != state && !didStateJustChange;
            if (didStateJustChange)
            {
                previousState = currentState;
                lastStateChangeTime = Time.fixedTime;

                lookAngle = Vector3.SignedAngle(new Vector3(0, 0, 1), transform.forward, Vector3.up);
                nextLookTime = Time.fixedTime + 0.35f;
                nextDashTime = Time.fixedTime + Random.Range(2f, 3f);
            }
        }


        private void UpdateText(Transform self)
        {
            if (stateText)
                stateText.transform.position = self.position + 0.5f * self.up;

            if (!didStateJustChange) return;

            switch (state)
            {
                case State.Patrol:
                    if (previousState is State.CatchUp or State.Investigate)
                        SetText("?");
                    break;
                case State.Investigate:
                    SetText("!", float.MaxValue);
                    break;
                case State.Sussed:
                    SetText("?");
                    break;
                case State.CatchUp:
                    break;
                case State.Dash:
                    SetText("!!", 2f);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdateMovement(Transform self)
        {
            var t = Time.fixedTime - lastStateChangeTime;
            Vector3 lookAt;
            var playerPosition = player.transform.position;

            switch (state)
            {
                case State.Patrol:
                    // Look randomly (faster right after losing track of a player)
                    var rotationSpeedModifier = Mathf.SmoothStep(0.4f, 0.1f, t / 6f);
                    var rotationAmount = rotationSpeed * rotationSpeedModifier * Time.deltaTime;
                    transform.rotation = Quaternion.Slerp(
                        self.rotation, Quaternion.AngleAxis(lookAngle, Vector3.up), rotationAmount);
                    break;

                case State.Investigate:
                    MoveTowards(playerPosition, transform);
                    lastSeenPlayerPosition = playerPosition;

                    // animator.SetFloat("Speed", isTouchingPlayer ? 0 : 10);
                    break;

                case State.CatchUp:
                    // Move towards last seen position, slowing down to 0.3*speed over 1 second
                    MoveTowards(lastSeenPlayerPosition, transform, Mathf.SmoothStep(1f, 0.5f, t / 3f));
                    break;

                case State.Sussed:
                    lookAt = LookAt(playerPosition, self);

                    // Slowly look towards player
                    transform.rotation = Quaternion.Slerp(
                        self.rotation, Quaternion.LookRotation(lookAt), 1.5f * Time.deltaTime);
                    break;

                case State.Dash:
                    var _ = 0.0f;
                    // var strength = Mathf.SmoothStep(1f, 0f, Vector3.Distance(self.position, player.transform.position) * 0.1f);
                    // PostManager.Instance.SetImpactStrength(strength, gameObject);

                    controller.DetectionProgress = 1.1f;
                    // print(controller.arcAngle);
                    if (t < 0.8)
                    {
                        controller.SetArcAngle(Mathf.SmoothDamp(controller.arcAngle, 15f, ref _, 8.0f * Time.deltaTime));
                    }
                    else if (t is > 0.8f and < 0.9f)
                    {
                        Destroy(stateText);
                        transform.position += 100f * Time.deltaTime * self.forward;
                        controller.SetRange(0.1f);
                        // transform.position = Vector3.SmoothDamp(self.position, )
                    }
                    else if (t < 1.5f)
                        controller.SetRange(0.1f);
                    else if (t > 1.5f)
                    {
                        controller.SetRange(Mathf.SmoothDamp(controller.range, visionRange, ref _, 10.0f * Time.deltaTime));
                        controller.SetArcAngle(Mathf.SmoothDamp(controller.arcAngle, visionArcAngle, ref _, 10.0f * Time.deltaTime));


                        // Begin running towards player if there is line of sight
                        if (Physics.Raycast(self.position, playerPosition - self.position, out var hit, 15f) &&
                            hit.transform == player.transform)
                        {
                            lookAt = LookAt(playerPosition, self);
                            transform.rotation = Quaternion.Slerp(
                                self.rotation, Quaternion.LookRotation(lookAt), 3.0f * Time.deltaTime);
                            MoveTowards(self.position + self.forward, self, 0.5f);
                        }
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static Vector3 LookAt(Vector3 targetPosition, Transform self)
        {
            var position = self.position;
            var lookAt = targetPosition - position;
            lookAt.y = 0;
            lookAt.Normalize();

            return lookAt;
        }

        private void MoveTowards(Vector3 targetPosition, Transform self, float speedModifier = 1.0f)
        {
            var lookAt = LookAt(targetPosition, self);
            transform.rotation = Quaternion.RotateTowards(
                self.rotation, Quaternion.LookRotation(lookAt), rotationSpeed * Time.deltaTime);

            var strength = Mathf.SmoothStep(1f, 0f, Vector3.Distance(self.position, targetPosition) * 0.1f);
            PostManager.Instance.SetImpactStrength(strength, gameObject);

            // Move if not colliding with player!
            if (!isTouchingPlayer)
                transform.position += speed * speedModifier * Time.deltaTime * lookAt;
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
