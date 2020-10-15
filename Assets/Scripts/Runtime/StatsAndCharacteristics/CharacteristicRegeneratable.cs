using System;
using Dungeon.Common;
using UnityEngine;

namespace Dungeon.StatsAndCharacteristics
{
    [Serializable]
    public sealed class CharacteristicRegeneratable : CharacteristicBase
    {
        #region PrivateData

        [SerializeField] private float _defaultRegenerationAmount;
        [SerializeField] private float _currentRegenerationAmount;

        #endregion


        #region Properties

        public float DefaultRegenerationAmount => _defaultRegenerationAmount;

        public float CurrentRegenerationAmount => _currentRegenerationAmount;

        #endregion


        #region ClassLifeCycles

        public CharacteristicRegeneratable(
            CharacteristicType type,
            MinMaxCurrent minMaxCurrent,
            float defaultRegenerationAmount
            ) : base(type, minMaxCurrent)
        {
            _defaultRegenerationAmount = _currentRegenerationAmount= defaultRegenerationAmount;
        }
        
        #endregion
        
        
        #region Metods

        public void Regenerate(float deltaTime)
        {
            CurrentValue += _currentRegenerationAmount * deltaTime;
        }

        #endregion
    }
}