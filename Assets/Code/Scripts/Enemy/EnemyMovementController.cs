using System;
using UnityEngine;

namespace Code.Scripts.Enemy
{
    public class EnemyMovementController : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float speed = 5f;
        [SerializeField] private GameObject player;

        private State state = State.Patrol;
        private VisionConeController controller;
        private Rigidbody rb;

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
        }

        private void FixedUpdate()
        {
            state = controller.DetectionProgress >= 1.0f ? State.Investigate : State.Patrol;

            switch (state)
            {
                case State.Patrol:
                    transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
                    break;

                case State.Investigate:
                    var self = transform;
                    var position = self.position;
                    var lookAt = player.transform.position - position;
                    lookAt.y = 0;
                    lookAt.Normalize();

                    transform.rotation = Quaternion.RotateTowards(
                        self.rotation, Quaternion.LookRotation(lookAt), rotationSpeed * Time.deltaTime);

                    rb.MovePosition(position + speed * Time.deltaTime * lookAt);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
