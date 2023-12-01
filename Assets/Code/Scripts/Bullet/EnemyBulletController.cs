using System.Collections;
using System.Collections.Generic;
using Code.Scripts.Player;
using UnityEngine;

public class EnemyBulletController : MonoBehaviour
{

    [SerializeField] private float lifetimeSeconds = 7;
    [SerializeField] private float speed = 5; // In units per second

    private Rigidbody rigidBody;

    // Start is called before the first frame update
    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();

        Destroy(gameObject, lifetimeSeconds);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(20);
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
