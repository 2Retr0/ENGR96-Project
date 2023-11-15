using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Vector3 tartgetOffset;
    // public float movementSpeed;
    public float smoothTime = 0.3f;
    public float maxSpeed = 10f; // Optionally add a max speed
    private Vector3 velocity = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveCamera();
    }

    void MoveCamera()
    {
        // transform.position = Vector3.Lerp(transform.position, target.position + tartgetOffset, movementSpeed * Time.deltaTime);
        transform.position = Vector3.SmoothDamp(transform.position, target.position + tartgetOffset, ref velocity, smoothTime, maxSpeed);
    }
}
