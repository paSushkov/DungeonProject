using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// TODO:
// 1. Class for spawn points, which will know if spot is vacant
// 2. Interface or base class for spawn point holder
// 3. Object pool


public class SpawnPointHolder : MonoBehaviour
{
    
    #region PrivateData

    public List<Transform> _spawnPoints;

    #endregion


    #region MyRegion

    private void Awake()
    {
        Transform[] allTransforms = GetComponentsInChildren<Transform>(true);
        _spawnPoints = allTransforms.ToList();
        _spawnPoints.RemoveAt(0);
    }

    #endregion
    

    public Vector3 GetRandomSpawnPointPosition()
    {
        return _spawnPoints[Random.Range(0, _spawnPoints.Count)].position;
    }
}