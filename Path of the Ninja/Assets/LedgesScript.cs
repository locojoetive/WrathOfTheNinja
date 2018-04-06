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
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
