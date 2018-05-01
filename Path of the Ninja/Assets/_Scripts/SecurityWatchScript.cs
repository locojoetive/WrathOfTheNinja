using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityWatchScript : MonoBehaviour {

    Vector2 upperBorder;
    public bool cf = false;

    public float maxDepth = 30;
    public float step = 0.0f;
    public int numberRays = 15;
    public float alphaRange = 0.0f;
    private float alpha;
    public bool playerDetected = false;

    List<Vector2> ray = new List<Vector2>();
    List<Vector2> hitpoint = new List<Vector2>();
    Transform target;
    public LineRenderer[] spotLight;


	// Use this for initialization
	void Start ()
    {
        calcFOV();
        spotLight = GetComponentsInChildren<LineRenderer>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }
    
    // Update is called once per frame
    void Update () {
        if (cf)
        {
            calcFOV();
            cf = false;
        }
        DrawRay();
        setSpotLight();
    }

    void setSpotLight()
    {
        spotLight[0].SetPosition(0, transform.position);
        spotLight[0].SetPosition(1, hitpoint[0]);

        spotLight[1].SetPosition(0, transform.position);
        spotLight[1].SetPosition(1, hitpoint[1]);

        spotLight[2].SetPosition(0, transform.position);
        spotLight[2].SetPosition(1, hitpoint[2]);

        spotLight[3].SetPosition(0, transform.position);
        spotLight[3].SetPosition(1, hitpoint[3]);

        spotLight[4].SetPosition(0, transform.position);
        spotLight[4].SetPosition(1, hitpoint[4]);
        Debug.Log(hitpoint.Count);
    }

    void calcFOV()
    {

        upperBorder = (Vector2)transform.localScale;
        upperBorder.Normalize();
        ray = new List<Vector2>();
        hitpoint = new List<Vector2>();

        alpha = alphaRange == 0F ? Mathf.Asin(upperBorder.y) : Mathf.Abs(alphaRange);

        step = 2 * alpha / numberRays;

        for (int i = 0; i < numberRays; i++)
        {
            float x = Mathf.Cos(alpha * Mathf.Deg2Rad);
            float y = Mathf.Sin(alpha * Mathf.Deg2Rad);
            ray.Add(new Vector2(x, y));
            alpha -= step;
        }
    }

    public bool getDetectionStatus()
    {
        return playerDetected;
    }

    public void DrawRay()
    {

        bool detected = false;
        hitpoint = new List<Vector2>();
        for (int i = 0; i < numberRays; i++)
        {

            Vector3 r = transform.TransformDirection(maxDepth * ray[i]);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, r, maxDepth);
            if (hit.point != null && hit.point != Vector2.zero)
            {
                r = hit.point;
                if (hit.collider.tag == "Player")
                {
                    detected = true;
                    Debug.DrawRay(transform.position, r - transform.position, Color.red);
                }
                else Debug.DrawRay(transform.position, r - transform.position);

            }
            else Debug.DrawRay(transform.position, maxDepth * r);
            
            hitpoint.Add(r);
        }
        playerDetected = detected;
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