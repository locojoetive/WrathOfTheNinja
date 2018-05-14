using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityWatchScript : MonoBehaviour {

    Vector2 upperBorder;
    public bool cf = false;

    public float maxDepth = 30;
    private float step = 0.0f;
    private int numberRays = 8;
    public float alphaRange = 0.0f;
    private float alpha;
    public bool playerDetected = false;

    List<Vector2> ray = new List<Vector2>();
    Transform target;
    public GameObject rayRenderer;
    public List<LineRenderer> spotLight;


	// Use this for initialization
	void Start ()
    {
        initFOV();
        initRays();

        initSpotLight();

        target = GameObject.FindGameObjectWithTag("Player").transform;
    }
    
    // Update is called once per frame
    void Update () {
        if (cf)
        {
            initFOV();
            initRays();
            cf = false;
        }
        DrawRayAndSpotLight();
    }
    void initFOV()
    {

        if (alphaRange <= 0.1f)
        {
            upperBorder = (Vector2)transform.localScale;
            upperBorder.Normalize();
            alpha = Mathf.Rad2Deg * Mathf.Asin(upperBorder.y);
        }
        else alpha = Mathf.Abs(alphaRange);

        step = 2 * alpha / numberRays;
    }

    void initRays() {

        ray = new List<Vector2>();

        for (int i = 0; i <= numberRays; i++)
        {
            float x = Mathf.Cos(alpha * Mathf.Deg2Rad);
            float y = Mathf.Sin(alpha * Mathf.Deg2Rad);
            Vector2 r = new Vector2(x, y);
            r.Normalize();
            ray.Add(r);
            alpha -= step;
        }
        
    }

    public void DrawRayAndSpotLight()
    {

        bool detected = false;
        List<Vector2> hitpoint = new List<Vector2>();
        for (int i = 0; i <= numberRays; i++)
        {
            spotLight[i].SetPosition(0, transform.position);
            Vector3 r = ray[i]; //this is local space
            RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position, transform.TransformDirection(r), maxDepth);

            if (hit.point != null && hit.point != Vector2.zero)
            {
                r = transform.InverseTransformPoint(hit.point);
                if (hit.collider.tag == "Player")
                {
                    detected = true;
                    Debug.DrawRay(transform.position, transform.TransformVector(r), Color.red);
                    spotLight[i].SetPosition(1, transform.TransformPoint(r));
                }
                else
                {
                    Debug.DrawRay(transform.position, transform.TransformVector(r), Color.blue);
                    spotLight[i].SetPosition(1, transform.TransformPoint(r));
                }
            }
            else
            {
                Debug.DrawRay(transform.position, maxDepth * transform.TransformDirection(r), Color.green);
                spotLight[i].SetPosition(1, transform.position + maxDepth * transform.TransformDirection(r));
            }
        }
        playerDetected = detected;
    }

    void initSpotLight()
    {
        spotLight = new List<LineRenderer>();

        for (int i = 0; i <= numberRays; i++)
        {
            GameObject rei = Instantiate(rayRenderer, transform.position, transform.rotation, transform);

            LineRenderer lr = rei.GetComponent<LineRenderer>();
            lr.SetPosition(0, this.transform.position);
            spotLight.Add(lr);

        }
    }
    

    void updateRays()
    {
        for (int i = 0; i < numberRays; i++)
        {
            Vector2 r = ray[i];

            RaycastHit2D hit = Physics2D.Raycast(transform.position, r, maxDepth);

            if (hit.point != null && hit.point != Vector2.zero)
            {
                r = hit.point - (Vector2)transform.position;
                if (hit.collider.tag == "Player")
                {
                    Debug.DrawRay(transform.position, transform.TransformDirection(r), Color.red);
                }
                else Debug.DrawRay(transform.position, transform.TransformDirection(r));
            }
        }
    }

    public bool getDetectionStatus()
    {
        return playerDetected;
    }
    
    public void SkipTrigger()
    {
        //This may be usefull to skip trigger collider
     /*if (hit.collider.gameObject.tag == "ledge")
     {
         foreach (RaycastHit2D h in (Physics2D.RaycastAll(transform.position, r, maxDepth))){
             if (h.collider.gameObject.tag != "ledge")
             {
                 hit = h;
                 contactPoint = hit.point;
             }    
         }
         r = contactPoint - transform.position;
         Debug.DrawRay(transform.position, r);
     }
     else
     {*/
    }

}