using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Code.Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float walkSpeed = 5; // In units per second
        [SerializeField] private float runSpeed = 10; // In units per second
        [SerializeField] private Camera playerCamera;
        [SerializeField] private GameObject bullet;

        [SerializeField] private AudioClip fireSound;
        [SerializeField] private AudioClip equipSound;
        [SerializeField] private AudioClip concealSound;

        [SerializeField] private Button playButton;
        [SerializeField] private Button quitButton;

        public float speed;
        private Vector3 displacement;
        public Animator animator;
        public GameObject gunInBack;
        public GameObject gunInHand;
        private bool isAiming;
        private Vector3 lookAt = Vector3.zero;
        private CameraController cameraController;

        public TextMeshProUGUI healthText;
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI levelText;
        public TextMeshProUGUI pausedText;
        public TextMeshProUGUI gameOverText;

        [SerializeField] private int health = 100;
        [SerializeField] private int score = 0;
        [SerializeField] public int level = 1;
        private float levelConstant;

        public UnityEvent onLevelUp;

        private bool isGamePaused;
        private int contactEnableCounter;
        private int pickup_point;
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Aim = Animator.StringToHash("Aim");
        private static readonly int Fire = Animator.StringToHash("Fire");
        private static readonly int Death = Animator.StringToHash("OnDeath");
        public bool isDead;
        private MainAudioSource mainAudioSource;


        // Start is called before the first frame update
        private void Start()
        {
            Time.timeScale = 1.0f;

            mainAudioSource = GetComponentInChildren<MainAudioSource>();

            displacement = Vector3.zero;
            gunInBack.SetActive(true);
            gunInHand.SetActive(false);
            GetComponent<Rigidbody>();
            if(!playerCamera) playerCamera = Camera.main;
            if (playerCamera != null) cameraController = playerCamera.GetComponent<CameraController>();

            levelConstant = 0.05f;

            healthText.text = "Health: " + health;
            scoreText.text = "Score: " + score;
            levelText.text = "Level: " + level;
            pausedText.text = " ";
            gameOverText.text = " ";

            isGamePaused = false;
            contactEnableCounter = 0;
            pickup_point = 25; //how much point awarded to each pick up

            playButton.gameObject.SetActive(false);
            quitButton.gameObject.SetActive(false);
        }

        private void Update()
        {
            var self = transform;

            if (isDead)
            {
                if (PostManager.Instance.currentThickness < 100f)
                    PostManager.Instance.SetPlayerOutlineThickness(PostManager.Instance.currentThickness + 0.10f);
                cameraController.Zoom(3f, -0.01f);
                return;
            }

            transform.position += displacement * (speed * Time.deltaTime * 0.4f);
            // rb.MovePosition(transform.position + speed * Time.deltaTime * displacement);
            UpdateModelRotation();

            // Animation
            if (displacement == Vector3.zero)
            {
                // Idle
                speed = 0;
                animator.SetFloat(Speed, 0);
            }
            else if (!Input.GetKey(KeyCode.LeftShift))
            {
                // Walk
                speed = walkSpeed;
                animator.speed = walkSpeed * 0.135f;
                animator.SetFloat(Speed, walkSpeed);
            }
            else if (Input.GetKey(KeyCode.LeftShift))
            {
                // Run
                speed = runSpeed;
                animator.speed = runSpeed * 0.1f;
                animator.SetFloat(Speed, runSpeed);
            }

            switch (isAiming)
            {
                case false when Input.GetButtonDown("Fire2"):
                    // RifleAim
                    if (!isGamePaused)
                    {
                        isAiming = true;
                        gunInBack.SetActive(false);
                        gunInHand.SetActive(true);
                        animator.SetBool(Aim, true);
                        mainAudioSource.equipSource.Play();
                    }
                    break;

                case true when Input.GetButtonUp("Fire2"):
                    // exit RifleAim
                    isAiming = false;
                    gunInBack.SetActive(true);
                    gunInHand.SetActive(false);
                    animator.SetBool(Aim, false);
                    mainAudioSource.concealSource.Play();
                    break;

                case true when Input.GetButtonDown("Fire1"):
                    FireGun(self);
                    break;
            }
        }

        private void FireGun(Transform t)
        {
            // Spawn bullet at player position with some forward and y-offset
            var spawnPosition = t.position + 1.25f * t.forward + 1.65f * t.up;
            if (!isGamePaused)
            {
                Instantiate(bullet, spawnPosition, t.rotation);
            }

            // Trigger fire animation
            animator.SetBool(Fire, true);
            // Start coroutine to reset fire animation
            StartCoroutine(ResetFireAnimation());

            mainAudioSource.fireSource.PlayOneShot(fireSound, 0.5f);
            // AudioSource.PlayClipAtPoint(fireSound, playerCamera.transform.position, 0.25f);
        }

        private IEnumerator ResetFireAnimation()
        {
            // Wait for one second
            yield return new WaitForSeconds(0.1f);

            // Reset the fire animation
            animator.SetBool(Fire, false);
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            contactEnableCounter++;
        }

        private void OnMove(InputValue input)
        {
            var planeDisplacement = input.Get<Vector2>(); // Converts WASD to normalized vec2
            var unRotatedDisplacement = -1 * new Vector3(planeDisplacement.x, 0.0f, planeDisplacement.y).normalized;
            // Cursed input rotation.
            displacement = Quaternion.AngleAxis(-45, Vector3.up) * unRotatedDisplacement;
        }

        private void OnPause()
        {
            PauseGame();
        }

        private void UpdateModelRotation()
        {
            var ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            var yPlane = new Plane(Vector3.up, new Vector3(0, 1.75f, 0));

            // Cast ray from mouse position in screen space to xz plane in world space
            if (!yPlane.Raycast(ray, out var distance)) return; // Should never occur

            var hitPoint = ray.GetPoint(distance);
            lookAt = new Vector3(hitPoint.x, transform.position.y, hitPoint.z);
            transform.LookAt(lookAt);
        }

        void OnTriggerEnter(Collider other) //code for pick up
        {
            
            if (other.gameObject.CompareTag("PickUp")) 
            {
                other.gameObject.SetActive(false);

                IncreaseScore(pickup_point);
            }else if (other.gameObject.CompareTag("SpeedIncrease"))
            {
                walkSpeed += 1;
                runSpeed += 1;
                other.gameObject.SetActive(false);
            }
            
        }

        public void IncreaseScore(int scoreIncrease)
        {
            score += scoreIncrease;
            // Update the count text with the current count.
            scoreText.text = "Score: " + score;
            IncreaseLevel();
        }

        private void IncreaseLevel()
        {
            var checkLevel = Mathf.FloorToInt(levelConstant * Mathf.Sqrt(score) + 1);
            if (level != checkLevel) { 
                levelText.text = "Level: " + checkLevel;
                onLevelUp?.Invoke();
                level = checkLevel;
            }
        }

        private void PauseGame() {
            isGamePaused = !isGamePaused;
            if (isGamePaused)
            {
                Time.timeScale = 0f;
                pausedText.text = "Paused\r\npress [esc] again to resume";
            }
            else {
                Time.timeScale = 1f;
                pausedText.text = " ";
            }
        }

        private void OnCollisionStay(Collision col)
        {
            if (isDead) return;
            if (col.gameObject.CompareTag("Enemy") && contactEnableCounter > 50) {
                TakeDamage(30);
                contactEnableCounter = 0;
                mainAudioSource.damageSource.Play();
            }
        }

        public void TakeDamage(int i) {
            health += -i;

            if (healthText) healthText.text = "Health: " + health;
            if (health > 0) return;
            OnDeath();
        }

        private void OnDeath()
        {
            if (isDead) return;

            isDead = true;
            Time.timeScale = 0.2f;
            animator.speed = 1f / 0.2f;

            isAiming = false;
            speed = 0;
            gunInBack.SetActive(true);
            gunInHand.SetActive(false);
            animator.SetBool(Aim, false);
            animator.SetFloat(Speed, 0f);
            mainAudioSource.deathSource.Play();

            animator.SetTrigger(Death);

            if (gameOverText) gameOverText.text = "GAME OVER";
            if (playButton) playButton.gameObject.SetActive(true);
            if (quitButton) quitButton.gameObject.SetActive(true);
        }
    }
}
