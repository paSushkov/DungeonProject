using System.Collections;
using System.Collections.Generic;
using Dungeon.Characters;
using Dungeon.Common;
using Dungeon.Player;
using UnityEngine;

namespace Dungeon.AI
{
    public class EnemyBase : MonoBehaviour, ITriggerListenerSubscriber
    {
        #region PrivateData

        public List<Transform> _targetCharacters = new List<Transform>();
        protected Transform _currentTarget = null;
        protected Transform _transform;
        protected bool _isSubscribedToTargetListener;
        public bool _canSeeTarget;
        private float _lookDistance;
        private Vector3 _lookStartPosition;
        protected Coroutine loseTargetAfterSec;
        private string _cachedCoroutineName = nameof(LoseChasedTarget);

        #endregion


        #region Fields

        public AIState state;
        public Transform head;
        public LayerMask viewMask;
        public TriggerListener targetListener;
        [Range(-1f, 20f)] public float overrideLookDistance = -1f;
        public float viewAngle = 60f;
        public float chaseAfterLooseTime = 5f;
        public EnemyAnimationController animationController;

        #endregion


        #region UnityMethods

        protected virtual void Awake()
        {
            InitAwake();
        }

        protected virtual void OnEnable()
        {
            SubscribeToTrigger(targetListener);
        }

        protected virtual void OnDisable()
        {
            UnsubscribeFromTrigger(targetListener);
        }

        #endregion

        #region Methods

        protected virtual void InitAwake()
        {
            _transform = transform;

            if (overrideLookDistance < 0f)
            {
                switch (targetListener.listeningCollider)
                {
                    case SphereCollider sphere:
                        _lookDistance = sphere.radius;
                        break;
                    case CapsuleCollider capsule:
                        _lookDistance = capsule.radius;
                        break;
                    default:
                        _lookDistance = 15f;
                        break;
                }
            }
            else
                _lookDistance = overrideLookDistance;
        }


        private void LooseTarget(Collider potentialTarget)
        {
            if (_targetCharacters.Contains(potentialTarget.transform.root))
                _targetCharacters.Remove(potentialTarget.transform.root);
        }

        protected void CheckTargetVisibility(Transform target)
        {
            _lookStartPosition = head?.position ?? transform.position;

            // Determine if target is Character-type and have HitTarget assigned
            Vector3 targetCheckPosition = target.position;
            if (target.TryGetComponent(out CharacterBase character))
            {
                if (!(character.HitTarget is null))
                    targetCheckPosition = character.HitTarget.position;
            }

            //Debug.DrawLine(_lookStartPosition + Vector3.up, targetCheckPosition + Vector3.up, Color.blue);

            // Determine direction to target
            var directionToTarget = (targetCheckPosition - _lookStartPosition).normalized;

            // Not in view angle, cant see. If checked target is actually current target - loose target with delay
            if (Vector3.Angle(_transform.forward, directionToTarget) > viewAngle)
            {
              //  Debug.DrawLine(_lookStartPosition, targetCheckPosition, Color.red);

                _canSeeTarget = false;
                if ((_currentTarget == target) && loseTargetAfterSec is null)
                    loseTargetAfterSec = StartCoroutine(_cachedCoroutineName, chaseAfterLooseTime);
                return;
            }

            // In view angle but can be hidden behind walls. Raycast it!
            var rayToPlayer = new Ray(_lookStartPosition, directionToTarget);
            if (Physics.Raycast(rayToPlayer, out var hit, _lookDistance, viewMask, QueryTriggerInteraction.Ignore))
            {
                _canSeeTarget = hit.transform.root == target;
                if (_canSeeTarget)
                {
                //    Debug.DrawLine(_lookStartPosition, targetCheckPosition, Color.green);
                    // 1. If can see and we dont have current target - assign as current target
                    if (_currentTarget is null)
                        _currentTarget = target;

                    // 2. if can see and is current target - check if is about to loose it with coroutine and stop
                    if (!(loseTargetAfterSec is null) && _currentTarget == target)
                    {
                        StopCoroutine(loseTargetAfterSec);
                        loseTargetAfterSec = null;
                    }
                }

                // 3. if cant see and current target - loose with delay
                else
                {
                    //Debug.DrawLine(_lookStartPosition, targetCheckPosition, Color.yellow);
                    if ((_currentTarget == target) && loseTargetAfterSec is null)
                    {
                        Debug.Log("Start loose Coroutine!");
                        loseTargetAfterSec = StartCoroutine(_cachedCoroutineName, chaseAfterLooseTime);
                    }
                }


                // 4. If cant see and not current target - do nothing
            }
        }


        private void GetTarget(Collider potentialTarget)
        {
            if (_targetCharacters.Contains(potentialTarget.transform.root))
                return;

            if (potentialTarget.gameObject.CompareTag("Player"))
            {
                _targetCharacters.Add(potentialTarget.transform.root);
            }
        }


        private IEnumerator LoseChasedTarget(float time)
        {
            yield return new WaitForSecondsRealtime(time);
            _currentTarget = null;
            loseTargetAfterSec = null;
        }

        #endregion


        #region ITriggerListenerSubscriber implementation

        public void SubscribeToTrigger(TriggerListener listener)
        {
            if (_isSubscribedToTargetListener || listener == null)
                return;
            listener.EnterTrigger += GetTarget;
            listener.StayingInTrigger += GetTarget;
            listener.ExitTrigger += LooseTarget;
        }

        public void UnsubscribeFromTrigger(TriggerListener listener)
        {
            if (!_isSubscribedToTargetListener || listener == null)
            {
                _isSubscribedToTargetListener = false;
                return;
            }

            listener.EnterTrigger -= GetTarget;
            listener.StayingInTrigger -= GetTarget;
            listener.ExitTrigger -= LooseTarget;
        }

        #endregion
    }
}