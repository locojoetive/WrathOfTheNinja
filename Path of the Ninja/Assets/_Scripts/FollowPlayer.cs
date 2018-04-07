using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {

    private Transform player;
    private Vector2 currentVelocity = Vector2.zero;
    public float smoothTime = 0.05f, maxSpeed = 20, velocity = 0.0F;
    public float zPosition = -17F;
    public float maxX = 0.0f, maxY = 0.0f;
    public Vector3 stageDimensions = Vector3.zero;
    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        stageDimensions = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
    }
    
    
    void Update()
    {
        float interpolation = maxSpeed * Time.deltaTime;

        Vector3 position = this.transform.position;
        position.y = Mathf.Lerp(this.transform.position.y, player.position.y, interpolation);
        position.x = Mathf.Lerp(this.transform.position.x, player.position.x, interpolation);

        this.transform.position = position;
    }
}
