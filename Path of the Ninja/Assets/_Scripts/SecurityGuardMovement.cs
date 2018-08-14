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

    private SecurityWatchScript watchScript;
    public float confirmedAt = 0.0f, runningSpeed;
    public bool cautious = false, playerDetected = false;
    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        watchScript = GetComponent<SecurityWatchScript>();
        
        chill = Time.time + turnFrequency;
        turnAt = chill + stayTime;
        scale = transform.localScale;
    }
	
	// Update is called once per frame
	void Update () {
        HandleMovement();
        HandleTurnAround();
        HandleDetection();
        animator.SetBool("staying", staying);

	}

    void HandleDetection()
    {
        if (watchScript.getDetectionStatus())
        {
            if (!cautious)
            {
                cautious = true;
                confirmedAt = Time.time + 5.0f;
            } else if(confirmedAt > Time.time)
            {
                playerDetected = true;
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
