using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
