using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] private LayerMask triggerLayerMask;
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private float enemyDestructionTime = 1f;
    [SerializeField] private GameObject explosionParticles;
    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerLayerMask == (triggerLayerMask | (1 << other.gameObject.layer)))
        {
            Debug.Log("Explode!");
            Instantiate(explosionParticles, transform.position, Quaternion.identity);
            Destroy(other.gameObject, enemyDestructionTime);
            Destroy(gameObject);
        }
        
    }
}
