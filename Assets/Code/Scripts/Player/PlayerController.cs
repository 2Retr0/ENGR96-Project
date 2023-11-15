using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed = 0;
    private float movementX;
    private float movementY;
    public Animator animator;
    public GameObject gunInBack;
    public GameObject gunInHand;
    private bool aim = false;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        gunInBack.SetActive(true);
        gunInHand.SetActive(false);
        rb = GetComponent <Rigidbody>(); 
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        rb.MovePosition(transform.position + movement * movementSpeed * Time.deltaTime * 5.0f);
        OnRotation();

        // Animation
        if (movement == Vector3.zero)
        {
            // Idle
            movementSpeed = 0;
            animator.SetFloat("Speed", 0);
        }
        else if (!Input.GetKey(KeyCode.LeftShift))
        {
            // Walk
            movementSpeed = 5;
            
            animator.SetFloat("Speed", 5);
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            // Run
            movementSpeed = 10;
            animator.SetFloat("Speed", 10);
        }

        if (Input.GetKey(KeyCode.Mouse1) && !aim)
        {
            // RifleAim
            aim = true;
            gunInBack.SetActive(false);
            gunInHand.SetActive(true);
            animator.SetBool("Aim", true);

            // Fire
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                animator.SetBool("Fire", true);
            }

            // no Fire
            if (!Input.GetKeyDown(KeyCode.Mouse0))
            {
                animator.SetBool("Fire", false);
            }
        }
        else if (Input.GetKey(KeyCode.Mouse1))
        {
            // exit RifleAim
            aim = false;
            gunInBack.SetActive(true);
            gunInHand.SetActive(false);
            animator.SetBool("Aim", false);
        }
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x; 
        movementY = movementVector.y;
    }

    void OnRotation()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
        }
    }
}
