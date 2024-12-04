using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPanel_UpDown : MonoBehaviour
{
    public float speed = 1f; // Speed of the movement
    public float maxHeight = 10f; // Maximum height
    public float pauseDuration = 1f; // Duration to pause at each extreme position

    private Vector3 startPos;
    private bool movingUp = true;

    private void Start()
    {
        startPos = transform.position;
    }

    private void FixedUpdate()
    {
        // Calculate the vertical movement based on the current direction
        float verticalMovement = speed * Time.deltaTime * (movingUp ? 1f : -1f);

        // Calculate the new position
        Vector3 newPosition = transform.position + Vector3.up * verticalMovement;

        // Move the object to the new position
        transform.position = newPosition;

        // Check if the object has reached the maximum height or returned to the ground
        if (movingUp && transform.position.y >= startPos.y + maxHeight)
        {
            movingUp = false;
            Invoke("ChangeDirection", pauseDuration);
        }
        else if (!movingUp && transform.position.y <= startPos.y)
        {
            movingUp = true;
            Invoke("ChangeDirection", pauseDuration);
        }
    }

    private void ChangeDirection()
    {
        // Empty method used as a callback to change the movement direction
    }

    private void OnTriggerEnter(Collider other)
    {
        other.transform.SetParent(transform);
    }

    private void OnTriggerExit(Collider other)
    {
        other.transform.SetParent(null);
    }
}
