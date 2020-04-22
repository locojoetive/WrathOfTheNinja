using UnityEngine;

public class TurningEye : MonoBehaviour
{
    public SecurityGuardMovement movement;
    public bool turning = false;
    public bool turnedAround = false;
    private float timeFrameToTurn;
    public float rotationSpeed;
    public float rotatedAngle = 0F;

    private void Start()
    {
        if (Mathf.Abs(movement.speed) > 0F)
            rotationSpeed = 180F / movement.tStayFor;
    }

    void Update()
    {
        if (Mathf.Abs(movement.speed) > 0F)
        {
            if (!turning && movement.staying)
            {
                turning = true;
                if (movement.facingRight) rotationSpeed = -Mathf.Abs(rotationSpeed);
                else rotationSpeed = Mathf.Abs(rotationSpeed);
            }
            else if (!movement.staying)
            {
                turning = false;
                turnedAround = false;
                rotatedAngle = 0F;
            }
            else if (turning)
            {
                if (!turnedAround && rotatedAngle > 90F)
                {
                    movement.TurnAround();
                    turnedAround = true;
                    if (movement.facingRight) rotationSpeed = -Mathf.Abs(rotationSpeed);
                    else rotationSpeed = Mathf.Abs(rotationSpeed);
                } else
                {
                    rotatedAngle += Time.deltaTime * Mathf.Abs(rotationSpeed);
                }
                transform.Rotate(Time.deltaTime * new Vector3(0F, rotationSpeed, 0F));
            }
        }
    }
}
