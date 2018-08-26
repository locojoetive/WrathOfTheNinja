using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaStateHandler : MonoBehaviour {
    private NinjaMove movement;

    private Rigidbody2D rb;
    private new CapsuleCollider2D collider;
    private Animator animator;
    public Transform groundCheckFront, 
        groundCheckBack, 
        wallCheckUp, 
        wallCheckDown,
        squeezeCheck, 
        wedgedCheck, 
        ledgeCheck;
    private float inputHorizontal = 0.0f,
        inputVertical = 0.0f,
        boxSize = 0.025f;
    public bool climbing = false,
        falling = false,
        groundedFront = false,
        groundedBack = false,
        grounded = false,
        goingUp = false,
        ledged = false,
        sliding = false,
        walledUp = false,
        walledDown = false,
        walled = false,
        wantingUp = false;
    private int whatIsI = 0,
        whatIsGround = 0,
        whatIsLedge = 0,
        whatIsClimbable = 0,
        whatIsClimbableLeft = 0,
        whatIsClimbableRight = 0,
        whatIsPermeable = 0;

    private Vector2 checkBox;

    void Start ()
    {
        getComponents();
        positionSensors();
        defineLayers();
    }

    private void Update()
    {
        positionSensors();
        setAnimations();
    }

    void FixedUpdate () {
        HandleState();
	}

    private void setAnimations()
    {
        animator.SetBool("climbing", climbing);
        animator.SetBool("grounded", grounded);
        animator.SetFloat("horizontal", movement.getHorizontalInput());
        animator.SetBool("sliding", sliding);
        animator.SetFloat("vertical", movement.getVerticalInput());
        animator.SetBool("walled", walled);
        animator.SetFloat("yVelocity", rb.velocity.y);

    }

    private void HandleState()
    {
        whatIsClimbable = (movement.isFacingRight() ? whatIsClimbableRight : whatIsClimbableLeft) | whatIsGround;
        Vector2 wallDetectionSize = new Vector2(boxSize, 0.25f * collider.size.y),
            groundDetectionSize = new Vector2(0.25f * collider.size.x, boxSize);

        groundedFront = Physics2D.OverlapBox(
            groundCheckFront.position,          // sensor positions
            groundDetectionSize,                // box size
            0.0f,                               // angle
            whatIsGround                        // respected layer
        );
        groundedBack= Physics2D.OverlapBox(
            groundCheckBack.position,
            groundDetectionSize, 
            0.0f, 
            whatIsGround
        );
        walledUp = Physics2D.OverlapBox(
            wallCheckUp.position,
            wallDetectionSize,
            0.0f, 
            whatIsClimbable
        );
        walledDown = Physics2D.OverlapBox(
            wallCheckDown.position, 
            wallDetectionSize,
            0.0f,
            whatIsClimbable
        );
        ledged = Physics2D.OverlapBox(
            wallCheckUp.position, 
            wallDetectionSize, 
            0.0f, 
            whatIsLedge
        );
        falling = rb.velocity.y < -0.01;
        goingUp = rb.velocity.y > 0.01;
        wantingUp = movement.getVerticalInput() > 0;
        grounded = groundedFront && groundedBack;
        walled = walledUp && walledDown && !grounded && (wantingUp);
        climbing = walled && wantingUp;
        sliding = walled && falling;
        DrawBox(groundCheckFront.position, groundDetectionSize, Color.blue);
        DrawBox(groundCheckBack.position, groundDetectionSize, Color.cyan);
        DrawBox(wallCheckUp.position, wallDetectionSize, Color.green);
        DrawBox(wallCheckDown.position, wallDetectionSize, Color.yellow);
        DrawBox(wedgedCheck.position, checkBox, Color.red);


        // squeezeCheck
        // ledgeCheck
        // wedgedCheck
    }

    private void getComponents()
    {
        movement = GetComponent<NinjaMove>();
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
    }

    private void defineLayers()
    {
        whatIsI = 1 << 8;
        whatIsGround = 1 << 0;
        whatIsLedge = 1 << 9;
        whatIsClimbableRight = 1 << 10;
        whatIsClimbableLeft = 1 << 11;
        whatIsPermeable = 1 << 12;
    }

    public bool isGrounded()
    {
        return grounded;
    }

    public bool isWalled()
    {
        return walled;
    }

    public bool isWayBlocked()
    {
        return walledDown || walledUp;
    }

    public bool isClimbing()
    {
        return climbing;
    }

    private void positionSensors()
    {
        groundCheckFront.localPosition = new Vector2(collider.offset.x + 0.25f * collider.size.x, collider.offset.y - 0.5f * collider.size.y);
        groundCheckBack.localPosition = new Vector2(collider.offset.x - 0.25f * collider.size.x, collider.offset.y - 0.5f * collider.size.y);
        wallCheckUp.localPosition = new Vector2(collider.offset.x + 0.5f * collider.size.x, collider.offset.y + 0.25f * collider.size.y);
        wallCheckDown.localPosition = new Vector2(collider.offset.x + 0.5f * collider.size.x, collider.offset.y - 0.25f * collider.size.y);
        squeezeCheck.localPosition = new Vector2(collider.offset.x, collider.offset.y + 0.5f * collider.size.y); //for staying duck, if no space
        ledgeCheck.localPosition = new Vector2(collider.offset.x + 0.5f * collider.size.x, collider.offset.y + 0.5f * collider.size.y - 0.5f * boxSize);
        wedgedCheck.localPosition = new Vector2(collider.offset.x + 0.4375f * collider.size.x, collider.offset.y - 0.4375f * collider.size.y);
    }

    public void DrawBox(Vector3 center, Vector3 size, Color color)
    {
        Vector3 rightTop = center + size,
            leftTop = center + new Vector3(-size.x, size.y),
            rightBottom = center + new Vector3(size.x, -size.y),
            leftBottom = center + new Vector3(-size.x, -size.y);
        Debug.DrawLine(rightTop, leftTop, color);
        Debug.DrawLine(leftTop, leftBottom, color);
        Debug.DrawLine(leftBottom, rightBottom, color);
        Debug.DrawLine(rightBottom, rightTop, color);
    }
}
