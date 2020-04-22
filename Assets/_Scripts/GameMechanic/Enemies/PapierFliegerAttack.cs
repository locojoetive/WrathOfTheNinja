using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PapierFliegerAttack : MonoBehaviour
{
    public GameObject fliegerPrefab;
    private GameObject fliegerClone;
    public Transform fliegerSpawn;
    private Animator animator;

    public float 
        throwAfter,
        throwAt,
        throwOffset;

    private void Start()
    {
        animator = GetComponent<Animator>();
        throwAt = Time.time + throwOffset + throwAfter;
    }

    private void Update()
    {
        if (throwAt < Time.time)
        {
            animator.SetTrigger("Throw");
            throwAt = Time.time + throwAfter;
        }
    }

    private void spawnFlieger()
    {
        fliegerClone = Instantiate(fliegerPrefab, fliegerSpawn.position, Quaternion.identity, null);
        fliegerClone.GetComponent<FliegerMove>().ShootInDirection(transform.localScale);
    }

}
