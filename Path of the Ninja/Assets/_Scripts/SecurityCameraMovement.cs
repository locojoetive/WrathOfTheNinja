using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCameraMovement : MonoBehaviour {

    public Vector3 originRotation;
    private Quaternion upperBorder, bottomBorder;
    public float angle = 30;
    public float timeFrameConfirmation=1F, maxSpeed = 2F;
    public bool cr = false, goingDown = true, detectionOn = false;
    private SecurityWatchScript watchScript;
    public Transform target;
    private float confirmedAt = 0.0f;
    public float step;
    //Use this for initialization
	void Start () {
        calcRange();
        watchScript = GetComponent<SecurityWatchScript>();
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

    public float maxDelta = 0.0f;

    Vector2 refVel;

    void FollowPlayer()
    {
        //Rotate towards Player
        Vector3 vectorToTarget = transform.position - target.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * step);
    }

    void CheckForPlayer()
    {
        if(!watchScript.getDetectionStatus())
        {
            confirmedAt = Time.time + timeFrameConfirmation;
        }

        if(Time.time > confirmedAt)
        {
            detectionOn = true;
        }
        else
        {
            detectionOn = false;
        }

    }

    void Rotate()
    {
        if (confirmedAt > Time.time)
        {
            if (transform.eulerAngles.z <= originRotation.z - angle) goingDown = false;
            else if (transform.eulerAngles.z >= originRotation.z + angle) goingDown = true;

            float interpolation = maxSpeed * Time.deltaTime;
            float rotation = 0;

            if (goingDown) rotation = Mathf.Lerp(0, -angle, interpolation);
            else rotation = Mathf.Lerp(0, angle, interpolation);


            this.transform.Rotate(0, 0, rotation, Space.Self);
        }

    }

    void calcRange()
    {
        originRotation = transform.eulerAngles;
    }
}
