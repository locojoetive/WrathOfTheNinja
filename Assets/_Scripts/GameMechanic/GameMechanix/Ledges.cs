using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*To change size of the hurdle the positions of all child objects 
 * must be (0, 0, 0) and only the scale of the Quadtransform (also 
 * child object) must be changed*/

public class Ledges : MonoBehaviour {
    private List<Transform> ledges;
    private new BoxCollider2D collider;

	void Start() {
        initializeComponents();
        setPositions();
        adjustScale();
    }

    private void Update()
    {
        adjustScale();
    }

    public void initializeComponents()
    {
        ledges = new List<Transform>();
        collider = GetComponent<BoxCollider2D>();
        foreach (Transform ledge in transform)
            ledges.Add(ledge);
    }

    public void setPositions()
    {
        Vector3 ledgeExtent = Vector2.zero;

        if (ledges.Count == 2)
        {
            ledgeExtent = new Vector3(0.5f, 0.5f, 1F);
            ledges[0].localPosition = ledgeExtent;
            ledgeExtent.x *= -1F;
            ledges[1].localPosition = ledgeExtent;
        } else
        {
            ledgeExtent = new Vector3(0F, 0.5f, 1F);
            ledges[0].localPosition = ledgeExtent;
        }
    }

    public void adjustScale()
    {
        Vector3 transformedScale = SimpleMath.PointwiseDivide(Vector3.one, transform.localScale);
        if (ledges.Count == 2)
        {
            ledges[0].localScale = transformedScale;
            ledges[1].localScale = transformedScale;
        } else ledges[0].localScale = new Vector3(1F ,transformedScale.y, 1F) ;
    }
}
