using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazyTime : MonoBehaviour {

    Animator anim;
    Rigidbody2D rb;
    public int count = 0;
    public bool lazyTime = false, reallyLazy = false;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        lazyTime = Mathf.Abs(rb.velocity.x) < 0.000001f
            && Mathf.Abs(rb.velocity.y) < 0.000001f;

        if (lazyTime)
        {
            count--;
        }
        else count = 0;


        reallyLazy = count < -1000;

        if (count > 0)
        {
            //reallyLazy = true;
            //anim.SetBool("lazy", true);
        }


	}
}
