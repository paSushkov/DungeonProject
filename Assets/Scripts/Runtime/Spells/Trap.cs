using UnityEngine;


namespace Dungeon.Spells
{
    public sealed class AOE : MonoBehaviour
    {
        #region Fields

        public LayerMask triggerLayerMask;
        public GameObject explosionParticles;
        public float enemyDestructionTime = 1f;

        #endregion

        #region UnityMethods

        private void OnTriggerEnter(Collider other)
        {
            if (triggerLayerMask == (triggerLayerMask | (1 << other.gameObject.layer)))
            {
                Instantiate(explosionParticles, transform.position, Quaternion.identity);
                Destroy(other.gameObject, enemyDestructionTime);
                Destroy(gameObject);
            }
        }

        #endregion
    }
}