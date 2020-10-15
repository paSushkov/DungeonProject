using System;
using Dungeon.Settings;
using UnityEngine;


namespace Dungeon.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class AnimationController : MonoBehaviour
    {
        #region PrivateData

        [SerializeField] private Animator animator = null;
        [SerializeField] private MoveSettings settings = null;
        private Rigidbody _body;
        private Transform _transform;
        private Vector3 _velocity;
        private float _speed;
        private static readonly int _jump = Animator.StringToHash("Jump");
        private static readonly int _movingSpeed = Animator.StringToHash("movingSpeed");

        #endregion

        #region UnityMethods

        private void Awake()
        {
            _transform = transform;
            TryGetComponent(out _body);
        }

        private void LateUpdate()
        {
            _velocity = _body.velocity;
            var horizontalVelocity = _velocity;
            horizontalVelocity.y = 0f;
            _speed = horizontalVelocity.magnitude;
            animator.SetFloat(_movingSpeed, Mathf.InverseLerp(0f, settings.runForwardSpeed, _speed));
        }

        #endregion

        public void Jump()
        {
            animator.SetTrigger(_jump);
        }
    }
}