using System;
using Code.Scripts.Player;
using Code.Scripts.Text;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

namespace Code.Scripts.Enemy
{
    public class EnemyBehaviorController : MonoBehaviour
    {
        [SerializeField] private GameObject textMesh;
        [SerializeField] public GameObject player;
        [SerializeField] public float speed = 5f;
        [SerializeField] private GameObject bullet;

        [SerializeField] private AudioClip detectSound;
        [SerializeField] private AudioClip dashTelegraphSound;
        [SerializeField] private AudioClip dashSound;

        public float rotationSpeed;

        // --- State Tracking Fields ---
        private State state = State.Rest;
        private State previousState = State.Rest;
        private bool didStateJustChange;
        private float lastStateChangeTime;
        private GameObject stateText;

        // --- Patrol State Fields ---
        private Vector3 lastSeenPlayerPosition;
        private float nextLookTime;
        private float lookAngle;
        private bool isPatrolling; // is walking
        private float nextWalkTime;

        // --- Investigate State Fields ---
        private float nextDashTime;
        private float visionArcAngle;
        private float visionRange;
        private readonly float patrolMovementDuration = 2f;
        private float patrolMovementTimer;

        // --- Dash State Fields ---
        private const int PlayerLayerMask = ~((1 << 2) | (1 << 3));
        private const float TargetDashDistance = 10f;
        private Vector3 startDashPosition;
        private float dashDistance;
        private bool hasDashed; // FIXME: AWFUL SOLUTION OMG

        private VisionConeController controller;
        private Material impactLineMaterial;
        private bool isTouchingPlayer;

        
        private bool isAlive;
        private int health;

        private float bulletTime;

        public Animator animator;
        private static readonly int Dash = Animator.StringToHash("Dash");
        private static readonly int Speed = Animator.StringToHash("Speed");
        private bool wasShotAt;

        public EnemyBehaviorController(bool isAlive)
        {
            this.isAlive = isAlive;
        }

        private enum State
        {
            Rest, Chase, CatchUp, Sussed, Dash, Patrol
        }

        // Start is called before the first frame update
        private void Start()
        {
            isAlive = true;
            health = 100;

            bulletTime = 1.5f;

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
        
        private void FireGun(Transform t)
        {
            // Spawn bullet at player position with some forward and y-offset
            
            var spawnPosition = t.position + 1.25f * t.forward + 1.65f * t.up;
            Instantiate(bullet, spawnPosition, t.rotation);
        }

        private void OnCollisionEnter(Collision other)
        {
            if(other.transform.CompareTag(player.tag))
                isTouchingPlayer = true;
        }

        private void OnCollisionExit()
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

            bulletTime -= Time.deltaTime;
            
            var t = transform;
            if (state == State.Chase && bulletTime < 0)
            {
                FireGun(t);
                bulletTime = 1.5f;
                Debug.Log("Fired");
            }


        }

