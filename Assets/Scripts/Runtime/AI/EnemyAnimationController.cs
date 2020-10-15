using Dungeon.Characters;
using Dungeon.Common;
using Dungeon.Settings;
using Dungeon.StatsAndCharacteristics;
using UnityEngine;
using UnityEngine.AI;


namespace Dungeon.AI
{
    public sealed class EnemyAnimationController : MonoBehaviour
    {
        #region PrivateData

        [SerializeField] private Animator animator = null;
        [SerializeField] private MoveSettings settings = null;
        private NavMeshAgent _agent;
        private float _speed;
        private float _speedTransponed;
        private static readonly int _movingSpeed = Animator.StringToHash("speed");
        private static readonly int _moving = Animator.StringToHash("moving");
        private static readonly int _attacking = Animator.StringToHash("Attacking");
        private bool _inSelfDestruction = false;
        private CharacteristicRegeneratable _health;
        private RagdollActivator _ragdollActivator;
        private WalkingEnemy _enemyBrain;

        #endregion

        #region Properties

        public bool IsAttacking
        {
            set { animator.SetBool(_attacking, value); }
        }

        #endregion


        #region UnityMethods

        private void Awake()
        {
            TryGetComponent(out _agent);
            TryGetComponent(out _ragdollActivator);
            TryGetComponent(out _enemyBrain);
            if (TryGetComponent(out CharacterBase characterBase))
                characterBase.TryGetCharacteristic(CharacteristicType.Health, out _health);

        }

        private void LateUpdate()
        {
            _speed = _agent.velocity.magnitude;
            if (_speed > 0.05f && animator.GetBool(_moving) != true)
                animator.SetBool(_moving, true);
            else if ((_speed < 0.05f && animator.GetBool(_moving)))
                animator.SetBool(_moving, false);

            _speedTransponed = Mathf.InverseLerp(settings.walkForwardSpeed, settings.runForwardSpeed, _speed);
            animator.SetFloat(_movingSpeed, _speedTransponed);
        }

        private void FixedUpdate()
        {
            if (!_inSelfDestruction && _health!=null)
            {
                if (_health.CurrentValue < 0.1f)
                {
                    if (_ragdollActivator != null)
                    {
                        _enemyBrain.enabled = false;
                        _agent.enabled = false;
                        animator.enabled = false;
                        _ragdollActivator.DoRagdoll(true);
                        Destroy(gameObject, 15f);
                        _inSelfDestruction = true;
                        this.enabled = false;
                    }
                    else
                    {
                        Destroy(gameObject);
                    }
                    
                }
            }
        }

        #endregion
    }
}