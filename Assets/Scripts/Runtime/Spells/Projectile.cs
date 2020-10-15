using UnityEngine;


namespace Dungeon.Spells
{
    public class Projectile : SpellBase
    {
        #region PrivateData

        [SerializeField] protected GameObject _hitEffect;

        #endregion


        #region UnityMethods

        protected virtual void OnCollisionEnter(Collision other)
        {
            Instantiate(_hitEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        #endregion
    }
}