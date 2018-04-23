using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*To change size of the hurdle the positions of all child objects 
 * must be (0, 0, 0) and only the scale of the Quadtransform (also 
 * child object) must be changed*/

public class LedgesScript : MonoBehaviour {
    public List<Transform> children ;

	// Use this for initialization
	void Start () {
		foreach(Transform child in transform)
        {
            children.Add(child);
        }
        float posX = 0.5f * children[0].localScale.x;
        float posY = 0.5f * children[0].localScale.y;

        children[1].localPosition = new Vector2(posX, posY);
        children[2].localPosition = new Vector2(-posX, posY);
    }

    // Update is called once per frame
    void Update () {

	}
}
