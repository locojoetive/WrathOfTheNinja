using UnityEngine;

public class SecurityCameraMovement : MonoBehaviour {
    public bool 
        initialize;
    public float 
        speed,
        maxRotation,
        startRotation;

    void Start () {
        initializeMotionSpace();
    }

    void FixedUpdate() {
        if (initialize) initializeMotionSpace();
        HandleRotation();
    }

    private void HandleRotation()
    {
        transform.rotation = Quaternion.Euler(0F, 0F, startRotation + maxRotation * Mathf.Sin(Time.time * speed));
    }

    void initializeMotionSpace()
    {
        startRotation = transform.rotation.eulerAngles.z;
    }
}
