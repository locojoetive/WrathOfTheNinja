using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCameraMovement : MonoBehaviour {

    private SecurityWatchScript watchScript;
    public Transform target;

    public bool rotateDown = true, detectionOn = false;
    public Vector3 maxRotationDirection, minRotationDirection;
    public float angle, step;

    public float timeFrameConfirmation=1F, maxSpeed;
    private float confirmedAt = 0.0f;


    void Rotate()
    {

        Vector3 vectorToTarget = Vector3.RotateTowards(transform.right, minRotationDirection, 1.0f, 0.0f);
        float step = maxSpeed * Time.deltaTime;
        float angle = 0.0f;
        if (rotateDown)
        {
            vectorToTarget = Vector3.RotateTowards(transform.right, minRotationDirection, step, 0.0f);
            angle = Vector3.Angle(transform.right, minRotationDirection);
        }
        else
        {
            vectorToTarget = Vector3.RotateTowards(transform.right, maxRotationDirection, step, 0.0f);
            angle = Vector3.Angle(transform.right, maxRotationDirection);
        }
        transform.right = vectorToTarget;

        Debug.Log(angle);
        if (angle <= 1F) rotateDown = !rotateDown;
    }


    void Start () {
        initMotionSpace();
        watchScript = GetComponent<SecurityWatchScript>();
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    void Update() {
        CheckForPlayer();
        if (detectionOn) FollowPlayer();
        else Rotate();
	}

    void initMotionSpace()
    {
        float x = Mathf.Cos(angle * Mathf.Deg2Rad);
        float y = Mathf.Sin(angle * Mathf.Deg2Rad);
        minRotationDirection = transform.TransformDirection(new Vector3(x, -y, 0.0f));
        maxRotationDirection = transform.TransformDirection(new Vector3(x, y, 0.0f));
    }


    void FollowPlayer()
    {
        Vector3 vectorToTarget = target.position - transform.position;
        float step = 2 * maxSpeed * Time.deltaTime;
        
        Vector3 newDir = Vector3.RotateTowards(transform.right, vectorToTarget, step, 0.0f);
        Debug.DrawRay(transform.position, vectorToTarget, Color.red);

        transform.right = newDir;

    }

    void CheckForPlayer()
    {
        if(!watchScript.getDetectionStatus())
            confirmedAt = Time.time + timeFrameConfirmation;

        if (Time.time > confirmedAt) detectionOn = true;
        else detectionOn = false;
    }
}
