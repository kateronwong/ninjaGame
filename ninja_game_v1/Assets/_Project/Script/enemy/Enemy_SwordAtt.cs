using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_SwordAtt : MonoBehaviour
{
    //att 
    public LayerMask PlayerLayer;
    private BoxCollider hitbox;

    private static bool isAttacking;

    public int damage = 10;

    // Start is called before the first frame update
    private void Start()
    {
        hitbox = GetComponent<BoxCollider>();
        hitbox.isTrigger = true;
    }

    private void Update()
    {
        if (isAttacking)
        {
            hitbox.enabled = true;
        }
        else
        {
            hitbox.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hitbox.enabled && other.CompareTag("Player"))
        {
            Damage_Handler playerDamageHandler = other.GetComponent<Damage_Handler>();
            if (playerDamageHandler != null)
            {
                playerDamageHandler.TakeDamage(damage);
            }
        }
    }

    public bool getIsAttacking()
    {
        return isAttacking;
    }

    public static void setIsAttacking(bool _isAttacking)
    {
        isAttacking = _isAttacking;
    }

    
}
