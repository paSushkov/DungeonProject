using System.Collections;
using System.Collections.Generic;
using Dungeon.AI;
using Dungeon.Characters;
using Dungeon.Environment;
using Dungeon.Managers;
using Dungeon.Settings;
using Dungeon.StatsAndCharacteristics;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class WalkingEnemy : EnemyBase
{
    #region PrivateData

    [SerializeField] private float stopDistance = 1f;
    [SerializeField] private LayerMask doorMask = new LayerMask();
    private NavMeshAgent _agent;
    private List<Vector3> _patrolPoints;
    private PatrolPointsProvider _pointsProvider;
    private int _targetPointIndex = -1;
    private IEnumerator _cachedDelay;
    private NavMeshPath _path;
    private bool _isAttacking;
    private bool _isPerformingAttack;

    #endregion


    #region Fields

    public bool _onPatrol = true;
    public MoveSettings moveSettings;

    public float turnRate = 45f;
    public float accuracyAngle = 30f;
    public float hitRange = 2f;
    public float meleeDamage = 30f;

    #endregion


    #region Properties

    public bool IsPerformingAttack
    {
        get => _isPerformingAttack;
        set { _isPerformingAttack = value; }
    }

    public AIState State
    {
        get => state;
        set
        {
            state = value;
            switch (value)
            {
                case AIState.None:
                    IsAttacking = false;
                    break;
                case AIState.Idle:
                    IsAttacking = false;
                    break;
                case AIState.Patrol:
                    IsAttacking = false;
                    break;
                case AIState.Chase:
                    IsAttacking = false;
                    break;
                case AIState.Attack:
                    break;
                default:
                    IsAttacking = false;
                    break;
            }
        }
    }


    public bool IsAttacking
    {
        get => _isAttacking;
        set
        {
            _isAttacking = value;
            if (!(animationController is null))
                animationController.IsAttacking = value;
        }
    }

    #endregion


    #region UnityMethods

    protected void Start()
    {
        _path = new NavMeshPath();

        if (_onPatrol && !(_pointsProvider is null))
        {
            State = AIState.Patrol;
            _agent.speed = moveSettings.walkForwardSpeed;
            RequestPatrolPoints();
            AssignNewPoint();
        }
        else
        {
            State = AIState.Idle;
        }
    }

    protected void FixedUpdate()
    {
        // If does not have target - check everyone in trigger range
        if (_currentTarget is null)
        {
            foreach (var potentialTarget in _targetCharacters)
            {
                CheckTargetVisibility(potentialTarget);
                if (!(_currentTarget is null))
                    break;
            }

            // And if default is on patrol = goTo patrol, if you dont have anyone to chase or idle somewhere
            if (_onPatrol && State != AIState.Patrol && State != AIState.Idle)
            {
                State = AIState.Patrol;
                NavMesh.CalculatePath(transform.position, _patrolPoints[_targetPointIndex], _agent.areaMask, _path);
                _agent.SetPath(_path);
            }
        }
        // Check only current target if we have one
        else
        {
            CheckTargetVisibility(_currentTarget);
        }

        CheckDoorsAhead();


        // If have target but not chasing or attacking him - chase!
        if (!(_currentTarget is null) && (State != AIState.Chase && State != AIState.Attack))
            State = AIState.Chase;

        switch (State)
        {
            case AIState.Patrol:
                if (!Mathf.Approximately(_agent.speed, moveSettings.walkForwardSpeed))
                    _agent.speed = moveSettings.walkForwardSpeed;

                if (_agent.isStopped)
                {
                    _agent.isStopped = false;
                    NavMesh.CalculatePath(transform.position, _patrolPoints[_targetPointIndex], _agent.areaMask, _path);
                    _agent.SetPath(_path);
                }

                if (CheckIfArrived())
                    AssignNewPoint();
                break;

            case AIState.Idle:
                if (!_agent.isStopped)
                {
                    _agent.isStopped = true;
                    _agent.velocity = Vector3.zero;
                }

                break;

            case AIState.Chase:
                if (!(_currentTarget is null) && !IsPerformingAttack)
                {
                    if (_agent.isStopped)
                        _agent.isStopped = false;

                    if (!Mathf.Approximately(_agent.speed, moveSettings.runForwardSpeed))
                        _agent.speed = moveSettings.runForwardSpeed;

                    NavMesh.CalculatePath(transform.position, _currentTarget.position, _agent.areaMask, _path);
                    _agent.SetPath(_path);
                    if (CheckIfArrived())
                    {
                        if (!_agent.isStopped)
                        {
                            _agent.isStopped = true;
                            _agent.velocity = Vector3.zero;
                        }

                        var angle = Vector3.Angle(_transform.forward, _currentTarget.position - _transform.position);
                        if (angle > accuracyAngle)
                        {
                            var desiredRotation =
                                Quaternion.LookRotation(_currentTarget.position - _transform.position);
                            _transform.rotation = Quaternion.RotateTowards(
                                _transform.rotation,
                                desiredRotation,
                                turnRate * Time.fixedDeltaTime);
                        }
                        else
                        {
                            State = AIState.Attack;
                        }
                    }
                }

                // else return to idle position
                break;
            case AIState.Attack:
                if (_currentTarget is null)
                    State = AIState.Idle;
                else
                {
                    Debug.DrawLine(transform.position, _currentTarget.position, Color.magenta);
                    Debug.DrawLine(transform.position, _agent.destination, Color.yellow);

                    if (Vector3.SqrMagnitude(transform.position - _currentTarget.position) >
                        Mathf.Pow(stopDistance, 2f))
                        State = AIState.Chase;
                    else
                    {
                        var angle = Vector3.Angle(_transform.forward, _currentTarget.position - _transform.position);
                        if (angle > accuracyAngle)
                        {
                            IsAttacking = false;
                            var desiredRotation =
                                Quaternion.LookRotation(_currentTarget.position - _transform.position);
                            _transform.rotation = Quaternion.RotateTowards(
                                _transform.rotation,
                                desiredRotation,
                                turnRate * Time.fixedDeltaTime);
                        }
                        else
                        {
                            if (!IsAttacking)
                                IsAttacking = true;
                        }
                    }
                }

                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        if (_patrolPoints is null || _patrolPoints.Count < 1)
            return;
        for (var i = _targetPointIndex; i < _patrolPoints.Count; i++)
        {
            Gizmos.DrawSphere(_patrolPoints[i], 0.5f);
        }

        if (_targetPointIndex > _patrolPoints.Count - 1 || _targetPointIndex < 0)
            return;
        Gizmos.DrawLine(_patrolPoints[_targetPointIndex], _patrolPoints[_targetPointIndex] + Vector3.up * 10f);
        Gizmos.DrawLine(_patrolPoints[_targetPointIndex], transform.position);
        if (!_agent.enabled)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, Vector3.up * 5f);
        }

        var testPath = _agent.path;
        if (testPath != null)
        {
            List<Vector3> test = new List<Vector3>();
            test.Add(transform.position);
            foreach (var corner in testPath.corners)
            {
                test.Add(corner);
            }

            for (var i = 1; i < test.Count; i++)
            {
                Debug.DrawLine(test[i], test[i - 1], Color.black);
            }
        }
    }

    #endregion

    #region Methods

    public void AttackMelee()
    {
        var targetPosition = _currentTarget.position;
        var angle = Vector3.Angle(_transform.forward, targetPosition - _transform.position);
        var distanceSqr = Vector3.SqrMagnitude(transform.position - targetPosition);
        if (angle < accuracyAngle * 3f &&
            distanceSqr < Mathf.Pow(hitRange, 2f) &&
            _currentTarget != null)
        {
            if (_currentTarget.TryGetComponent(out CharacterBase character))
            {
                character.GetHealthHit(meleeDamage);
            }
        }
    }

    private void CheckDoorsAhead()
    {
        if (Physics.Raycast(transform.position, transform.forward, out var hit, 2f, doorMask,
            QueryTriggerInteraction.Collide))
        {
            //Debug.DrawLine(transform.position, hit.point, Color.red);

            if (hit.transform.TryGetComponent(out DoorController door))
            {
                if (door.OpenState < 0.75f)
                {
                    if (door.ShouldBeClosed)
                        door.ShouldBeClosed = false;
                    State = AIState.Idle;
                }
                else if (_onPatrol && _currentTarget is null && State != AIState.Patrol)
                    State = AIState.Patrol;
                else if (!(_currentTarget is null) && (State != AIState.Chase || State != AIState.Attack))
                    State = AIState.Chase;
            }
        }

// else
//     Debug.DrawRay(transform.position, transform.forward * 2f, Color.green);
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
        return (transform.position - _agent.destination).sqrMagnitude < Mathf.Pow(stopDistance, 2) ||
               (Mathf.Approximately((transform.position - _agent.destination).sqrMagnitude, Mathf.Pow(stopDistance, 2)));
    }

    private void CheckIfPathIsPartial()
    {
        if (_path.status == NavMeshPathStatus.PathPartial)
        {
            if ((_path.corners[_path.corners.Length - 1] - _patrolPoints[_targetPointIndex]).sqrMagnitude < 2f)
            {
                _patrolPoints[_targetPointIndex] = _path.corners[_path.corners.Length - 1];
                NavMesh.CalculatePath(transform.position, _patrolPoints[_targetPointIndex], _agent.areaMask, _path);
                CheckIfPathIsPartial();
            }
            else
            {
                AssignNewPoint();
            }
        }
    }

    protected virtual void AssignNewPoint()
    {
        _targetPointIndex++;
        if (_targetPointIndex >= _patrolPoints.Count)
        {
            RequestPatrolPoints();
            _targetPointIndex = 0;
        }

        NavMesh.CalculatePath(transform.position, _patrolPoints[_targetPointIndex], _agent.areaMask, _path);
        _agent.SetPath(_path);
    }

    #endregion
}