using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class GravityArea : MonoBehaviour
{
    [SerializeField] private int _priority;
    public int Priority => _priority;
    
    void Start()
    {
        transform.GetComponent<Collider>().isTrigger = true;
    }
    
    public abstract Vector3 GetGravityDirection(Gravity_Control Gravity_Control);
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Gravity_Control Gravity_Control))
        {
            Gravity_Control.AddGravityArea(this);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Gravity_Control Gravity_Control))
        {
            Gravity_Control.RemoveGravityArea(this);
        }
    }
}