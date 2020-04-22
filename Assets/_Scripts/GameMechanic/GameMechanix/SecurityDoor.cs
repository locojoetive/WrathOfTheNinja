using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityDoor : MonoBehaviour
{
    private new BoxCollider2D collider;
    private SecurityWatch[] securities;
    private void Start()
    {
        registerSecurity();
    }


    private void Update()
    {
        secureDoor();
    }

    private void secureDoor()
    {
        bool close = false;
        foreach (SecurityWatch security in securities)
        {
            if (security.isPlayerDetected())
            {
                close = true;
                break;
            }
        }
        if (close) closeDoor();
        else openDoor();
    }

    public void closeDoor()
    {
        collider.enabled = true;
    }

    public void openDoor()
    {
        collider.enabled = false;
    }

    private void registerSecurity()
    {
        collider = GetComponent<BoxCollider2D>();
        GameObject[] securityEntities = GameObject.FindGameObjectsWithTag("security");
        securities = new SecurityWatch[securityEntities.Length];
        int index = 0;
        foreach (GameObject securityEntity in securityEntities)
        {
            securities[index] = securityEntity.GetComponent<SecurityWatch>();
            index++;
        }
    }
}
