using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTrap : MonoBehaviour
{
    [SerializeField] private LayerMask spawnLayerMask;
    [SerializeField] private float lifeTime;
    [SerializeField] private GameObject trap;

    private void Awake()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (spawnLayerMask == (spawnLayerMask | (1 << other.collider.gameObject.layer)))
        {
            // WRONG!:
            var trapRotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            var myTrap = Instantiate(trap, other.contacts[0].point, trapRotation);
            
            Destroy(gameObject);
        }
    }
}
