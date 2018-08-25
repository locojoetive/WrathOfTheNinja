using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    private Rigidbody2D rb;
    public float speed;

	void Start () { rb = GetComponent<Rigidbody2D>(); }
	
	// Update is called once per frame
	void Update () {
        HandleMovement();		
	}

    void HandleMovement()
    {
        rb.velocity = Vector2.right * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.gameObject.tag == "Player")
        {
            collision.collider.gameObject.GetComponent<LazyTime>().InflictDamage();
        }
    }
}
