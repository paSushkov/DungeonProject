using System.Collections.Generic;
using System.Linq;
using Dungeon.Characters;
using Dungeon.Common;
using UnityEngine;

namespace Dungeon.Temp
{
    public class ExplosionMine : MonoBehaviour
    {
        #region Fields

        public GameObject explosionPrefab;
        public float force = 10f;
        public float upForce = 5f;
        public float radius = 15;
        public LayerMask triggerMask;
        public Rigidbody rigidbody;

        #endregion


        #region UnityMethods

        private void OnTriggerEnter(Collider other)
        {
            if (!TriggerListener.IsInLayerMask(other.gameObject.layer, triggerMask) || other.isTrigger) return;
            var position = transform.position;
            Instantiate(explosionPrefab, position, Quaternion.identity);

            Collider[] colliders = Physics.OverlapSphere(position, radius);
            List<Collider> colliderList = colliders.ToList();
            foreach (var hitColliders in colliderList)
            {
                if (hitColliders.gameObject.CompareTag("Player"))
                    continue;

                if (hitColliders.transform.TryGetComponent(out CharacterBase characterBase))
                    characterBase.GetHealthHit(500f);
                
                if (hitColliders.transform.TryGetComponent(out Rigidbody body))
                    body.AddExplosionForce(force, position, radius, upForce, ForceMode.Impulse);
            }
        }

        #endregion
    }
}