using Dungeon.Characters;
using Dungeon.Managers;
using Dungeon.StatsAndCharacteristics;
using UnityEngine;


namespace Dungeon.Player
{
    public sealed class PlayerShooter : MonoBehaviour
    {
        #region PrivateData

        [SerializeField] private GameObject projectile = null;
        [SerializeField] float shootForce = 50f;
        [SerializeField] float manaCost = 20f;
        [SerializeField] private CharacterBase character;
        private Transform _cameraTransform;
        private CharacteristicRegeneratable _mana;

        #endregion


        #region UnityMethods

        private void Awake()
        {
            _cameraTransform = Camera.main.transform;
            character.TryGetCharacteristic(CharacteristicType.Mana, out _mana);
        }

        private void OnEnable()
        {
            InputManager.Instance.OnShootInputDone += Shoot;
        }

        private void OnDisable()
        {
            InputManager.Instance.OnShootInputDone -= Shoot;
        }

        #endregion


        #region Methods

        private void Shoot()
        {    
            if (_mana.CurrentValue - manaCost < 0f) return;

            _mana.CurrentValue -= manaCost;
            var myProjectile = Instantiate(projectile, transform.position +transform.up*1.5f + _cameraTransform.forward, Quaternion.identity);
            if (myProjectile.TryGetComponent(out Rigidbody projectileRb))
            {
                projectileRb.AddForce(_cameraTransform.forward * (shootForce * projectileRb.mass), ForceMode.Impulse);
            }
        }

        #endregion
    }
}