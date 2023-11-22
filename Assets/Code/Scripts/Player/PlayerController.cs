using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

namespace Code.Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float walkSpeed = 5; // In units per second
        [SerializeField] private float runSpeed = 10; // In units per second
        [SerializeField] private Camera playerCamera;
        [SerializeField] private GameObject bullet;

        private float speed = 0;
        private Vector3 displacement;
        public Animator animator;
        public GameObject gunInBack;
        public GameObject gunInHand;
        private bool isAiming = false;
        private Vector3 lookAt = Vector3.zero;
        private Rigidbody rb;
        // Start is called before the first frame update
        private void Start()
        {
            displacement = Vector3.zero;
            gunInBack.SetActive(true);
            gunInHand.SetActive(false);
            rb = GetComponent<Rigidbody>();
            if(!playerCamera) playerCamera = Camera.main;
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
                    isAiming = true;
                    gunInBack.SetActive(false);
                    gunInHand.SetActive(true);
                    animator.SetBool("Aim", true);
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
                    Instantiate(bullet, spawnPosition, t.rotation);

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
            rb.MovePosition(transform.position + speed * Time.deltaTime * displacement);
            UpdateModelRotation();
        }

        private void OnMove(InputValue input)
        {
            var planeDisplacement = input.Get<Vector2>(); // Converts WASD to normalized vec2
            var unRotatedDisplacement = -1 * new Vector3(planeDisplacement.x, 0.0f, planeDisplacement.y).normalized;
            // Cursed input rotation.
            displacement = Quaternion.AngleAxis(-45, Vector3.up) * unRotatedDisplacement;
        }

        private void UpdateModelRotation()
        {
            if (!Physics.Raycast(playerCamera.ScreenPointToRay(Input.mousePosition), out var hit))
                return;

            lookAt = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            transform.LookAt(lookAt);
        }
    }
}
