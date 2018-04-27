using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Cameras Position MUST be (0, 0, 0)!!!

public class FollowPlayer : MonoBehaviour {

    private Transform player;
    private Vector2 currentVelocity = Vector2.zero;
    public float smoothTime = 0.05f, maxSpeed = 20, velocity = 0.0F;
    public float zPosition = -17F;
    public float maxX = 0.0f, maxY = 0.0f;
    public Vector3 stageDimensions = Vector3.zero;
    Vector2 topRightEdgeVector;
    private Bounds cameraBounds, bounds;
    
    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        stageDimensions = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        
        topRightEdgeVector = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
        bounds = GameObject.FindGameObjectWithTag("bgLayer0").GetComponent<SpriteRenderer>().sprite.bounds;

        Debug.Log("Cameraborder: " + cameraBounds);
        Debug.Log("Sprites: " + bounds);
    }
    
    
    void FixedUpdate()
    {
        Debug.Log("Cameraborder: " + topRightEdgeVector);
        Debug.Log("Sprites: " + bounds);

        float interpolation = maxSpeed * Time.deltaTime;

        Vector3 position = this.transform.position;
        position.y = Mathf.Lerp(this.transform.position.y, player.position.y, interpolation);
        position.x = Mathf.Lerp(this.transform.position.x, player.position.x, interpolation);



        float left = -bounds.extents.x + topRightEdgeVector.x;
        float right = bounds.extents.x - topRightEdgeVector.x;

        float bottom = -bounds.extents.y + topRightEdgeVector.y;
        float up = bounds.extents.y - topRightEdgeVector.y;

        Debug.Log("Left Border " + left);

        if (position.x < left) position.x = left;
        else if (position.x > right) position.x = right;

        if (position.y < bottom) position.y = bottom;
        else if (position.y > up) position.y = up;

        transform.position = position;
    }
    
}
