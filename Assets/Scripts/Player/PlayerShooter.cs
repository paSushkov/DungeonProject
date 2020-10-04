using Dungeon.Managers;
using UnityEngine;


namespace Dungeon.Player
{
    public sealed class PlayerShooter : MonoBehaviour
    {
        #region PrivateData

        [SerializeField] private GameObject _projectile;
        [SerializeField] float _shootForce = 50f;
        private Transform _cameraTransform;

        #endregion


        #region UnityMethods

        private void Awake()
        {
            _cameraTransform = Camera.main.transform;
        }

        private void OnEnable()
        {
            InputManager.Instance.ShootInputDone += Shoot;
        }

        private void OnDisable()
        {
            InputManager.Instance.ShootInputDone -= Shoot;
        }

        private void OnDestroy()
        {
            InputManager.Instance.ShootInputDone -= Shoot;
        }

        #endregion


        #region Methods

        private void Shoot()
        {
            var myProjectile = Instantiate(_projectile, _cameraTransform.position + _cameraTransform.forward,
                _cameraTransform.rotation);
            if (myProjectile.TryGetComponent(out Rigidbody projectileRb))
            {
                projectileRb.AddForce(_cameraTransform.forward * _shootForce, ForceMode.Impulse);
            }
        }

        #endregion
    }
}