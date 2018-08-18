using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityWatchScript : MonoBehaviour {

    public bool cf = false;
    // FOV
    public float angle;
    public Vector2 scale;
    public float maxDepth;
    public int numberRays;
    Vector2[] detectionRays;
    
    public GameObject rayRenderer;
    public List<LineRenderer> spotLight;

    public LayerMask layerMask;
    public bool playerDetected = false;

    // Use this for initialization
    void Start ()
    {
        initFOV();
        initRays();
        // initSpotLight();
    }
    
    // Update is called once per frame
    void Update () {
        DrawRayAndSpotLight();
    }
    void initFOV()
    {
        float hypothenuse = Mathf.Sqrt(scale.x * scale.x + scale.y * scale.y);
        float sinAngle = Mathf.Abs(scale.y) / hypothenuse;
        angle = Mathf.Rad2Deg * Mathf.Asin(sinAngle);
    }

    void initRays() {

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

    void initSpotLight()
    {
        spotLight = new List<LineRenderer>();
        for (int i = 0; i < numberRays; i++)
        {
            GameObject rei = Instantiate(rayRenderer, transform.position, transform.rotation, transform);
            LineRenderer lr = rei.GetComponent<LineRenderer>();
            lr.SetPosition(0, (Vector2)transform.position + detectionRays[i]);
            spotLight.Add(lr);
        }
    }

    public void DrawRayAndSpotLight()
    {
        bool detected = false;
        for (int i = 0; i < numberRays; i++)
        {
            // spotLight[i].SetPosition(0, transform.position);
            Vector2 direction = (Vector2) transform.TransformDirection(detectionRays[i]);
            //direction.Normalize();
            RaycastHit2D hit = Physics2D.Raycast( transform.position, direction, maxDepth, layerMask.value);
            if (hit.transform != null) {
                direction = (Vector3) hit.point - transform.position;
                Debug.DrawRay(transform.position, direction, Color.red);
                detected = true;
            }
            else {
                Debug.DrawRay(transform.position, direction * maxDepth, Color.green);
            }
            Debug.DrawRay(transform.position, transform.right);
        }
        playerDetected = detected;
    }

    public bool getDetectionStatus()
    {
        return playerDetected;
    }
}