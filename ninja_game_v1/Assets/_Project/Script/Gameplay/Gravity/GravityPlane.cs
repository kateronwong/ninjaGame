using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityPlane : GravityArea
{
    public override Vector3 GetGravityDirection(Gravity_Control Gravity_Control)
    {
        return -transform.up;
    }
}
