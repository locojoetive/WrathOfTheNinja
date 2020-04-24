using System.Linq;
using UnityEngine;

public class NinjaMove : MonoBehaviour {
    private Rigidbody2D physicalBody;
    private Animator anim;
    private NinjaStatesAnimationSound state;
    
    private Vector2 wallJumpDir = Vector2.zero;
    private float
        slidingSpeed = 3f,
        translation = 5.0f;
    public float
        ledgeJumpHeight = 5F,
        ledgeLandingSpeed = 5F,
        horizontalDamping = 0.5f,
        fallMultiplier = 2.5f,
        lowJumpMultiplier = 0.5f,
        timeScale = 0.8f,
        climbingSpeed = 3F,
        jumpHeight = 3.5f,
        walkingSpeed = 5F,
        wallJumpSpeed = 3F,
        wallJumpHeight = 3F;
    private float velocityX;
    private float fJumpPressedRemember = 0;
    private float fGroundedRemember = 0;
    public float 
        fJumpPressedRememberTime = 0.5f,
        fGroundedRememberTime;
    

    // Jump And Fall Debugging
    private GameObject line;
    private int lineIndex = 0;
    private int maximumNumberOfLines = 200;

    void Start()
    {
        physicalBody = GetComponent<Rigidbody2D>();
        state = GetComponent<NinjaStatesAnimationSound>();
        Time.timeScale = timeScale;
    }

    private void Update()
    {
        HandleJumpAndFall();
    }

    private void FixedUpdate()
    {
        DebugJumpAndFall();

        if (!state.isWallJumping())
        {
            changeFacing();
            if (state.controlHorizontal) HandleMovement();
            HandleClimbing();
        }
    }



    private void HandleMovement()
    {
        if (!state.isWalled())
        {
            float horizontalVelocity = walkingSpeed 
                * (state.isDucking() ? 0.5f : 1F)
                * (InputManager.xAxis > 0.2f 
                    ? 1F 
                    : InputManager.xAxis < -0.2f 
                        ? -1F
                        : 0F
                );
            physicalBody.velocity = Vector3.Lerp(physicalBody.velocity, new Vector2(horizontalVelocity, physicalBody.velocity.y), 10F);
        }
        if (state.isLanding())
        {
            physicalBody.AddForce(new Vector3(state.isFacingRight() ? translation : -translation, 0F, 0F), ForceMode2D.Force);
        } else if(state.isWedged() && !state.isGroundedFront() && !state.isWalled() && physicalBody.velocity.y == 0.0f)
        {
            physicalBody.velocity = new Vector2(state.isFacingRight() ? -3.5f : 3.5f, 0.0f);
        }
    }

    public void HandleClimbing()
    {
        if (state.isLedgeClimbingUp())
        {
            physicalBody.velocity = new Vector2(0F, ledgeJumpHeight);
        }
        else if (state.isLedgeLanding())
        {
            physicalBody.velocity = new Vector2(state.isFacingRight() ? ledgeLandingSpeed : -ledgeLandingSpeed, physicalBody.velocity.y); 
        }
        if (state.isClimbing())
        {
            physicalBody.velocity = new Vector2(0.0f, climbingSpeed);
        }
        if (state.isSliding() && physicalBody.velocity.y < -slidingSpeed)
        {
            physicalBody.velocity = new Vector2(0F, -slidingSpeed);
        }
    }

    private void HandleJumpAndFall() {

        fGroundedRemember -= Time.deltaTime;
        if (state.isGrounded())
        {
            fGroundedRemember = fGroundedRememberTime;
            
        }
        fJumpPressedRemember -= Time.deltaTime;
        if (state.jumpButton)
        {
            fJumpPressedRemember = fJumpPressedRememberTime;
        }

        if (fJumpPressedRemember > 0 && fGroundedRemember > 0 && !state.isDucking())
        {
            fJumpPressedRemember = 0F;
            fGroundedRemember = 0F;
            physicalBody.velocity = new Vector2(physicalBody.velocity.x, jumpHeight);
        }
  
        if (state.jumpButton && state.isOnWall())
        {
            HandleWallJump();
        }
        else if (state.isWallJumping())
        {

            float horizontalVelocity = 0f;
            if (state.commitAndGo())
            {
                horizontalVelocity = state.horizontal * walkingSpeed;
            }
            horizontalVelocity = Mathf.Abs(horizontalVelocity) > Mathf.Abs(physicalBody.velocity.x) ? horizontalVelocity : physicalBody.velocity.x;
            physicalBody.velocity = new Vector2(horizontalVelocity, physicalBody.velocity.y);
            /*
            float horizontalVelocity = state.horizontal * walkingSpeed;
            horizontalVelocity = Mathf.SmoothDamp(physicalBody.velocity.x, horizontalVelocity, ref velocityX, 10F*horizontalDamping);
            physicalBody.velocity = new Vector2(horizontalVelocity, physicalBody.velocity.y);
            */
        }

        if (!InputManager.jumpContinuous && physicalBody.velocity.y > 0F)
        {
            physicalBody.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1F) * Time.deltaTime;
        }

        if (physicalBody.velocity.y < 0F && !state.grounded)
        {
            physicalBody.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1F) * Time.deltaTime;

            // physicalBody.velocity = new Vector2(physicalBody.velocity.x, physicalBody.velocity.y * fallMultiplier);
        }
    }


    void DebugJumpAndFall()
    {
        if(!state.grounded)
        {
            if (lineIndex == 0)
            {
                if (line == null)
                {
                    line = new GameObject("line");
                    line.AddComponent<LineRenderer>();
                }

                line.GetComponent<LineRenderer>().positionCount = maximumNumberOfLines;
                line.GetComponent<LineRenderer>().SetPosition(lineIndex, transform.position);
                line.GetComponent<LineRenderer>().startWidth = 0.25f;
                line.GetComponent<LineRenderer>().endWidth = 0.25f;
                lineIndex = 0;
            }
            foreach (int index in Enumerable.Range(lineIndex, maximumNumberOfLines - lineIndex))
            {
                line.GetComponent<LineRenderer>().SetPosition(index, transform.position);
            }
            line.GetComponent<LineRenderer>().SetPosition(lineIndex, transform.position);
            lineIndex++;
        } 
        else if (state.grounded && lineIndex != 0)
        {
            line.GetComponent<LineRenderer>().positionCount = lineIndex;
            lineIndex = 0;
        }
    }
    public void HandleWallJump()
    {
        TurnDirection();
        wallJumpDir = new Vector2(state.isFacingRight() ? wallJumpSpeed : -wallJumpSpeed, wallJumpHeight);
        physicalBody.velocity = Vector2.zero;
        transform.Translate(state.isFacingRight() ? 0.1f * Vector2.right : -0.1f * Vector2.right);
        physicalBody.velocity = wallJumpDir;
        state.initialzeWallJump();
    }

    void changeFacing()
    {
        if ((state.isFacingRight() && state.horizontalInput() < 0F)
            || !state.isFacingRight() && state.horizontalInput() > 0F)
        {
            if (!state.climbing || state.climbing && Mathf.Abs(state.horizontalInput()) > 0.5f)
                TurnDirection();
        }
    }

    private void TurnDirection()
    {
        if (state.isLedgeClimbingUp() || state.isLedgeLanding()) {
            state.resetLedgeClimb();
        }
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }
}