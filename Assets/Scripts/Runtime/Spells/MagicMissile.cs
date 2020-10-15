using Dungeon.Characters;
using Dungeon.StatsAndCharacteristics;
using UnityEngine;


namespace Dungeon.Spells
{
    public sealed class MagicMissile : Projectile
    {
        #region Fields

        public float damage = 30f;

        #endregion


        #region ClassLifeCycles

        public MagicMissile()
        {
            _spellEffectType = SpellEffectType.Damaging;
            _spellType = SpellType.Projectile;
        }

        #endregion


        #region UnityMethods

        protected override void OnCollisionEnter(Collision other)
        {
            if (other.transform.root.transform.TryGetComponent(out CharacterBase character))
            {
                    ApplyEffect(character);
            }

            base.OnCollisionEnter(other);
        }

        #endregion


        #region Methods

        protected override void ApplyEffect(CharacterBase character)
        {
            character.GetHealthHit(damage);
        }

        #endregion
    }
}