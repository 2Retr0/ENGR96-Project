using UnityEngine;

namespace Code.Scripts.Player
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 targetOffset = new(-10, 10, 10);
        // public float movementSpeed;
        [SerializeField] private float smoothTime = 30f;

        private float originalSmoothTime;
        private Camera camera;
        private float originalOrthographicSize;

        // Start is called before the first frame update
        private void Start()
        {
            camera = GetComponent<Camera>();
            originalOrthographicSize = camera.orthographicSize;
            originalSmoothTime = smoothTime;
            if (!target) target = FindObjectOfType<PlayerController>().gameObject.transform;
        }

        // Update is called once per frame
        private void Update()
        {
            MoveCamera();
        }

        public void Reset()
        {
            smoothTime = originalSmoothTime;
            camera.orthographicSize = originalOrthographicSize;
        }

        public void Zoom(float limit, float delta)
        {
            if (camera.orthographicSize > limit)
            {
                camera.orthographicSize += delta;
                targetOffset.y -= delta * 0.35f;
                smoothTime = originalSmoothTime * Time.timeScale;
            }
        }

        private void MoveCamera()
        {
            var _ = Vector3.zero;
            transform.position = Vector3.SmoothDamp(transform.position, target.position + targetOffset, ref _, smoothTime);
        }
    }
}
