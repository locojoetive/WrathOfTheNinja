using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaMove : MonoBehaviour {
    private NinjaStateHandler stateHandler;
    private Rigidbody2D rb;
    private CapsuleCollider2D collider;
    
    public float horizontal = 0.0f,
        vertical = 0.0f,
        walkingSpeed = 5.0f,
        jumpHeight = 5F,
        fallMultiplier = 2.5f,
        lowJumpMultiplier = 2F,
        climbingSpeed = 3F;
    private bool climbing = false,
        facingRight = true,
        jump = false,
        jumping = false,
        walled = false;

    private void getComponents()
    {
        stateHandler = GetComponent<NinjaStateHandler>();
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<CapsuleCollider2D>();
    }

    private void Start()
    {
        getComponents();
    }

    private void FixedUpdate()
    {
        HandleInput();
        HandleFacing();
        HandleMovement();
        HandleJump();
        HandleClimbing();
    }

    private void HandleInput()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        jumping = Input.GetKey(KeyCode.L);
        jump = Input.GetKeyDown(KeyCode.L);
    }

    private void HandleFacing()
    {
        if (horizontal > 0.1f && !facingRight ||
            horizontal < -0.1f && facingRight)
        {
            facingRight = !facingRight;
            transform.localScale = Vector3.Scale(transform.localScale, new Vector3(-1, 1, 1));
        }
    }

    private void HandleMovement()
    {
        if (!stateHandler.isWayBlocked())
        { rb.velocity = new Vector2(walkingSpeed * horizontal, rb.velocity.y); }
    }

    private void HandleJump()
    {
        if (stateHandler.isGrounded() && jump)
        {
            rb.AddForce(new Vector2(rb.velocity.x, jumpHeight), ForceMode2D.Impulse);
        }

        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !jumping)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    private void HandleClimbing()
    {
        if(stateHandler.isClimbing())
        {
            rb.velocity = Vector2.up * climbingSpeed;
        }
    }
    
    public bool isFacingRight()
    {
        return facingRight;
    }

    public float getHorizontalInput()
    {
        return Mathf.Abs(horizontal);
    }

    public float getVerticalInput()
    {
        return vertical;
    }
}