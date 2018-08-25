using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour {
    private bool playerDetected;
    private SecurityWatch watchScript;
    public GameObject bullet;

	void Start () {
        watchScript = GetComponent<SecurityWatch>();
	}
	
	// Update is called once per frame
	void Update () {

	}
}
