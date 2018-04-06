using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {

    private Transform player;
    private Vector2 currentVelocity = Vector2.zero;
    public float smoothTime = 0.05f, maxSpeed = 20, velocity = 0.0F;
    public float zPosition = -17F;

    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player").transform;
	}
    
    /*void LateUpdate()
    {
            //transform.position = Vector2.SmoothDamp(transform.position, player.position, ref currentVelocity, smoothTime);//new Vector2(player.position.x, player.position.y);
            
            Vector2 newPosition = Vector2.SmoothDamp(transform.position, player.position, ref currentVelocity, smoothTime, maxSpeed, Time.deltaTime);
            transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
    }*/ 
    
    void Update()
    {
        float interpolation = maxSpeed * Time.deltaTime;

        Vector3 position = this.transform.position;
        position.y = Mathf.Lerp(this.transform.position.y, player.position.y, interpolation);
        position.x = Mathf.Lerp(this.transform.position.x, player.position.x, interpolation);

        this.transform.position = position;
    }
}
