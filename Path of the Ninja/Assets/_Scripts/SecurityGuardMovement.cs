using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityGuardMovement : MonoBehaviour {

    public float speed = 0.0f, stayTime, turnfrequency;
    private float nextTurn = 0.0f;
    public bool facingRight;
    private Rigidbody2D rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        HandleMovement();
        HandleTurnAround();
	}

    void HandleMovement()
    {
        rb.velocity = facingRight? speed * Vector2.right : -speed * Vector2.right;
    }

    void HandleTurnAround()
    {

    }
}
