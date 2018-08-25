using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityWatch : MonoBehaviour {
    public float angle;
    public Vector2 scale;
    public float maxDepth;
    public int numberRays;
    Vector2[] detectionRays;
    public LayerMask layerMask;
    public bool playerDetected = false;
    
    void Start ()
    {
        initFOV();
        initRays();
    }

    void Update () {
        DrawRayAndSpotLight();
    }

    private void initFOV()
    {
        float hypothenuse = Mathf.Sqrt(scale.x * scale.x + scale.y * scale.y);
        float sinAngle = Mathf.Abs(scale.y) / hypothenuse;
        angle = Mathf.Rad2Deg * Mathf.Asin(sinAngle);
    }

    private void initRays() {

        detectionRays = new Vector2[numberRays];
        float step = 2 * angle / numberRays;
        float alpha = -angle;
        for (int i = 0; i < numberRays; i++)
        {
            float x = Mathf.Cos(alpha * Mathf.Deg2Rad); // * scale.magnitude;
            float y = Mathf.Sin(alpha * Mathf.Deg2Rad); // * scale.magnitude;
            Vector2 direction = new Vector2(x, y);
            detectionRays[i] = direction;
            alpha += step;
        }
    }
    
    private void DrawRayAndSpotLight()
    {
        bool detected = false;
        for (int i = 0; i < numberRays; i++)
        {
            Vector2 direction = ( Vector2 ) transform.TransformDirection(detectionRays[i]);
            RaycastHit2D hit = Physics2D.Raycast( transform.position, direction, maxDepth, layerMask.value);
            if (hit.transform != null) {
                direction = hit.point - (Vector2)transform.position;
                if (hit.transform.tag == "Player") {
                    Debug.DrawRay(transform.position, direction, Color.red);
                    detected = true;
                }
                else
                {
                    Debug.DrawRay(transform.position, direction, Color.red);
                }
            }
            else
            {
                Debug.DrawRay( transform.position, direction * maxDepth, Color.green );
            }
            Debug.DrawRay( transform.position, transform.right );
        }
        playerDetected = detected;
    }

    public bool getDetectionStatus()
    {
        return playerDetected;
    }
}