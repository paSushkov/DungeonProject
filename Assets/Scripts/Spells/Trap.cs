using UnityEngine;


namespace Dungeon.Spells
{
    public sealed class Trap : MonoBehaviour
    {
        #region PrivateData

        [SerializeField] private LayerMask _triggerLayerMask;
        [SerializeField] private float _enemyDestructionTime = 1f;
        [SerializeField] private GameObject _explosionParticles;

        #endregion


        #region UnityMethods

        private void OnTriggerEnter(Collider other)
        {
            if (_triggerLayerMask == (_triggerLayerMask | (1 << other.gameObject.layer)))
            {
                Instantiate(_explosionParticles, transform.position, Quaternion.identity);
                Destroy(other.gameObject, _enemyDestructionTime);
                Destroy(gameObject);
            }
        }

        #endregion
    }
}