using UnityEngine;

public class StayInPosition : MonoBehaviour
{
    Vector2 position;
    Quaternion rotation;
    void Awake()
    {
        position = transform.position;
        rotation = transform.rotation;
    }

    void LateUpdate()
    {
        transform.position = position;
        transform.rotation = rotation;
    }
}
