using System;
using UnityEngine;

public class FliegerMove : MonoBehaviour
{
    public float speed;
    private Rigidbody2D physicalBody;


    internal void ShootInDirection(Vector3 localScale)
    {
        speed = localScale.x > 0F ? Mathf.Abs(speed) : -Mathf.Abs(speed);
        transform.localScale = new Vector3(
            localScale.x < 0F ? -transform.localScale.x : transform.localScale.x,
            transform.localScale.y,
            transform.localScale.z
        );
        GetComponent<Rigidbody2D>().AddForce(new Vector2(speed, 0F), ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
            collision.gameObject.GetComponent<NinjaStatesAnimationSound>().KillNinja();
        else Destroy(gameObject, 1F);
    }
}
