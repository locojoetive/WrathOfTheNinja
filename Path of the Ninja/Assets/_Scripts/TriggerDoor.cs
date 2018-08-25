using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDoor : MonoBehaviour {

    bool open = true;
    public List<SecurityWatch> list;
    BoxCollider2D col;
	// Use this for initialization
	void Start () {
        foreach (GameObject s in GameObject.FindGameObjectsWithTag("security"))
        {
            list.Add(s.GetComponent<SecurityWatch>());
        }

        col = GetComponent<BoxCollider2D>();
	}
	
	// Update is called once per frame
	void Update () {
        bool closeDoor = false;

		foreach(SecurityWatch s in list)
        {
            if (s.getDetectionStatus()) closeDoor = true;
        }

        open = !closeDoor;

        if (open) col.isTrigger = true;
        else col.isTrigger = false;
    }
}
