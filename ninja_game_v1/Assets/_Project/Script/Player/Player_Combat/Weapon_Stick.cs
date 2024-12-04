using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Stick : MonoBehaviour
{
    private Rigidbody rb;

    private bool targetHit;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (targetHit)
            return;
        else
            targetHit = true;

        // sticks to surface
        rb.isKinematic = true;

        // moves with target
        transform.SetParent(collision.transform);
    }
}
