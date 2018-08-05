using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityGuardMovement : MonoBehaviour {

    public float speed, stayTime, turnFrequency;
    private float chill = 0.0f, turnAt = 0.0f;
    private bool facingRight = true;
    private Rigidbody2D rb;
    private Vector3 scale;
    private SecurityWatchScript watchScript;
    private bool staying = false;
    private Animator animator;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        chill = Time.time + turnFrequency;
        turnAt = chill + stayTime;
        scale = transform.localScale;
        watchScript = GetComponent<SecurityWatchScript>();
        animator = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
        HandleMovement();
        HandleTurnAround();
        animator.SetBool("staying", staying);
	}

    void HandleMovement()
    {
        if (Time.time < chill)
        {
            staying = false;
            rb.velocity = facingRight ? new Vector2(speed, rb.velocity.y) : new Vector2(-speed, rb.velocity.y);
        }
    }

    void HandleTurnAround()
    {
        if (Time.time > chill)
        {
            if (!staying)
            {
                staying = true;
                turnAt = Time.time + stayTime;
            } else if(turnAt < Time.time)
            {
                facingRight = !facingRight;
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                chill = turnAt + turnFrequency;
            }
        }

    }
}
