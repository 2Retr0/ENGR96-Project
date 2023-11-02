using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyBehavior : MonoBehaviour
{
    private const int IgnoreLayerMask = ~(1 << 2);
    private const float ArcLengthDelta = 0.2f;

    [SerializeField] private float range = 5f;
    [SerializeField] private float arcAngle = 45f;

    private EnemyMovementController moveController;
    private float rayCount;
    private float arcAngleDelta;

    // Start is called before the first frame update
    void Start()
    {
        moveController = GetComponent<EnemyMovementController>();

        rayCount = (int) (range * (arcAngle * Mathf.Deg2Rad) / ArcLengthDelta);
        arcAngleDelta = arcAngle / rayCount;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CastRays(transform);
    }

    void CastRays(Transform t)
    {
        var castDirection = Quaternion.Euler(0, t.rotation.y - (arcAngle * 0.5f), 0) * t.forward;

        for (var i = 0; i < rayCount; ++i)
        {
            castDirection = Quaternion.Euler(0, arcAngleDelta, 0) * castDirection;

            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(t.position, castDirection, out var hit, range, IgnoreLayerMask))
                Debug.DrawRay(t.position, castDirection * hit.distance, Color.green);
            else
                Debug.DrawRay(t.position, castDirection * range, Color.red);
        }
    }
}
