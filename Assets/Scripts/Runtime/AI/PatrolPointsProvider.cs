using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dungeon.AI
{
    public sealed class PatrolPointsProvider : MonoBehaviour
    {
        #region PrivateData

        private List<Transform> _patrolPoints;

        #endregion


        #region UnityMethods

        private void Awake()
        {
            Transform[] allTransforms = GetComponentsInChildren<Transform>(true);
            _patrolPoints = allTransforms.ToList();
            _patrolPoints.RemoveAt(0);
        }

        #endregion


        #region Methods

        public Vector3 GetRandomPotentialPoint()
        {
            return _patrolPoints[Random.Range(0, _patrolPoints.Count)].position;
        }

        public List<Vector3> GetRandomPatrolPoints(int count = -1)
        {
            if (count > _patrolPoints.Count-1 || count < 0)
                count = Random.Range(1, _patrolPoints.Count);

            var result = new List<Vector3>();
            for (var i = 0; i < count; i++)
            {
                Vector3 potentialPoint;
                do
                {
                    potentialPoint = _patrolPoints[Random.Range(0, _patrolPoints.Count)].position;
                } while (result.Contains(potentialPoint));
                result.Add(potentialPoint);
            }
            for (var i = 0; i < result.Count; i++)
            {
                Vector3 randomOffset = new Vector3(Random.Range(-1.0f, 1.0f), 0f, Random.Range(-1.0f, 1.0f));
                result[i] += randomOffset;
            }

            return result;
        }

        #endregion
    }
}