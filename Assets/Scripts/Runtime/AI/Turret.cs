using System;
using Dungeon.Characters;
using Dungeon.Common;
using Dungeon.Spells;
using Dungeon.StatsAndCharacteristics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dungeon.AI
{
    public class Turret : MonoBehaviour, ITriggerListenerSubscriber
    {
        #region PrivateData

        [SerializeField] private Transform head = null;
        [SerializeField] private TriggerListener triggerListener = null;
        private bool _isSubscribedToTrigger;
        private Transform _targetTransform;
        private float _shootDistance;
        private float _reloadTimer = 2.1f;
        private CharacteristicRegeneratable _targetHealth; 

        #endregion

        #region Fields

        public LayerMask obstaclesToSee;
        public GameObject projectile;
        public float rotationAngles;
        public float accuracyAngle;
        public float shootForce = 15f;
        public float damage = 30f;
        public float missFactor = 1f;
        public float reloadTime = 2f;

        #endregion


        #region UnityMethods

        private void Start()
        {
            if (triggerListener.listeningCollider is SphereCollider sphere)
                _shootDistance = sphere.radius;
        }

        private void OnEnable()
        {
            SubscribeToTrigger(triggerListener);
        }

        private void OnDisable()
        {
            UnsubscribeFromTrigger(triggerListener);
        }

        #endregion

        private void FixedUpdate()
        {
            if (_targetHealth != null && Mathf.Approximately(_targetHealth.CurrentValue, 0f))
                _targetTransform = null;
            
            if (_targetTransform)
            {
                AimToTarget(_targetTransform);
                TryShoot();
            }
            else
            {
                var headAngles = head.eulerAngles;
                float angle = Mathf.MoveTowardsAngle(headAngles.x, 0f, rotationAngles * Time.deltaTime);
                headAngles = new Vector3(angle, headAngles.y, headAngles.z);
                head.eulerAngles = headAngles;

                head.Rotate(0f, rotationAngles * Time.fixedDeltaTime, 0f);
            }
        }


        #region Methods

        private void ReturnToWarden(Collider potentialTarget)
        {
            if (potentialTarget.transform == _targetTransform)
            {
                _targetTransform = null;
                _targetHealth = null;
            }
        }

        private void GetTarget(Collider potentialTarget)
        {
            if (_targetTransform != null) return;

            if (potentialTarget.transform.TryGetComponent(out CharacterBase character) &&
                character.TryGetCharacteristic(CharacteristicType.Health, out CharacteristicRegeneratable health))
            {
                if (health.CurrentValue < 0f || Mathf.Approximately(health.CurrentValue, 0f))
                    return;
                if (potentialTarget.gameObject.CompareTag("Player") ||
                    potentialTarget.gameObject.CompareTag("Enemy"))
                {
                    _targetTransform = potentialTarget.transform.root;
                    _targetHealth = health;
                }
            }
        }

        private void AimToTarget(Transform targetTransform)
        {
            var shootTargetPosition = _targetTransform.TryGetComponent(out CharacterBase character)
                ? character.HitTarget.position
                : _targetTransform.position;

            var targetedRotation
                = Quaternion.LookRotation(shootTargetPosition - head.position);

            head.rotation = Quaternion.RotateTowards(
                head.rotation,
                targetedRotation,
                rotationAngles * Time.fixedDeltaTime);
        }

        private void TryShoot()
        {
            if (_reloadTimer < reloadTime || Mathf.Approximately(_reloadTimer, reloadTime))
                _reloadTimer += Time.fixedDeltaTime;

            var shootTargetPosition = _targetTransform.TryGetComponent(out CharacterBase character)
                ? character.HitTarget.position
                : _targetTransform.position;

            if (Physics.Raycast(head.position + head.forward, head.forward, out var hit,
                _shootDistance, obstaclesToSee, QueryTriggerInteraction.Ignore))
            {
                var angle = Vector3.Angle(head.forward, shootTargetPosition - head.position);
                if (angle < accuracyAngle)
                {
                    if (!(_reloadTimer >= reloadTime) || hit.transform.root != _targetTransform) return;

                    var newProjectile = Instantiate(projectile,
                        head.position + head.forward, Quaternion.identity);

                    if (newProjectile.transform.TryGetComponent(out MagicMissile missile))
                        missile.damage = damage;

                    if (newProjectile.transform.TryGetComponent(out Rigidbody body))
                    {
                        Vector3 randomOffset = new Vector3(
                            Random.Range(-missFactor, missFactor),
                            Random.Range(-missFactor, missFactor),
                            Random.Range(-missFactor, missFactor));

                        Vector3 direction;
                        direction =
                            ((shootTargetPosition + randomOffset) - head.position).normalized;

                        body.AddForce(direction * (shootForce * body.mass), ForceMode.Impulse);
                        _reloadTimer = 0f;
                    }
                }
            }
        }

        #endregion


        #region ITriggerListenerSubscriber implementation

        public void SubscribeToTrigger(TriggerListener listener)
        {
            if (_isSubscribedToTrigger || listener == null)
                return;
            listener.EnterTrigger += GetTarget;
            listener.StayingInTrigger += GetTarget;
            listener.ExitTrigger += ReturnToWarden;
        }


        public void UnsubscribeFromTrigger(TriggerListener listener)
        {
            if (!_isSubscribedToTrigger || listener == null)
            {
                _isSubscribedToTrigger = false;
                return;
            }

            listener.EnterTrigger -= GetTarget;
            listener.StayingInTrigger -= GetTarget;
            listener.ExitTrigger -= ReturnToWarden;
        }

        #endregion
    }
}