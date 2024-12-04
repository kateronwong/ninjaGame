using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Movement : MonoBehaviour
{
    // Start is called before the first frame update
    public abstract Vector3 GetVelocity();
    public abstract Vector3 GetMovementVelocity();
    public abstract bool IsGrounded();

    //Events;
    public delegate void VectorEvent(Vector3 v);
    public VectorEvent OnJump;
    public VectorEvent OnLand;
}
