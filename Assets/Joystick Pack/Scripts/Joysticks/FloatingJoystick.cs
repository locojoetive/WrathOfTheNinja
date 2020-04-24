using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FloatingJoystick : Joystick
{
    protected override void Start()
    {
        base.Start();
        background.gameObject.SetActive(false);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (TouchBehaviour.iMovingFinger == -1
            && eventData.position.x < TouchBehaviour.neutralScreenPosition.x
            )
        {
            background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
            background.gameObject.SetActive(true);
            base.OnPointerDown(eventData);

            TouchBehaviour.StartMoving(eventData.pointerId);
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (TouchBehaviour.iMovingFinger == eventData.pointerId)
        {
            TouchBehaviour.EndMoving();
            background.gameObject.SetActive(false);
            base.OnPointerUp(eventData);
        }
    }

}