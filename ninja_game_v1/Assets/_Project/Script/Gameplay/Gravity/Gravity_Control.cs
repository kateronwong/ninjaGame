using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody))]
public class Gravity_Control : MonoBehaviour
{
    /*
    public float GRAVITY_FORCE = 800;
    public float GravityRotateSpeed = 3f;

    private Rigidbody rb;
    private List<GravityArea> gravityAreas;

    // Start is called before the first frame update
    void Start()
    {
        rb = transform.GetComponent<Rigidbody>();
        gravityAreas = new List<GravityArea>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.AddForce(GravityDirection * (GRAVITY_FORCE * Time.fixedDeltaTime), ForceMode.Acceleration);

        Quaternion upRotation = Quaternion.FromToRotation(transform.up, -GravityDirection);
        Quaternion newRotation = Quaternion.Slerp(rb.rotation, upRotation * rb.rotation, Time.fixedDeltaTime * GravityRotateSpeed);;
        rb.MoveRotation(newRotation);
    }


    public Vector3 GravityDirection
    {
        get
        {
            if (gravityAreas.Count == 0) return Vector3.down;
            gravityAreas.Sort((area1, area2) => area1.Priority.CompareTo(area2.Priority));
            return gravityAreas.Last().GetGravityDirection(this).normalized;
        }
    }

    public void AddGravityArea(GravityArea gravityArea)
    {
        gravityAreas.Add(gravityArea);
    }

    public void RemoveGravityArea(GravityArea gravityArea)
    {
        gravityAreas.Remove(gravityArea);
    }
    */

    public float GRAVITY_FORCE = 800;
    public float GravityRotateSpeed = 3f;

    private Rigidbody rb;
    private List<GravityArea> gravityAreas;

    // Start is called before the first frame update
    void Start()
    {
        rb = transform.GetComponent<Rigidbody>();
        gravityAreas = new List<GravityArea>();
    }

    private void Update()
    {
        if (Player_Movement.GetOnWall())
        {
            GRAVITY_FORCE = 2000;
        }
        else
        {
            GRAVITY_FORCE = 800;
        }

        if (Player_Movement.GetonSlop())
        {
            GRAVITY_FORCE = 0;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {

        Vector3 gravityDirection = CalculateGravityDirection();

        rb.AddForce(gravityDirection * (GRAVITY_FORCE * Time.fixedDeltaTime), ForceMode.Acceleration);

        Quaternion upRotation = Quaternion.FromToRotation(transform.up, -gravityDirection);
        Quaternion newRotation = Quaternion.Slerp(rb.rotation, upRotation * rb.rotation, Time.fixedDeltaTime * GravityRotateSpeed);
        rb.MoveRotation(newRotation);
    }

    private Vector3 CalculateGravityDirection()
    {
        if (gravityAreas.Count == 0)
            return Vector3.down;

        Vector3 cumulativeGravityDirection = Vector3.zero;

        foreach (GravityArea gravityArea in gravityAreas)
        {
            Vector3 gravityDirection = gravityArea.GetGravityDirection(this).normalized;
            cumulativeGravityDirection += gravityDirection;
        }

        // Calculate the middle force direction if the player is affected by two areas
        if (gravityAreas.Count >= 2)
        {
            cumulativeGravityDirection /= gravityAreas.Count;
        }

        //Debug.Log("in " + gravityAreas.Count + " walls");
        //Debug.Log(cumulativeGravityDirection);
        return cumulativeGravityDirection.normalized;
    }

    public void AddGravityArea(GravityArea gravityArea)
    {
        gravityAreas.Add(gravityArea);
    }

    public void RemoveGravityArea(GravityArea gravityArea)
    {
        gravityAreas.Remove(gravityArea);
        
    }

    /*
    public static float GetGravityForce()
    {
        return GRAVITY_FORCE;
    }

    public static void SetGravityForce(float value)
    {
        GRAVITY_FORCE = value;
    }
    */
}
