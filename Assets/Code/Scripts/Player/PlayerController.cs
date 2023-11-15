using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float walkSpeed = 5; // In units per second
        [SerializeField] private float runSpeed = 10; // In units per second
        [SerializeField] private Camera playerCamera;
        [SerializeField] private GameObject bullet;

        private float velocity = 0;
        private Vector3 displacement;
        public Animator animator;
        public GameObject gunInBack;
        public GameObject gunInHand;
        private bool isAiming = false;
        private Vector3 lookAt = Vector3.zero;
        private Rigidbody rb;
        // Start is called before the first frame update
        void Start()
        {
            displacement = Vector3.zero;
            gunInBack.SetActive(true);
            gunInHand.SetActive(false);
            rb = GetComponent <Rigidbody>();
            if(!playerCamera) playerCamera = Camera.main;
        }

        void Update()
        {
            var t = transform;

            // Animation
            if (displacement == Vector3.zero)
            {
                // Idle
                velocity = 0;
                animator.SetFloat("Speed", 0);
            }
            else if (!Input.GetKey(KeyCode.LeftShift))
            {
                // Walk
                velocity = walkSpeed;
                animator.SetFloat("Speed", walkSpeed);
            }
            else if (Input.GetKey(KeyCode.LeftShift))
            {
                // Run
                velocity = runSpeed;
                animator.SetFloat("Speed", runSpeed);
            }

            switch (isAiming)
            {
                case false when Input.GetKeyDown(KeyCode.Mouse1):
                    // RifleAim
                    isAiming = true;
                    gunInBack.SetActive(false);
                    gunInHand.SetActive(true);
                    animator.SetBool("Aim", true);
                    break;

                case true when Input.GetKeyUp(KeyCode.Mouse1):
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
                    break;
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            rb.MovePosition(transform.position + velocity * Time.deltaTime * displacement);
            UpdateModelRotation();
        }

        private void OnMove(InputValue input)
        {
            var planeDisplacement = input.Get<Vector2>(); // Converts WASD to normalized vec2
            // Cursed input rotation.
            displacement = Quaternion.AngleAxis(-45, Vector3.up) * (-1 * new Vector3(planeDisplacement.x, 0.0f, planeDisplacement.y));
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
