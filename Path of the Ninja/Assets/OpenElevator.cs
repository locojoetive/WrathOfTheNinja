using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenElevator : MonoBehaviour {
    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            animator.SetBool("opened", true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            animator.SetBool("opened", false);
        }

    }
}
