using System.Collections;
using System.Collections.Generic;
using Dungeon.AI;
using Dungeon.Managers;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class WalkingEnemy : EnemyBase
{
    #region PrivateData

    [SerializeField] private float _stopDistance;
    [SerializeField] private float checkPathTimer;
    private NavMeshAgent _agent;
    private List<Vector3> _patrolPoints;
    private PatrolPointsProvider _pointsProvider;
    private int _targetPointIndex;
    private IEnumerator _cachedDelay;
    private string _cachedCoroutineName;

    #endregion


    #region UnityMethods

    protected void Start()
    {
        _cachedDelay = new WaitForSecondsRealtime(checkPathTimer);
        _cachedCoroutineName = nameof(CheckIfCanReach);
        Gizmos.color = Color.magenta;
        RequestPatrolPoints();
        _agent.SetDestination(_patrolPoints[0]);
    }

    protected void FixedUpdate()
    {
        if (CheckIfArrived())
        {
            AssignNewPoint();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_patrolPoints == null)
            return;
        Gizmos.DrawSphere(_patrolPoints[_targetPointIndex], 0.5f);
        Gizmos.DrawLine(_patrolPoints[_targetPointIndex], _patrolPoints[_targetPointIndex] + Vector3.up * 10f);
        Gizmos.DrawLine(_patrolPoints[_targetPointIndex], transform.position);
    }

    #endregion

    #region Methods

    private IEnumerator CheckIfCanReach()
    {
        yield return _cachedDelay;
        var path = new NavMeshPath();
        _agent.CalculatePath(_patrolPoints[_targetPointIndex], path);
        if (path.status == NavMeshPathStatus.PathPartial)
        {
            if (path.corners.Length > 1)
            {
                var tempOffset = path.corners[path.corners.Length - 1] - path.corners[path.corners.Length - 2];
                tempOffset.Normalize();
                tempOffset *= -1f;
                _agent.SetDestination(path.corners[path.corners.Length - 1] + tempOffset);
            }
        }

        else
            _agent.SetPath(path);

        StartCoroutine(_cachedCoroutineName);
    }

    protected override void InitAwake()
    {
        base.InitAwake();
        TryGetComponent(out _agent);
        _pointsProvider = GameManager.Instance.PointsProvider;
    }

    protected virtual void RequestPatrolPoints()
    {
        _patrolPoints = _pointsProvider.GetRandomPatrolPoints();
    }

    protected virtual bool CheckIfArrived()
    {
        return (_agent.transform.position - _agent.pathEndPosition).sqrMagnitude < Mathf.Pow(_stopDistance, 2);
        //return _agent.remainingDistance < _stopDistance;
    }

    protected virtual void AssignNewPoint()
    {
        if (_patrolPoints == null)
            return;
        _targetPointIndex++;
        if (_targetPointIndex > _patrolPoints.Count - 1)
        {
            RequestPatrolPoints();
            _targetPointIndex = 0;
        }

        NavMeshPath path = new NavMeshPath();
        _agent.CalculatePath(_patrolPoints[_targetPointIndex], path);
        if (path.status == NavMeshPathStatus.PathPartial)
        {
            StopAllCoroutines();
            AssignNewPoint();
        }

        else
        {
            _agent.SetPath(path);
            StopAllCoroutines();
            StartCoroutine(_cachedCoroutineName);
        }
    }

    #endregion
}