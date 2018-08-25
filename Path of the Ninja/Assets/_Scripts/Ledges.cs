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
        Vector3 ledgeExtent = transform.InverseTransformDirection(collider.bounds.extents);
        ledges[0].position = transform.position + ledgeExtent;
        ledges[1].position = transform.position + Vector3.Scale(ledgeExtent, new Vector3(-1, 1, 1));
    }

    public void adjustScale()
    {
        Vector3 transformedScale = new Vector3(
            1F / transform.localScale.x,
            1F / transform.localScale.y,
            1F / transform.localScale.z
        );
        ledges[0].localScale = transformedScale;
        ledges[1].localScale = transformedScale;
    }
}
