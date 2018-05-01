using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaAttacks : MonoBehaviour
{
    private BoxCollider2D attackCol;
    private Animator anim;
    public Transform shurikenSpawn;
    public bool attacking = false;
    public int mode = 0;

    // Use this for initialization
    void Start()
    {
        attackCol = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleSwordAttack();
    }

    void HandleSwordAttack()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (mode == 0) {
                anim.SetTrigger("swordAttack");
                Debug.Log("HA!");
            }
        }
    }

    public bool getAttackStatus()
    {
        return attacking;
    }

    public void SetAttackStatus(int attackMode)
    {
        mode = attackMode;
    }
}