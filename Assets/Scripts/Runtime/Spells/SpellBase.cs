using Dungeon.Characters;
using Dungeon.StatsAndCharacteristics;
using UnityEngine;
using UnityEngine.UI;

namespace Dungeon.Spells
{
    public class SpellBase : MonoBehaviour
    {
        #region PrivateData

        protected SpellEffectType _spellEffectType;
        protected SpellType _spellType;
        protected Image _spellIcon;

        #endregion

        #region Methods

        protected virtual void ApplyEffect(CharacteristicBase characteristicBase)
        {
            
        }
        protected virtual void ApplyEffect(CharacterBase character)
        {
            
        }

        #endregion
    }
}