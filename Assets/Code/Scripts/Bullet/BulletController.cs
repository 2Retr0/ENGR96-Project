using Code.Scripts.Enemy;
using UnityEngine;

namespace Code.Scripts.Bullet
{
    public class BulletController : MonoBehaviour
    {
        [SerializeField] private float lifetimeSeconds = 5;
        [SerializeField] private float speed = 50; // In units per second

        private Rigidbody rigidBody;

        // Start is called before the first frame update
        private void Start()
        {
            rigidBody = GetComponent<Rigidbody>();

            Destroy(gameObject, lifetimeSeconds);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                collision.gameObject.GetComponent<EnemyBehaviorController>().TakeDamage(1);
            }
            Destroy(gameObject);
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            var t = transform;
            var velocity = speed * t.forward;
            rigidBody.MovePosition(t.position + velocity * Time.deltaTime);

        }
    }
}
