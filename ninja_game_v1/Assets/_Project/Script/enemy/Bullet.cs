using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float life = 10f; // Time in seconds before the GameObject gets destroyed

    private void Start()
    {
        Destroy(gameObject, life);
    }
}
