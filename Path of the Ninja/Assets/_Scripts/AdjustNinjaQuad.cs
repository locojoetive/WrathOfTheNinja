using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustNinjaQuad : MonoBehaviour {

    Transform player;

	// Use this for initialization
	void Start () {
        player = GetComponentInChildren<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
		player.localPosition = Vector2.up * player.localScale.y * 0.5f;
	}
}
