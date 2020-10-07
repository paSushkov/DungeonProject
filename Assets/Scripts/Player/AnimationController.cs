using System;
using Dungeon.Settings;
using UnityEngine;


namespace Dungeon.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class AnimationController : MonoBehaviour
    {
        #region PrivateData

        [SerializeField] private Animator animator;
        [SerializeField] private MoveSettings settings;
        private Rigidbody _body;
        private Transform _transform;
        private Vector3 _velocity;
        private float _speed;
        private readonly int _speedParameter = Animator.StringToHash("speed");

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

            animator.SetFloat(_speedParameter, Mathf.InverseLerp(0f, settings.runForwardSpeed, _speed));
        }

        #endregion
    }
}