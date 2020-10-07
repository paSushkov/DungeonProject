using Dungeon.Common;
using Dungeon.Spells;
using UnityEngine;

namespace Dungeon.AI
{
    public class Turret : MonoBehaviour, ITriggerListenerSubscriber
    {
        #region PrivateData

        [SerializeField] private Transform _head;
        [SerializeField] private float _accuracyAngle;
        [SerializeField] GameObject _projectile;
        [SerializeField] private float _shootForce;
        [SerializeField] private float _missFactor;
        [SerializeField] private float _reloadTime = 2f;
        [SerializeField] private float _reloadTimer;
        [SerializeField] private TriggerListener _triggerListener;
        private bool _isSubscribedToTrigger;
        private Transform _targetTransform;

        #endregion

        #region Fields

        public float rotationAngles;

        #endregion


        #region UnityMethods

        private void OnEnable()
        {
            SubscribeToTrigger(_triggerListener);
        }

        private void OnDisable()
        {
            UnsubscribeFromTrigger(_triggerListener);
        }

        #endregion

        private void FixedUpdate()
        {
            if (_targetTransform)
            {
                AimToTarget(_targetTransform);
                TryShoot();
            }
            else
            {
                float angle = Mathf.MoveTowardsAngle(_head.eulerAngles.x, 0f, rotationAngles * Time.deltaTime);
                _head.eulerAngles = new Vector3(angle, _head.eulerAngles.y, _head.eulerAngles.z);

                _head.Rotate(0f,  rotationAngles * Time.fixedDeltaTime, 0f);
            }
        }


        #region Methods

        private void ReturnToWarden(Collider potentialTarget)
        {
            if (potentialTarget.transform == _targetTransform)
            {
                _targetTransform = null;
            }
        }

        private void GetTarget(Collider potentialTarget)
        {
            Debug.Log(potentialTarget.tag);
            if (potentialTarget.gameObject.CompareTag("Player") ||
                potentialTarget.gameObject.CompareTag("Enemy"))
            {
                _targetTransform = potentialTarget.transform;
            }
        }

        private void AimToTarget(Transform targetTransform)
        {
            var targetedRotation
                = Quaternion.LookRotation(targetTransform.position - _head.position);

            _head.rotation = Quaternion.RotateTowards(
                _head.rotation,
                targetedRotation,
                rotationAngles * Time.fixedDeltaTime);
        }

        private void TryShoot()
        {
            if (_reloadTimer > _reloadTime)
            {
                var angle = Vector3.Angle(_head.forward, _targetTransform.position - _head.position);
                if (angle < _accuracyAngle)
                {
                    var myProjectile = Instantiate(_projectile, _head.transform.position + _head.forward,
                        Quaternion.identity);
                    if (myProjectile.TryGetComponent(out Bomb bomb))
                    {
                        var randomVector = new Vector3(
                            Random.Range(-_missFactor, _missFactor),
                            Random.Range(-_missFactor, _missFactor),
                            Random.Range(-_missFactor, _missFactor));

                        bomb.velocity = _head.forward * _shootForce;

                        if (Vector3.Dot(_head.forward, randomVector) < 0f)
                            _missFactor *= -1f;
                        bomb.velocity += randomVector;
                    }

                    _reloadTimer = 0f;
                }
            }
            else if (_reloadTimer < _reloadTime || Mathf.Approximately(_reloadTimer, _reloadTime))
            {
                _reloadTimer += Time.fixedDeltaTime;
            }
        }

        #endregion

        #region ITriggerListenerSubscriber implementation

        public void SubscribeToTrigger(TriggerListener triggerListener)
        {
            if (_isSubscribedToTrigger || triggerListener == null)
                return;
            _triggerListener.EnterTrigger += GetTarget;
            _triggerListener.StayingInTrigger += GetTarget;
            _triggerListener.ExitTrigger += ReturnToWarden;
        }


        public void UnsubscribeFromTrigger(TriggerListener triggerListener)
        {
            if (!_isSubscribedToTrigger || triggerListener == null)
            {
                _isSubscribedToTrigger = false;
                return;
            }

            _triggerListener.EnterTrigger -= GetTarget;
            _triggerListener.StayingInTrigger -= GetTarget;
            _triggerListener.ExitTrigger -= ReturnToWarden;
        }

        #endregion
    }
}