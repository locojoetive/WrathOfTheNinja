using UnityEngine;

public class SecurityGuardMovement : MonoBehaviour {

    private Rigidbody2D rb;
    private Animator animator;
    private SecurityWatch watchScript;

    public bool 
        facingRight = true,
        cautious = false,
        playerDetected = false,
        staying = false;

    public float
        speed,
        tStayFor,
        tWalkFor,
        tStayAt = 0.0f,
        tWalkAt = 0.0f;

    void Start () {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        watchScript = GetComponentInChildren<SecurityWatch>();
        tStayAt = Time.time + tWalkFor;
        tWalkAt = tStayAt + tStayFor;
    }

	void Update () {
        if (animator) animator.SetBool("staying", rb.velocity.magnitude < 0.1f);
    }

    private void FixedUpdate()
    {
        if (watchScript)
        {
            playerDetected = watchScript.isPlayerDetected();
        }
        // HandleStaying();
        HandleMovement();
    }

    void HandleMovement()
    {
        if (Mathf.Abs(speed) > 0F) { 
            if (!staying && Time.time > tStayAt)
            {
                tWalkAt = Time.time + tStayFor;
                staying = true;
            } else if (staying && Time.time > tWalkAt)
            {
                tStayAt = Time.time + tWalkFor;
                staying = false;
            }

            if (!staying)
            {
                rb.velocity = new Vector2(speed, rb.velocity.y);
            }
        }
    }

    public void TurnAround()
    {
        facingRight = !facingRight;
        speed = facingRight ? Mathf.Abs(speed) : -Mathf.Abs(speed);
        transform.localScale = new Vector3(
            facingRight ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x),
            transform.localScale.y,
            transform.localScale.z
        );
    }
}
