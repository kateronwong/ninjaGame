using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

//backup of sword attack
public class Sword_Combat : MonoBehaviour
{
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

    private bool isAttacking;

    [SerializeField] private AnimationClip Att1;
    [SerializeField] private AnimationClip Att2;

    private int currentComboCount = 0;

    private Player_Movement_N pm;
    public Player_Animation_Control_N PAC;

    void Start()
    {
        pm = GetComponentInParent<Player_Movement_N>();

        Att1Length = Att1.length;
        Att2Length = Att2.length;
    }

    void Update()
    {
        if (AttackStarted)
        {
            if ((Time.time - lastClickTime > maxComboDelay))
            {
                currentComboCount = 0;
                pm.freeze = false;
                AttackStarted = false;
            }
        }

        if (Time.time > next)
        {
            if (Input.GetMouseButtonDown(0) && !isAttacking)
            {
                    OnClick();

            }
        }

    }

    void OnClick()
    {
        lastClickTime = Time.time;

        currentComboCount++;

        if (currentComboCount > 2)
        {
            currentComboCount = 1;
        }


        if (currentComboCount == 1)
        {
            AttackStarted = true;
            anim.SetTrigger("Attack" + currentComboCount);
            StartCoroutine(ResetAttackParameter(attackDuration, false));
        }
        else if (currentComboCount == 2)
        {
            anim.SetTrigger("Attack" + currentComboCount);
            StartCoroutine(ResetAttackParameter(attackDuration, true));
        }

    }
    IEnumerator ResetAttackParameter(float _attackDuration, bool _isEndofCombo)
    {
        isAttacking = true;
        pm.isUsingRootMotion = true;
        pm.freeze = true;

        yield return new WaitForSeconds(_attackDuration);


        isAttacking = false;
        pm.isUsingRootMotion = false;
        if (_isEndofCombo)
        {
            pm.freeze = false;
        }
    }

    /*
    public Animator anim;

    public bool isBlocking;

    public bool isAttacking;
    private float timeSinceAttack;
    private int currentCombo = 0;


    public Player_Movement_N pm;


    private void Update()
    {
        timeSinceAttack += Time.deltaTime;
        Attack();

        Block();
    }

    private void Block()
    {
        if (Input.GetMouseButton(1))
        {
            anim.SetBool("Block", true);
            isBlocking = true;
            pm.freeze = true;
        }
        else
        {
            anim.SetBool("Block", false);
            isBlocking = false;
            pm.freeze = false;
        }

    }

    private void Attack()
    {
        if (Input.GetMouseButtonDown(0) && timeSinceAttack > 0.5f)
        {
            currentCombo++;
            isAttacking = true;
            pm.freeze = true;

            if (currentCombo > 3)
                currentCombo = 1;

            if (timeSinceAttack > 0.7f)
                currentCombo = 1;

            anim.SetTrigger("Attack" + currentCombo);

            timeSinceAttack = 0;
        }

    }

    public void ResetAttack()
    {
        isAttacking = false;
        pm.freeze = false;
    }
    */
}
