using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToPlayer : MovementBehaviours
{
    private GameObject playerTarget;
    public bool isRotating = true;
    
    [Header("Rotating speed: 0 means no rotation, 1 means instant rotation.")]
    [SerializeField] [Range(0, 1)] private float rotatingSpeed = 1;

    private void Start()
    {
        playerTarget = GlobalReferenceManager.PlayerPosition.gameObject;
    }

    void FixedUpdate()
    {
        if (isRotating)
        {
            //Rotating
            Vector3 direction = playerTarget.transform.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(-direction, Vector3.forward);
            rotation.x = transform.rotation.x;
            rotation.y = transform.rotation.y;
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotatingSpeed);
        }
    }
}
