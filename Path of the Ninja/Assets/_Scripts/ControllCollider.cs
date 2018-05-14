 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//from: hiphish
//https://forum.unity.com/threads/layers-collision-and-one-way-platforms-a-question.71790/

public class ControllCollider : MonoBehaviour {


    void OnTriggerEnter2D(Collider2D jumper)
    {
        //make the parent platform ignore the jumper
        
        Debug.Log("Yo " + jumper.gameObject.name + "!");
        Physics2D.IgnoreCollision(jumper, GetComponent<EdgeCollider2D>());
    }

    void OnTriggerExit2D(Collider2D jumper)
    {
        Debug.Log("Yo " + jumper.gameObject.name + "!");
        Physics2D.IgnoreCollision(jumper, GetComponent<EdgeCollider2D>(), false);
    }

}
