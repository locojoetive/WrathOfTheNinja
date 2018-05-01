using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCameraMovement : MonoBehaviour {

    public Vector3 originRotation;
    private Quaternion upperBorder, bottomBorder;
    public float angle = 30;
    public float timeFrameConfirmation=2F, maxSpeed = 2F;
    public bool cr = false, goingDown = true, detectionOn = false;
    private SecurityWatchScript moveScript;
    public Transform target;

    //Use this for initialization
	void Start () {
        calcRange();
        moveScript = GetComponent<SecurityWatchScript>();
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update() {
        if (cr)
        {
            calcRange();
            cr = false;
        }
        CheckForPlayer();

        if (detectionOn)
        {
            FollowPlayer();
        }
        else Rotate();
	}

    void FollowPlayer()
    {
        Vector2 reference = target.position - transform.position;
        reference = new Vector2(-reference.y, reference.x);
        
        transform.rotation = Quaternion.LookRotation(Vector3.forward, reference);
    }

    void CheckForPlayer()
    {
        detectionOn = moveScript.getDetectionStatus();

    }

    void Rotate()
    {
        if (transform.eulerAngles.z <= originRotation.z - angle) goingDown = false;
        else if (transform.eulerAngles.z >= originRotation.z + angle) goingDown = true;
        
        float interpolation = maxSpeed * Time.deltaTime;
        float rotation = 0;

        if (goingDown) rotation = Mathf.Lerp(0, -angle, interpolation);
        else rotation = Mathf.Lerp(0, angle, interpolation);

        
        this.transform.Rotate(0, 0, rotation, Space.Self);

    }

    void calcRange()
    {
        originRotation = transform.eulerAngles;

    }
}
