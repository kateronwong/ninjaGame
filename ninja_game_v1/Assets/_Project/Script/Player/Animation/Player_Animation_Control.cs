using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Animation_Control : MonoBehaviour
{
    private Animator animator;

    [SerializeField]
    private Rigidbody playerRigidbody;

    private bool isMoving = false;

    private Player_Movement pm;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        pm = GetComponentInParent<Player_Movement>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get the horizontal and vertical input values (e.g., from Input.GetAxis)
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (playerRigidbody.linearVelocity.magnitude >=2)
        {
            if (horizontalInput != 0 || verticalInput != 0)
            {
                if (!pm.freeze)
                {
                    isMoving = true;
                }

            }
            else isMoving = false;
        }
        else isMoving = false;


        // Calculate the player's velocity based on the input and transform.up
        Vector3 velocity = (horizontalInput * transform.right + verticalInput * transform.up).normalized;

        // Set the horizontal and vertical velocities in the animator
        float horizontalVelocity = Vector3.Dot(velocity, transform.right);
        float verticalVelocity = Vector3.Dot(velocity, transform.up);

        animator.SetBool("isMoving", isMoving);
        animator.SetFloat("VelocityHorizontal", horizontalVelocity);
        animator.SetFloat("VelocityVertical", verticalVelocity);
    }

    public void EnableHitbox()
    {
        GetComponentInChildren<Sword_Attack>().EnableHitbox();
    }

    public void DisableHitbox()
    {
        GetComponentInChildren<Sword_Attack>().DisableHitbox();
    }
}
