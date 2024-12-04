using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword_Attack : MonoBehaviour
{

    // att animation 
    public Animator anim;
    private float next = 0f;
    float lastClickTime = 0;
    public float maxComboDelay = 1;
    public float attackDuration = 0.2f;

    //att 
    public LayerMask enemyLayer;
    private BoxCollider hitbox;
    private float Att1Length, Att2Length;
    private bool AttackStarted = false;


    // block
    private bool isBlocking;
    private bool isDefending;
    private bool isAttacking;
    private Collider Defending_HitBox;
    private bool Block_Success;

    //ref
    private Player_Movement pm;
    [SerializeField] private Transform orientation;

    [SerializeField] private AnimationClip Att1;
    [SerializeField] private AnimationClip Att2;

    public GameObject Defending_HitBox_Obj;
    public float blockWindow = 0.2f;

    // Combo
    private int currentComboCount = 0;

    

    // Start is called before the first frame update
    void Start()
    {
        pm = GetComponentInParent<Player_Movement>();
        Defending_HitBox = Defending_HitBox_Obj.GetComponent<Collider>();
        hitbox = GetComponent<BoxCollider>();
        hitbox.isTrigger = true;

        Att1Length = Att1.length;
        Att2Length = Att2.length;

    }

    // Update is called once per frame
    void Update()
    {

        if (AttackStarted)
        {
            if ((Time.time - lastClickTime > maxComboDelay) )
            {
                currentComboCount = 0;
                anim.SetBool("ExitAttack", true);
                pm.freeze = false;
                AttackStarted = false;
            }

        }
        
        
        if (Time.time > next)
        {
            if (Input.GetMouseButtonDown(0) && !isDefending && !isBlocking && !isAttacking)
            {
                anim.SetBool("ExitAttack", false);
                if (pm.GetGrounded())
                {
                    OnClick();
                    
                }
            }
        }

        // Blocking function
        if (Input.GetMouseButtonDown(1) && !isAttacking)
        {
            // Call a method to handle blocking logic
            isBlocking = true;
            Block_Success = false;
            pm.freeze = true;
            Defending_HitBox.enabled = true;
            anim.SetBool("isBlock", true);
            StartCoroutine(Block());
            
        }

        if (Input.GetMouseButton(1))
        {
            if (isBlocking || Block_Success)
            {
                // Already blocking, no need to defend
                return;
            } 

            isDefending = true;
            pm.freeze = true;
            Defending_HitBox.enabled = true;
            anim.SetBool("isDefend", true);
            Defend();
            // Play defensive animation or adjust player's collider
            
        }
        else
        {
            isDefending = false;
            if (!isAttacking)
                pm.freeze = false;

            if (!isBlocking)
                Defending_HitBox.enabled = false;

            anim.SetBool("isDefend", false);
            anim.SetBool("isBlock", false);
            anim.SetBool("Defend_Success", false);
            // Reset defensive animation or adjust player's collider
        }

        
        // Reset blocking flag
        if (Input.GetMouseButtonUp(1))
        {
            isBlocking = false;
            isDefending = false;
            if(!isAttacking)
                pm.freeze = false;
            Defending_HitBox.enabled = false;
            anim.SetBool("isBlock", false);
        }
        
    }


    void OnClick()
    {
        lastClickTime = Time.time;

        if(currentComboCount >= 2 ) 
        {
            currentComboCount = 0;
        }

        currentComboCount++;


        if (currentComboCount == 1)
        {
            AttackStarted = true;
            anim.SetTrigger("Attack");
            StartCoroutine(ResetAttackParameter(attackDuration, false));
        }
        else if (currentComboCount == 2)
        {
            anim.SetTrigger("Attack");
            StartCoroutine(ResetAttackParameter(attackDuration, true));
        }


    }


    IEnumerator ResetAttackParameter(float _attackDuration, bool _isEndofCombo)
    {
        isAttacking = true;
        pm.freeze = true;

        yield return new WaitForSeconds(_attackDuration);

        
        isAttacking = false;
        if (_isEndofCombo ) 
        {
            pm.freeze = false;
            anim.SetBool("ExitAttack", true); 
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if ((enemyLayer == (enemyLayer | (1 << other.gameObject.layer))) && isAttacking)
        {
            Damage_Handler enemyDamageHandler = other.GetComponent<Damage_Handler>();
            Debug.Log("Hit an enemy!");

            //do damage
            if (enemyDamageHandler != null)
            {
                int damage = 20;
                enemyDamageHandler.TakeDamage(damage);
            }
        }
    }


    private System.Collections.IEnumerator Block()
    {

        yield return new WaitForSeconds(blockWindow);

        if (isBlocking && CheckWeaponCollision())
        {
            // Block successful
            Debug.Log("Block successful!");

            Block_Success = true;
            // Play blocking animation, reduce damage, etc.
            anim.SetTrigger("Block_SuccessT");
            //anim.SetBool("isBlock", false);
            yield return new WaitForSeconds(0.3f);
            anim.SetBool("isBlock", false);
            anim.SetTrigger("Block_End");
        }

        isBlocking = false;
    }

    private void Defend()
    {
        anim.SetBool("Defend_Success", false); 

        if (isDefending && CheckWeaponCollision())
        {
            // Defend successful 
            Debug.Log("Defended attack!");
            anim.SetBool("Defend_Success", true);
            
        }
    }

    private bool CheckWeaponCollision()
    {
        // Find all objects with the "weapon" tag
        GameObject[] weaponObjects = GameObject.FindGameObjectsWithTag("weapon");

        foreach (GameObject weaponObject in weaponObjects)
        {
            // Skip the object holding this script
            if (weaponObject == gameObject)
                continue;

            Collider playerCollider = Defending_HitBox;
            Collider weaponCollider = weaponObject.GetComponent<Collider>();
            
            

            // Check collision between player weapon and enemy weapon
            if (playerCollider.bounds.Intersects(weaponCollider.bounds))
            {
                return true;
            }
        }

        return false;
    }

    public void EnableHitbox()
    {
        hitbox.enabled = true;
    }
    public void DisableHitbox()
    {
        hitbox.enabled = false;
    }
}
