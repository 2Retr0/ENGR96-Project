using UnityEngine;

namespace Code.Scripts.Player
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 tartgetOffset = new Vector3(-10, 10, 10);
        // public float movementSpeed;
        [SerializeField] private float smoothTime = 0.3f;
        [SerializeField] private float maxSpeed = 10f; // Optionally add a max speed
        private Vector3 velocity = Vector3.zero;
        // Start is called before the first frame update
        private void Start()
        {

        }

        // Update is called once per frame
        private void Update()
        {
            MoveCamera();
        }

        private void MoveCamera()
        {
            // transform.position = Vector3.Lerp(transform.position, target.position + tartgetOffset, movementSpeed * Time.deltaTime);
            transform.position = Vector3.SmoothDamp(transform.position, target.position + tartgetOffset, ref velocity, smoothTime, maxSpeed);
        }
    }
}
