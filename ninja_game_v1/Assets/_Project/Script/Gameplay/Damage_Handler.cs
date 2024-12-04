using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage_Handler : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    // Start is called before the first frame update
    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage!");

        if (currentHealth <= 0)
        {
            Die();

        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} died!");
        // Add any death-related logic here, like playing death animation, disabling the object, etc.
        Destroy(gameObject);
    }

    public void DealDamage(Damage_Handler target, int damage)
    {
        target.TakeDamage(damage);
    }
}
