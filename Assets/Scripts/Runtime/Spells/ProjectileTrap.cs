using UnityEngine;


namespace Dungeon.Spells
{
    public sealed class ProjectileTrap : MonoBehaviour
    {
        #region PrivateData

        [SerializeField] private LayerMask _spawnLayerMask = new LayerMask();
        [SerializeField] private GameObject _trap = null;

        #endregion


        #region UnityMethods

        private void OnCollisionEnter(Collision other)
        {
            if (_spawnLayerMask == (_spawnLayerMask | (1 << other.collider.gameObject.layer)))
            {
                // WRONG!:
                var trapRotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
                Instantiate(_trap, other.contacts[0].point, trapRotation);

                Destroy(gameObject);
            }
        }

        #endregion
    }
}