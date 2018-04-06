using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFDMoveScript : MonoBehaviour {

    private bool moving = false, facingRight = true;



    //Unity Components
    private Rigidbody rb;
    private Transform tr;
    private Animator anim;
    private BoxCollider col;
 
    //Floor & Hit Detection
    private LayerMask whatIsGround;
    public Transform groundCheck;
    public List<Transform> groundCheckList;
    public Transform wallcheck;
    public Transform hitCheck;

    //Movement
    private int combo = 0;
    private float horizontal, vertical, vertVel;
    public float speed, jumpHeight;
    public bool grounded = false, walled = false;
    public bool g0 = false, g1 = false, g2 = false, g3 = false;
    public float boxSize = 0.1f;
    //Jump
    private float tapTime = 0.2f, lastTap = 0.0f, jumpTime = 0.0f, jumpTimeFrame = 0.5f;
    private int jumps = 0, allowedJumps = 1;


    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        tr = GetComponent<Transform>();
        anim = GetComponent<Animator>();
        col = GetComponent<BoxCollider>();
        
        whatIsGround.value = 1 << 8;

        groundCheck.localPosition = new Vector3(0.0f, -0.5f * col.size.y, 0.0f);
        groundCheck.GetChild(0).localPosition = new Vector3(0.5f * col.size.x - 0.5f*boxSize, 0.0f, 0.5f * col.size.z - 0.5f * boxSize);
        groundCheck.GetChild(1).localPosition = new Vector3(-0.5f * col.size.x + 0.5f * boxSize, 0.0f, 0.5f * col.size.z - 0.5f * boxSize);
        groundCheck.GetChild(2).localPosition = new Vector3(0.5f * col.size.x - 0.5f * boxSize, 0.0f, -0.5f * col.size.z + 0.5f * boxSize);
        groundCheck.GetChild(3).localPosition = new Vector3(-0.5f * col.size.x + 0.5f * boxSize, 0.0f, -0.5f * col.size.z + 0.5f * boxSize);
        
        wallcheck.localPosition = new Vector3(col.size.x, 0.0f, 0.0f);
    }

    // Update is called once per frame
    void Update () {
        
        HandleMovement();
        HandleJumpAndFall();
        HandleAnimation();
    }

    private void HandleMovement()
    {
        g0 = Physics.CheckBox(
            groundCheck.GetChild(0).position,
            new Vector3(boxSize, 0.0001f, boxSize),
            transform.rotation,
            whatIsGround
        );
        g1 = Physics.CheckBox(
            groundCheck.GetChild(1).position,
            new Vector3(boxSize, 0.00001f, boxSize),
            transform.rotation,
            whatIsGround
        );
        g2 = Physics.CheckBox(
             groundCheck.GetChild(2).position,
             new Vector3(boxSize, 0.000001f, boxSize),
             transform.rotation,
             whatIsGround
         );
        g3 = Physics.CheckBox(
             groundCheck.GetChild(3).position,
             new Vector3(boxSize, 0.1f, boxSize),
             transform.rotation,
             whatIsGround
         );
        grounded = (g0 && g1 && g2) || (g0 && g2 && g3)  || (g0 && g1 && g3) || (g1 && g2 && g3);
        /*grounded = Physics.CheckBox(
            groundCheck.position + new Vector3(0.0f, -0.05f, 0.0f),
            new Vector3(GetComponent<BoxCollider>().size.x * 0.25f, 0.1f, GetComponent<BoxCollider>().size.z * 0.25f),
            transform.rotation,
            whatIsGround
        );
        */
        walled = !grounded && Physics.CheckBox(
            wallcheck.position,
            new Vector3(GetComponent<BoxCollider>().size.x * 0.25f, 0.5f* GetComponent<BoxCollider>().size.y, GetComponent<BoxCollider>().size.z * 0.25f),
            transform.rotation,
            whatIsGround
        );

        if (facingRight) transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        else transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);

        if (combo == 0)
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            if (Mathf.Abs(horizontal) + Mathf.Abs(vertical) != 0) moving = true;
            else moving = false;

            anim.SetBool("moving", moving);

            if (Input.GetButton("Fire3") && Mathf.Abs(horizontal) > 0)
            {
                transform.Rotate(new Vector3(0.0f, horizontal, 0.0f));
            }
            else if (grounded)
            {
                rb.velocity = new Vector3(0.0f, rb.velocity.y, 0.0f) + (transform.forward * vertical + transform.right * horizontal) * speed;
            }
            else if (!grounded)
            {
                rb.velocity = new Vector3(0.0f, rb.velocity.y, 0.0f) + (transform.forward * vertical + transform.right * horizontal) * speed * 0.5f;
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (Time.time < lastTap)
                {
                    facingRight = false;
                }
                lastTap = Time.time + tapTime;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (Time.time < lastTap)
                {
                    facingRight = true;
                }
                lastTap = Time.time + tapTime;
            }
        }
    }

    private void HandleJumpAndFall()
    {
        if (grounded)
        {
            jumps = 0;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpTime = Time.time + jumpTimeFrame;
            if (jumps < allowedJumps)
                rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
        }

        if (Input.GetButton("Jump"))
        {
            if (jumps < allowedJumps && Time.time < jumpTime)
            {
                rb.AddForce(3 * Vector3.up * jumpHeight, ForceMode.Force);
            }
            combo = 0;
            anim.SetInteger("Attack", combo);
            Debug.Log("JUMP" + jumps);
        }
        else if (Input.GetButtonUp("Jump"))
        {
            jumps++;
        }

        // provides sliding on walls
        else if (!grounded)
        {
            if (!g0 && g1 && !g2 && g3) rb.velocity = 3F * tr.right + (-0.5f) * tr.up;
            else if (g0 && !g1 && g2 && !g3) rb.velocity = -3F * tr.right + (-0.5f) * tr.up;
            else if (!g0 && !g1 && g2 && g3) rb.velocity = -3F * tr.forward + (-0.5f) * tr.up;
            else if (!g0 && g1 && !g2 && g3) rb.velocity = 3F * tr.forward + (-0.5f) * tr.up;
        }
    }


    void HandleAnimation()
    {
        vertVel = rb.velocity.y;
        
        //Walking Adjusment, in case horizontal + vertical == 0
        float walkingSpeed = 0.5f * (horizontal + vertical);

        if (horizontal + vertical == 0 && horizontal != 0)
        {
            walkingSpeed += 0.05f;
        }        
        ////////////////////////////////////////////////////////

        anim.SetFloat("VerticalVel", vertVel);
        if (facingRight) anim.SetFloat("Speed", walkingSpeed);
        else anim.SetFloat("Speed", -walkingSpeed);
        anim.SetBool("Grounded", grounded);
        anim.SetBool("Walled", walled);
    }

    void Jump()
    {
        rb.velocity = rb.velocity + (Vector3.up * jumpHeight);
    }
    

    //returns a value between start and end for movement purpose
    /*float getRangeProportion(float start, float end, int flag)
    {
        switch(flag)
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            default:
                break;
    }*/
}
