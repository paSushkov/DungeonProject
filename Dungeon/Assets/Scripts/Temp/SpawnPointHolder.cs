using System.Collections.Generic;
using UnityEngine;

// TODO:
// 1. Class for spawn points, which will know if spot is vacant
// 2. Interface or base class for spawn point holder
// 3. Object pool


public class SpawnPointHolder : MonoBehaviour
{
    public List<Transform> spawnPoints = new List<Transform>();

    public Vector3 GetRandomSpawnPointPosition()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Capacity)].position;
    }
}