        private void UpdateState(Transform self)
        {
            var currentState = state;
            switch (state)
            {
                case State.Patrol:
                    PostManager.Instance.SetImpactStrength(0, gameObject);

                    if (controller.CanSeePlayer)
                        state = State.Sussed;
                    else if (Time.fixedTime >= nextWalkTime)
                        state = State.Rest;
                    break;

                case State.Rest:
                    var t = Time.fixedTime - lastStateChangeTime;

                    if (Time.fixedTime >= nextLookTime)
                    {
                        // Look around more often right after losing track of a player
                        var spanFactor = Mathf.SmoothStep(0.2f, 1f, t / 6f);
                        nextLookTime += Random.Range(2f, 4f) * spanFactor;
                        lookAngle += Random.Range(-100f, 100f) * spanFactor;
                    }

                    // Only switch to walk state if finished rotating.
                    if (Time.fixedTime >= nextWalkTime)
                        state = State.Patrol;
                    else if (controller.CanSeePlayer)
                        state = State.Sussed;
                    break;

                case State.Chase:
                    var strength = Mathf.SmoothStep(1f, 0f, Vector3.Distance(self.position, player.transform.position) * 0.1f);
                    PostManager.Instance.SetImpactStrength(strength, gameObject);

                    if (!controller.CanSeePlayer && !isTouchingPlayer)
                        state = State.CatchUp;
                    else if (Time.fixedTime >= nextDashTime)
                    {
                        state = State.Dash;
                        AudioSource.PlayClipAtPoint(dashTelegraphSound, transform.position, 0.5f);
                        visionArcAngle = controller.arcAngle;
                        visionRange = controller.range;
                        startDashPosition = self.position;
                        hasDashed = false;
                        dashDistance = Physics.Raycast(startDashPosition, player.transform.position - startDashPosition, out var hit,
                            TargetDashDistance, PlayerLayerMask) ? hit.distance : TargetDashDistance;
                    }
                    break;

                case State.CatchUp:
                    PostManager.Instance.SetImpactStrength(0, gameObject);

                    if (controller.CanSeePlayer)
                        state = State.Chase;
                    else if (Vector3.Distance(self.position, lastSeenPlayerPosition) < 0.05f * speed ||
                        Time.fixedTime - lastStateChangeTime > 5.0f)
                    {
                        state = State.Rest;
                    }
                    break;

                case State.Sussed:
                    if (controller.DetectionProgress >= 1.0f)
                    {
                        state = State.Chase;
                        AudioSource.PlayClipAtPoint(detectSound, transform.position, 1.0f);
                    }
                    else if (controller.DetectionProgress < 0.5f && !controller.CanSeePlayer)
                        state = State.Rest;
                    break;

                case State.Dash:
                    if (Time.fixedTime - lastStateChangeTime > 2.25f)
                    {
                        state = State.Chase;
                        animator.SetBool(Dash, false);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            didStateJustChange = currentState != state && !didStateJustChange;
            if (!didStateJustChange) return;

            OnStateChanged(currentState);
        }

        private void OnStateChanged(State oldState)
        {
            lookAngle = Vector3.SignedAngle(new Vector3(0, 0, 1), transform.forward, Vector3.up);
            nextWalkTime += (previousState == State.Patrol) ? Random.Range(3f, 4f) : Random.Range(7f, 8f);
            nextLookTime = Time.fixedTime + 0.35f;
            nextDashTime = Time.fixedTime + Random.Range(2f, 3f);

            previousState = oldState;
            lastStateChangeTime = Time.fixedTime;
        }


        private void UpdateText(Transform self)
        {
            if (stateText)
                stateText.transform.position = self.position + 0.5f * self.up;

            if (!didStateJustChange) return;

            switch (state)
            {
                case State.Patrol:
                    break;
                case State.Rest:
                    if (previousState != State.Sussed && previousState != State.Patrol)
                        SetText("?");
                    break;
                case State.Chase:
                    if (previousState != State.CatchUp)
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
                    // Move in the look direction
                    MoveTowards(transform.position + Vector3.forward, transform, 0.45f);
                    animator.SetFloat(Speed, 5);
                    break;

                case State.Rest:
                    // Look randomly (faster right after losing track of a player)
                    var rotationSpeedModifier = Mathf.SmoothStep(0.4f, 0.1f, t / 6f);
                    animator.SetFloat(Speed, 0);
                    var rotationAmount = rotationSpeed * rotationSpeedModifier * Time.deltaTime;
                    transform.rotation = Quaternion.Slerp(
                        self.rotation, Quaternion.AngleAxis(lookAngle, Vector3.up), rotationAmount);
                    break;

                case State.Chase:
                    MoveTowards(playerPosition, transform);
                    lastSeenPlayerPosition = playerPosition;

                    animator.SetFloat(Speed, isTouchingPlayer ? 0 : 10);
                    break;

                case State.CatchUp:
                    // Keep vision cone on detected mode until enemy gives up
                    controller.DetectionProgress = 1.1f;

                    // Move towards last seen position, slowing down to 0.3*speed over 1 second
                    MoveTowards(lastSeenPlayerPosition, transform, Mathf.SmoothStep(1f, 0.5f, t / 3f));
                    break;

                case State.Sussed:
                    lookAt = LookAt(playerPosition, self);

                    animator.SetFloat(Speed, 0);
                    // Slowly look towards player
                    transform.rotation = Quaternion.Slerp(
                        self.rotation, Quaternion.LookRotation(lookAt), 1.5f * Time.deltaTime);
                    break;

                case State.Dash:
                    var _ = 0.0f;
                    // var strength = Mathf.SmoothStep(1f, 0f, Vector3.Distance(self.position, player.transform.position) * 0.1f);
                    // PostManager.Instance.SetImpactStrength(strength, gameObject);
                    const float targetDashTime = 0.1f;
                    const float dashStartTime = 0.8f;
                    // Proper dash time based on the amount of distance that can be travelled
                    var dashTime = dashDistance / TargetDashDistance * targetDashTime;

                    controller.DetectionProgress = 1.1f;
                    switch (t)
                    {
                        // print(controller.arcAngle);
                        case < dashStartTime:
                            controller.SetArcAngle(Mathf.SmoothDamp(controller.arcAngle, 12f, ref _, 8.0f * Time.deltaTime));
                            controller.SetRange(Mathf.SmoothDamp(controller.range, dashDistance, ref _, 8.0f * Time.deltaTime));
                            animator.SetBool(Dash, true);
                            break;

                        case >= dashStartTime when t < dashStartTime + dashTime:
                            // --- Dash Movement Logic ---
                            Destroy(stateText);
                            transform.position = Vector3.Lerp(startDashPosition, startDashPosition + dashDistance * self.forward,
                                (t - dashStartTime) * (1 / dashTime));
                            controller.SetRange(0.1f);
                            if (!hasDashed)
                                AudioSource.PlayClipAtPoint(dashSound, playerPosition, 1.0f);

                            hasDashed = true;
                            break;

                        case < dashStartTime + 0.4f:
                            controller.SetRange(0.1f);
                            break;

                        case >= dashStartTime + 0.4f:
                            animator.SetBool(Dash, false);
                            animator.SetFloat(Speed, 5);
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
                            break;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (wasShotAt && t < 1.0f)
            {
                lookAt = LookAt(playerPosition, self);
                transform.rotation = Quaternion.Slerp(
                    self.rotation, Quaternion.LookRotation(lookAt), 10f * Time.deltaTime);
            }
            else
            {
                wasShotAt = false;
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
            else if (state != State.Chase)
            {
                var originalState = state;
                state = State.Chase;
                AudioSource.PlayClipAtPoint(detectSound, transform.position, 1.0f);
                wasShotAt = true;
                OnStateChanged(originalState);
            }
        }

        private void OnKill()
        {
            try
            {
                player.GetComponent<PlayerController>().IncreaseScore(400);
            }
            catch
            {

            }
            Destroy(stateText);
            Destroy(gameObject);
            PostManager.Instance.SetImpactStrength(0, gameObject);
        }
    }
}
