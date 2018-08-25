using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityGuardMovement : MonoBehaviour {

    public float speed, stayTime, turnFrequency;
    private float chill = 0.0f, turnAt = 0.0f;
    private bool facingRight = true;
    private Rigidbody2D rb;
    private Vector3 scale;
    private bool staying = false;
    private Animator animator;
    private SecurityWatch watchScript;
    private float confirmedAt = 0.0f;
    public float confirmedAfter, runningSpeed;
    public bool cautious = false, playerDetected = false;
    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        watchScript = GetComponentInChildren<SecurityWatch>();
        chill = Time.time + turnFrequency;
        turnAt = chill + stayTime;
        scale = transform.localScale;
    }
	
	// Update is called once per frame
	void Update () {
        animator.SetBool("staying", staying);

	}
    private void FixedUpdate()
    {
        HandleDetection();
        HandleMovement();
        HandleTurnAround();
    }

    void HandleDetection()
    {
        if (watchScript.getDetectionStatus())
        {
            if (!cautious)
            {
                cautious = true;
                confirmedAt = Time.time + confirmedAfter;
                staying = true;
            } else if(confirmedAt < Time.time)
            {
                playerDetected = true;
                staying = false;
            }
        } else
        {
            cautious = false;
            playerDetected = false;
        }
    }

    void HandleMovement()
    {
        if ( !cautious && Time.time < chill )
        {
            staying = false;
            rb.velocity = facingRight ? new Vector2(speed, rb.velocity.y) : new Vector2(-speed, rb.velocity.y);
        } else if (playerDetected)
        {
            staying = false;
            rb.velocity = facingRight ? new Vector2(2*speed, rb.velocity.y) : new Vector2(-2*speed, rb.velocity.y);
        }
    }

    void HandleTurnAround()
    {
        if (!cautious && Time.time > chill)
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
