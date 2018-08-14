using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityWatchScript : MonoBehaviour {

    public bool cf = false;
    // FOV
    private float angle;
    public Vector2 scale;
    public float hypothenuse = 0.0f;
    public float maxDepth;
    public int numberRays = 15;
    Vector2[] detectionRay;
    
    Transform target;
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
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }
    
    // Update is called once per frame
    void Update () {
        initFOV();
        initRays();

        if (cf)
        {
            initFOV();
            initRays();
            initSpotLight();
            cf = false;
        }
        DrawRayAndSpotLight();
    }
    void initFOV()
    {
        
        //upperBorder.Normalize();
        hypothenuse = Mathf.Sqrt(scale.x * scale.x + scale.y * scale.y);
        float sinAngle = Mathf.Abs(scale.y) / hypothenuse;
        angle = Mathf.Rad2Deg * Mathf.Asin(sinAngle);
    }

    void initRays() {

        detectionRay = new Vector2[numberRays];
        float step = 2*angle / numberRays;
        float alpha = angle;

        for (int i = 0; i < numberRays; i++)
        {
            float x = Mathf.Cos(alpha*Mathf.Deg2Rad);
            float y = Mathf.Sin(alpha*Mathf.Deg2Rad);
            Vector2 direction = new Vector2(x, y);
            direction.Normalize();
            detectionRay[i] = direction;
            Debug.Log(alpha + " deg with Ray " + i + ":" + detectionRay[i]);
            alpha -= step;
        }
    }

    void initSpotLight()
    {
        spotLight = new List<LineRenderer>();
        for (int i = 0; i < numberRays; i++)
        {
            GameObject rei = Instantiate(rayRenderer, transform.position, transform.rotation, transform);
            LineRenderer lr = rei.GetComponent<LineRenderer>();
            lr.SetPosition(0, (Vector2)transform.position + detectionRay[i]);
            spotLight.Add(lr);
        }
    }

    public void DrawRayAndSpotLight()
    {
        bool detected = false;
        for (int i = 0; i < numberRays; i++)
        {
            // spotLight[i].SetPosition(0, transform.position);
            Vector2 direction = (Vector2)transform.InverseTransformDirection(detectionRay[i]) 
                * maxDepth;
            RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position, direction, maxDepth, layerMask.value);
            if (hit.transform != null)
            {
                direction = (Vector3)hit.point - transform.position;
                Debug.DrawRay(transform.position, direction, Color.red);
            } else Debug.DrawRay(transform.position, direction*maxDepth, Color.green);
        }
    }

    public bool getDetectionStatus()
    {
        return playerDetected;
    }
    
    public void SkipTrigger()
    {

    }

}