using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaMove : MonoBehaviour {
    private bool facingRight = true;

    //Unity Components
    private Rigidbody2D rb;
    private Transform tr;
    private BoxCollider2D col;
    private Animator anim;
    
    //Floor & Hit Detection
    private LayerMask whatIsGround;
    private LayerMask whatIsLedge;
    public Transform groundCheckFront;
    public Transform groundCheckBack;
    public Transform wallCheckUp;
    public Transform wallCheckDown;
    public Transform wallCheckMid;
    public Transform landingCheck;
    public Transform ledgeCheck;
    private float boxSize = 0.005f;
    private float slidingTime =0.0f;
    public float translation;

    //Movement
    private int combo = 0;
    private float horizontal, vertical;
    public float walkingSpeed = 5F, runningSpeed=0.0f, jumpHeight=3F, climbingSpeed=1.5f, slidingSpeed=5F;
    private bool grounded = false, running = false, ducking = false, walking = true, wallGrab = false;
    private bool paused = false, climbing = false, controls = true, sliding = false;
    public bool walled = false, walledUp = false, walledDown = false, walledMid = false, gF = false, gB = false, onLedge = false, slipping = false, landing = false, onWall = false;

    //Extended Movement

    //Jump
    private float tapTime = 0.2f, lastTap = 0.0f, jumpTime = 0.0f, jumpTimeFrame = 0.2f;
    private int jumpNo = 0, allowedJumps = 1;
    private float fallMultiplier = 2.5f;
    private float lowJumpMultiplier = 2f;

    // Use this for initialization
    void Start()
    {
        //components for manupilation
        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<Transform>();
        col = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();

        //Time.timeScale = 0.1f;
        //initialize ground tags & sensors positions
        whatIsGround.value = 1 << 8;
        whatIsLedge.value = 2 << 8;
        /*//initialize consistent player size
        startScaleX = transform.localScale.x;
        startScaleY = transform.localScale.y;
        A = startScaleX * startScaleY;*/
    }

    // Update is called once per frame
    void Update()
    {
        HandleSensors();
        HandleState();
        HandleMovement();
        HandleJumpAndFall();
        HandleClimbing();
        HandleAnimation();
        
    }

    private void FixedUpdate()
    {
        changeFacing();
        //changeForm();
    }

    private void HandleSensors()
    {
        groundCheckFront.localPosition = new Vector2(col.offset.x + 0.25f * col.size.x, col.offset.y - 0.5f * col.size.y);
        groundCheckBack.localPosition = new Vector2(col.offset.x - 0.25f * col.size.x, col.offset.y -0.5f * col.size.y);
        wallCheckUp.localPosition = new Vector2(col.offset.x + 0.5f * col.size.x, col.offset.y + 0.25f * col.size.y);
        wallCheckDown.localPosition = new Vector2(col.offset.x + 0.5f * col.size.x, col.offset.y - 0.25f * col.size.y);
        wallCheckMid.localPosition = new Vector2(col.offset.x + 0.5f * col.size.x, col.offset.y);

        ledgeCheck.localPosition = new Vector2(col.offset.x + 0.5f * col.size.x, col.offset.y + 0.125f * col.size.y);
        landingCheck.localPosition = new Vector2(col.offset.x + 0.5f * col.size.x + 0.5f * boxSize, col.offset.y - 0.25f * col.size.y);
    }

    private void HandleState()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        //get active sensors
        gF = Physics2D.OverlapBox(groundCheckFront.position, new Vector2(0.5f * col.size.x , boxSize), 0.0f, whatIsGround.value);
        gB = Physics2D.OverlapBox(groundCheckBack.position, new Vector2(0.5f * col.size.x, boxSize), 0.0f, whatIsGround.value);
        grounded = gF && gB;

        //climbing
        walledUp = Physics2D.OverlapBox(wallCheckUp.position, new Vector2(boxSize, 0.5f * col.size.y - 0.015f), 0.0f, whatIsGround.value);
        walledDown = Physics2D.OverlapBox(wallCheckDown.position, new Vector2(boxSize, 0.5f * col.size.y - 0.015f), 0.0f, whatIsGround.value);
        walledMid = Physics2D.OverlapBox(wallCheckMid.position, new Vector2(boxSize, 0.5f * col.size.y), 0.0f, whatIsGround.value);
        walled = walledUp && walledDown;
        onLedge = Physics2D.OverlapBox(ledgeCheck.position, new Vector2(boxSize, 0.25f * col.size.y), 0.0f, whatIsLedge.value) && !grounded;
        onWall = walled && !grounded;
        
        //decides wether player can control rigidbody
        controls = !onLedge
            && !(!walledUp && walledDown && !grounded) && !(walledUp && !walledDown && !grounded); //nicht an Kanten verkeilen

        ducking = grounded && vertical < -0.5f;

        landing = gF && !gB && !walledDown && !walledUp;
        slipping = !gF && gB && !walledDown;


        //wall behaviour
        wallGrab = onWall
            && (facingRight && Input.GetKey(KeyCode.RightArrow) || !facingRight && Input.GetKey(KeyCode.LeftArrow));
        climbing = wallGrab && vertical > 0.1f;
        sliding = (!wallGrab && onWall && !climbing) || (!walledUp && walledDown && !gF);
    }

    private void HandleMovement()
    {
        if (controls)
        {
            rb.velocity = new Vector2(ducking? horizontal * walkingSpeed * 0.5f : horizontal * walkingSpeed, rb.velocity.y);
        }

        if (slipping)
        {
            rb.velocity = new Vector2(facingRight ? translation : -translation, 0.0f);
        } else if (landing)
        {
            rb.velocity = new Vector2(facingRight ? translation : -translation, rb.velocity.y);
        }
    }
    
    private void HandleJumpAndFall() {
        if (controls)
        {
            if (grounded && jumpNo != 0 && jumpTime < Time.time)
            {
                jumpNo = 0;
                //Form change variables at landing || squeezeing variables
                //calcLandingsqueezeY();
            }

            //initialize jump
            if (Input.GetKeyDown(KeyCode.Space) && jumpNo < allowedJumps)
            {
                jumpTime = Time.time + jumpTimeFrame;
                rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
                jumpNo++;
            }

            //adjust Jump & Fall Speed
            if (rb.velocity.y < 0)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
            else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
        }
    }

    public float radix = 1F;

    public void HandleClimbing()
    {
        if (onLedge &&  vertical > 0)
        {
            rb.velocity = Vector2.up * radix;
            Debug.Log("I'm ledge!");
        }
        else if (onWall && !onLedge)
        {
            if (sliding)
            {
                slidingTime += Time.deltaTime;
                if (slidingTime > 1F)
                    slidingTime = 1F;
                rb.velocity = new Vector2(facingRight ? 3F : -3F, -2F * slidingSpeed * slidingTime);
            }
            else {
                slidingTime = 0.0f;
                if (climbing)
                {
                    if (onLedge)
                    {
                    }
                    else
                    {
                        rb.velocity = new Vector2(0.0f, climbingSpeed * vertical);

                    }
                }
                else if (wallGrab) rb.velocity = Vector2.zero;
            }
        }
    }


    public void Pause()
    {
        controls = false;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
        Debug.Log("Yo Ninja!");
        paused = true;
    }

    public void Resume()
    {
        controls = true;
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
        if (!paused && controls && (facingRight && horizontal < 0 || !facingRight && horizontal > 0))
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
        anim.SetFloat("horizontal", Mathf.Abs(horizontal));
        anim.SetBool("walled", walled);
        anim.SetBool("climbing", climbing);
        anim.SetBool("ducking", ducking);
        anim.SetBool("sliding", sliding);
        anim.SetBool("onLedge", onLedge);

        anim.SetFloat("VeloY", rb.velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer.Equals(whatIsLedge))
            Debug.Log(collision.gameObject.name);
    }
}