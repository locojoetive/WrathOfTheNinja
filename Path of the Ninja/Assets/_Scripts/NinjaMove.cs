using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaMove : MonoBehaviour {
    private bool facingRight = true;
    public Vector3 velocity;
    //Unity Components
    private Rigidbody2D rb;
    private Transform tr;
    private BoxCollider2D col;
    private Animator anim;
    private NinjaAttacks ninjaAttack;
    
    //Floor & Hit Detection
    private LayerMask whatIsGround;
    private LayerMask whatIsLedge;
    public Transform groundCheckFront;
    public Transform groundCheckBack;
    public Transform wallCheckUp;
    public Transform wallCheckDown;
    public Transform squeezeCheck;
    public Transform landingCheck;
    public Transform ledgeCheck;
    private float boxSize = 0.005f;
    private float slidingTime =0.0f;
    public float translation;

    //Movement
    private int combo = 0;
    public float horizontal, vertical;
    private float walkingSpeed = 5F, runningSpeed = 0.0f, jumpHeight = 3F,
        climbingSpeed = 1.5f;

    //Sliding Attributes

    public float slideBeginningSpeed = 0.0f, slidingSpeed = 1.5f, slidingTimeFrame = 0.0f;


    public float x = 0.0f, ms = 1F, wallJumpStart = 0.0f, wallJumpFrame = 0.25f, wallJumpEnd = 0.0f, wallJumpHeight = 2.75f, radix = 10F;

    private bool grounded = false, running = false, ducking = false, walking = true;
    public bool paused = false, climbing = false, controlHorizontal = true, controlVertical = true, sliding = false;
    public bool walled = false, walledUp = false, walledDown = false, squeezed= false, gF = false, gB = false, 
        onLedge = false, slipping = false, landing = false, wallGrab = false, wallJumping = false;

    //Extended Movement
    private float duckingMeasurement = 0.0f;
    public bool wedged = false;

    //Jump
    private float tapTime = 0.2f, lastTap = 0.0f, jumpTime = 0.0f, jumpTimeFrame = 0.2f;
    private int jumpNo = 0, allowedJumps = 1;
    private float fallMultiplier = 2.5f;
    private float lowJumpMultiplier = 2f;

    public void HandleClimbing()
    {
        if (onLedge)
        {
            slidingTime = 0.0f;
            rb.velocity = Vector2.zero;
            if (vertical > 0)
            {
                rb.AddForce(Vector2.up * 3.0f, ForceMode2D.Impulse);
            }
        }
        else if (climbing)
        {
            slidingTime = 0.0f;
            rb.velocity = new Vector2(0.0f, climbingSpeed * vertical);
        }
        else if (wallGrab)
        {
            if (slidingTime == 0.0f) slideBeginningSpeed = rb.velocity.y;
            float speed = - linearDecrease(slidingTime, slidingSpeed, slideBeginningSpeed, slidingTimeFrame);
            slidingTime += Time.deltaTime;
            rb.velocity = new Vector2(0.0f, speed);
        }
        else
            slidingTime = 0.0f;
    }


    // Use this for initialization
    void Start()
    {
        //components for manupilation
        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<Transform>();
        col = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        ninjaAttack = GetComponent<NinjaAttacks>();

        //Time.timeScale = 0.25f;
        //initialize ground tags & sensors positions
        whatIsGround.value = 1 << 8;
        whatIsLedge.value = 2 << 8;
        /*//initialize consistent player size
        startScaleX = transform.localScale.x;
        startScaleY = transform.localScale.y;
        A = startScaleX * startScaleY;*/
        
        duckingMeasurement = col.offset.y + 0.5f *col.size.y;
    }

    // Update is called once per frame
    void Update()
    {
        HandleAnimation();        
    }

    private void FixedUpdate()
    {
        velocity = rb.velocity;
        HandleSensors();
        HandleState();

        HandleClimbing();
        changeFacing();
        HandleMovement();
        HandleJumpAndFall();
    }

    private void HandleSensors()
    {
        groundCheckFront.localPosition = new Vector2(col.offset.x + 0.25f * col.size.x, col.offset.y - 0.5f * col.size.y);
        groundCheckBack.localPosition = new Vector2(col.offset.x - 0.25f * col.size.x, col.offset.y -0.5f * col.size.y);
        wallCheckUp.localPosition = new Vector2(col.offset.x + 0.5f * col.size.x, col.offset.y + 0.25f * col.size.y);
        wallCheckDown.localPosition = new Vector2(col.offset.x + 0.5f * col.size.x, col.offset.y - 0.25f * col.size.y);
        squeezeCheck.localPosition = new Vector2(col.offset.x, duckingMeasurement); //for staying duck, if no space

        ledgeCheck.localPosition = new Vector2(col.offset.x + 0.5f * col.size.x, col.offset.y + 0.5f * col.size.y - 0.5f * boxSize);
        landingCheck.localPosition = new Vector2(col.offset.x + 0.5f * col.size.x, col.offset.y - 0.5f * col.size.y);
    }

    private void HandleState()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        //get active sensors
        gF = Physics2D.OverlapBox(groundCheckFront.position, new Vector2(0.5f * col.size.x - 0.015f, boxSize), 0.0f, whatIsGround.value);
        gB = Physics2D.OverlapBox(groundCheckBack.position, new Vector2(0.5f * col.size.x - 0.015f, boxSize), 0.0f, whatIsGround.value);
        grounded = gF && gB;

        //climbing
        onLedge = !grounded && Physics2D.OverlapBox(ledgeCheck.position, new Vector2(boxSize, boxSize), 0.0f, whatIsLedge.value);
        walledUp = !onLedge && Physics2D.OverlapBox(ledgeCheck.position, new Vector2(boxSize, boxSize), 0.0f, whatIsGround.value);
        walledDown = Physics2D.OverlapBox(wallCheckDown.position, new Vector2(boxSize, 0.5f * col.size.y - 0.015f), 0.0f, whatIsGround.value);
        squeezed = Physics2D.OverlapBox(squeezeCheck.position, new Vector2(col.size.x - 0.1f, boxSize), 0.0f, whatIsGround.value);
        walled = walledUp && walledDown && !grounded;

        wedged = Physics2D.OverlapBox(landingCheck.position, 2 * new Vector2(boxSize, boxSize), 0.0f, whatIsGround.value);

        if (wallJumping && wallJumpEnd < Time.time) {
            wallJumping = false;
        }

        //decides wether player can control rigidbody
        //nicht an Kanten verkeilen
        controlHorizontal = !wallJumping && !((walledDown && !walledUp) || (!walledDown && walledUp));
        controlVertical = !(onLedge);


        ducking = grounded && (vertical < -0.5f || squeezed);

        landing = gF && !gB && !walledDown && !walledUp;
        slipping = !gF && gB && !walledDown;


        //wall behaviour
        wallGrab = !(rb.velocity.y > 0.0f && vertical < 0.01f) && walled && !gF;
        climbing = wallGrab && vertical > 0.1f;
        sliding = wallGrab && !climbing;
    }

    public float maxSpeed;

    private void HandleMovement()
    {
        if (controlHorizontal && !walled)
        {
            //if (Mathf.Abs(rb.velocity.x) < maxSpeed)
            //rb.AddForce(new Vector2(ducking ? horizontal * walkingSpeed * 0.5f : horizontal * walkingSpeed, rb.velocity.y), ForceMode2D.Force);
            //else rb.velocity = new Vector2(facingRight? maxSpeed : -maxSpeed, rb.velocity.y);
            rb.velocity = new Vector2(ducking ? horizontal * walkingSpeed * 0.5f : horizontal * walkingSpeed, rb.velocity.y);
            
        }

        if (slipping)
        {
            rb.velocity = new Vector2(facingRight ? translation : -translation, 0.0f);
            Debug.Log("slipping");
        } else if (landing)
        {
            rb.velocity = new Vector2(facingRight ? translation : -translation, rb.velocity.y);
            Debug.Log("Landing");
        }
        else if(wedged && !gF && !walledDown && rb.velocity.y == 0.0f)
        {
            rb.velocity = new Vector2(facingRight ? -3.5f : 3.5f, 0.0f);
            Debug.Log("Wedged!");
        }
    }

    public Vector2 wallJumpDir = Vector2.zero;

    private void HandleJumpAndFall() {

        if (grounded && jumpNo != 0)
        {
            jumpNo = 0;
            //Form change variables at landing || squeezeing variables
            //calcLandingsqueezeY();
        }
        else if (!grounded && jumpNo == 0)
        {
            jumpNo++;
        }
        
        //initialize jump
        if (Input.GetKeyDown(KeyCode.L) && grounded && !ducking)
        {
            rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
            jumpNo++;
        }
        //wallJump init
        else if ((Input.GetKeyDown(KeyCode.L) && !wallJumping && walled) || wallJumping)
        {
            HandleWallJump();
        }


        //adjust Jump & Fall Speed
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.L))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    public void HandleWallJump()
    {
        if (!wallJumping)
        {
            wallJumpStart = Time.time;
            wallJumpEnd = wallJumpStart + wallJumpFrame;
            wallJumping = true;
            controlHorizontal = false;

            transform.Translate(-0.1f * transform.right);
            TurnDirection();

            wallJumpDir = 2F * transform.right;
            wallJumpDir = facingRight ? wallJumpDir : -wallJumpDir;
            rb.velocity = Vector2.zero;
            rb.AddForce(wallJumpDir, ForceMode2D.Impulse);
            x = 0.0f;
        }
        else
        {
            x += Time.deltaTime;
            float y = wallJumpHeight * Mathf.Sin(wallJumpFrame * Mathf.PI * 0.5f * x);
            float hSpeed = 2F * walkingSpeed * (wallJumpFrame - x) / wallJumpFrame + horizontal * x / wallJumpFrame;
            float actualSpeed = horizontal * walkingSpeed;

            hSpeed = facingRight ? Mathf.Max(hSpeed, actualSpeed) : Mathf.Min(-hSpeed, actualSpeed);
            rb.velocity = new Vector2(hSpeed, wallJumpHeight - y);
        }
    }

    public float maxSS= 1F;
    
    



    public void Pause()
    {
        controlHorizontal = false;
        controlVertical = false;

        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
        Debug.Log("Yo Ninja!");
        paused = true;
    }

    public void Resume()
    {
        controlHorizontal = true;
        controlVertical = true;

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        Debug.Log("Let's go Ninja!");
        paused = false;
    }
    
    private float landingTimeFrame = 0.5f;
    private float endLandingAt = 0.0f;
    private float startScaleX = 0.5f, startScaleY = 1.0f;
    private float maxScaleY = 1.25f;
    private float A, velY;
    private Vector2 squeezeFunctionY;


    void changeFacing()
    {
        if (!paused && controlHorizontal && (facingRight && horizontal < 0 || !facingRight && horizontal > 0))
        {
            TurnDirection();
        }
    }

    void TurnDirection()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    void changeForm()
    {
        //facing direction
        changeFacing();

        velY = Mathf.Abs(rb.velocity.y);
        float scaleY = startScaleX;
        float scaleX = startScaleY;
        //= 0.25f + 0.25f*Mathf.Exp(-velY)
        /*float delta = endLandingAt - Time.time;
        if (false)//delta > 0.0f * landingTimeFrame)
        {
            scaleY = squeezeFunctionY.x * delta * delta + squeezeFunctionY.y * delta + squeezeYScaleFrom;
            scaleX = A / scaleY;
            //Debug.Log("Delta: " + delta + ", (" + scaleX + ", " + scaleY + ")");

        }*/

        if (ducking)
        {
            scaleY = startScaleY + vertical * 0.5f;
            scaleX = A / scaleY;
        }
        else if (!grounded && Mathf.Abs(velY) > 3.0f)
        {
            float G = maxScaleY - minScaleY;
            float shift = 9F;
            float f0 = maxScaleY - startScaleY;

            scaleY = startScaleY + G * 1.0f/(1+Mathf.Exp(-G*(velY-shift))*(G/f0 - 1.0f));
            if(maxScaleY < scaleY) scaleY = maxScaleY;
            scaleX = A / scaleY;
            //Debug.Log("!Jump&Fall! (" + scaleX + ", " + scaleY + ") " + velY);

        }
        else if (velY < 10.0f)
        {
            scaleY = startScaleY + (minScaleY - startScaleY) * Mathf.Abs(horizontal);
            scaleX = A / scaleY;
            //Debug.Log("!Ground! (" + scaleX + ", " + scaleY + ")");
        }
        transform.localScale = new Vector2(facingRight ? scaleX : -scaleX, scaleY);
    }

    private float magicSqueezeNumber, squeezeYScaleFrom, minScaleY = 0.75f;

    void calcLandingsqueezeY()
    {
        squeezeYScaleFrom = transform.localScale.y;
        landingTimeFrame = squeezeYScaleFrom * 0.5f;
        endLandingAt = Time.time + landingTimeFrame;
        Debug.Log("Squeeze Y From: " + squeezeYScaleFrom + " Landingtime: " + landingTimeFrame);

        float minY = 2*startScaleY - squeezeYScaleFrom;
        float l = landingTimeFrame;
        float q = squeezeYScaleFrom - startScaleY;
        float r = minY - squeezeYScaleFrom;
        float X = 0.5f;

        float a = (q * X - r * l) / (Mathf.Pow(l, 2.0f) * X - Mathf.Pow(X, 2.0f) * l);
        float b = (q - a * Mathf.Pow(l, 2.0f)) / l;

        squeezeFunctionY = new Vector2(a, b);
    }

    private void HandleAnimation()
    {
        anim.SetBool("grounded", grounded);
        anim.SetFloat("horizontal", controlHorizontal? Mathf.Abs(horizontal): 0.0f);
        anim.SetBool("walled", walled);
        anim.SetBool("onWall", wallGrab || sliding || onLedge);
        anim.SetBool("climbing", climbing);
        anim.SetBool("ducking", ducking);
        anim.SetBool("onLedge", onLedge);

        anim.SetFloat("VeloY", rb.velocity.y);
    }


    //math functions
    public float linearIncrease(float time, float maxSpeed, float maxSpeedTime)
    {
        return time * (maxSpeed / maxSpeedTime);
    }

    public float linearDecrease(float time, float minSpeed ,float beginningSpeed, float timeFrame)
    {
        if (time < timeFrame) return beginningSpeed - time * (beginningSpeed / timeFrame);
        else return minSpeed;
    }

    public float circularIncrease(float time, float maxSpeed)
    {
        return Mathf.Abs(Mathf.Sqrt(2.0f * maxSpeed * time + time * time));
    }

    public float circularDecrease(float time, float maxSpeed)
    {
        return maxSpeed - Mathf.Abs(Mathf.Sqrt(2.0f * maxSpeed * time + time * time));
    }

    public float limitedIncrease(float time, float maxSpeed, float increaseRate)
    {
        return maxSpeed - maxSpeed * Mathf.Exp(-increaseRate * time);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer.Equals(whatIsLedge))
            Debug.Log(collision.gameObject.name);
    }
}