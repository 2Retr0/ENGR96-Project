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

        [SerializeField] private Button playButton;
        [SerializeField] private Button quitButton;

        private float speed = 0;
        private Vector3 displacement;
        public Animator animator;
        public GameObject gunInBack;
        public GameObject gunInHand;
        private bool isAiming = false;
        private Vector3 lookAt = Vector3.zero;
        private Rigidbody rb;

        public TextMeshProUGUI healthText;
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI levelText;
        public TextMeshProUGUI pausedText;
        public TextMeshProUGUI gameOverText;

        private int health;
        private int score;
        private int level;
        private float levelConstant;

        public UnityEvent onLevelUp;

        private bool isGamePaused;
        private int contactEnableCounter;

        // Start is called before the first frame update
        private void Start()
        {
            Time.timeScale = 1.0f;

            displacement = Vector3.zero;
            gunInBack.SetActive(true);
            gunInHand.SetActive(false);
            rb = GetComponent<Rigidbody>();
            if(!playerCamera) playerCamera = Camera.main;

            health = 100;
            score = 0;
            level = 1;
            levelConstant = 0.05f;

            healthText.text = "Health: " + health.ToString();
            scoreText.text = "Score: " + score.ToString();
            levelText.text = "Level: " + level.ToString();
            pausedText.text = " ";
            gameOverText.text = " ";

            isGamePaused = false;
            contactEnableCounter = 0;

            playButton.gameObject.SetActive(false);
            quitButton.gameObject.SetActive(false);
        }

        private void Update()
        {
            var t = transform;

            // Animation
            if (displacement == Vector3.zero)
            {
                // Idle
                speed = 0;
                animator.SetFloat("Speed", 0);
            }
            else if (!Input.GetKey(KeyCode.LeftShift))
            {
                // Walk
                speed = walkSpeed;
                animator.SetFloat("Speed", walkSpeed);
            }
            else if (Input.GetKey(KeyCode.LeftShift))
            {
                // Run
                speed = runSpeed;
                animator.SetFloat("Speed", runSpeed);
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
                        animator.SetBool("Aim", true);
                    }
                    break;

                case true when Input.GetButtonUp("Fire2"):
                    // exit RifleAim
                    isAiming = false;
                    gunInBack.SetActive(true);
                    gunInHand.SetActive(false);
                    animator.SetBool("Aim", false);
                    break;

                case true when Input.GetButtonDown("Fire1"):
                    // Spawn bullet at player position with some forward and y-offset
                    var spawnPosition = t.position + 1.25f * t.forward + 1.65f * t.up;
                    if (!isGamePaused)
                    {
                        Instantiate(bullet, spawnPosition, t.rotation);
                    }

                    // Trigger fire animation
                    animator.SetBool("Fire", true);
                    // Start coroutine to reset fire animation
                    StartCoroutine(ResetFireAnimation());
                    break;
            }


        
        }

        private IEnumerator ResetFireAnimation()
        {
            // Wait for one second
            yield return new WaitForSeconds(0.1f);

            // Reset the fire animation
            animator.SetBool("Fire", false);
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            transform.position += speed * Time.deltaTime * displacement;
            // rb.MovePosition(transform.position + speed * Time.deltaTime * displacement);
            UpdateModelRotation();

            contactEnableCounter++;

            if (contactEnableCounter < 50)
            {
                Debug.Log("No Damage mode");

            }
            else {
                Debug.Log("Damage mode");
            }
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
            if (!Physics.Raycast(playerCamera.ScreenPointToRay(Input.mousePosition), out var hit))
                return;

            lookAt = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            transform.LookAt(lookAt);
        }

        public void IncreaseScore(int scoreIncrease)
        {
            score += scoreIncrease;
            // Update the count text with the current count.
            scoreText.text = "Score: " + score.ToString();
            IncreaseLevel();
        }

        private void IncreaseLevel()
        {
            int checkLevel = Mathf.FloorToInt(levelConstant * Mathf.Sqrt(score) + 1);
            if (level != checkLevel) { 
                levelText.text = "Level: " + checkLevel.ToString();
                onLevelUp?.Invoke();
                Debug.Log("LEVELD UP");
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

        private void OnCollisionEnter(Collision col) {
            if (col.gameObject.tag == "Enemy" && contactEnableCounter > 50) {
                TakeDamage(30);
            }
            contactEnableCounter = 0;
        }

        private void TakeDamage(int i) {
            health += -i;
            healthText.text = "Health: " + health.ToString();
            CheckGameOver();
        }

        private void CheckGameOver() {
            if (health < 1) {
                int h = 0;
                healthText.text = "Health: " + h.ToString();
                gameOverText.text = "GAME OVER";
                Time.timeScale = 0.2f;
                playButton.gameObject.SetActive(true);
                quitButton.gameObject.SetActive(true);
            }
        }
    }
}
