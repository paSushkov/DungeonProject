using System;
using System.Collections.Generic;
using UnityEngine;


namespace Dungeon.StatsAndCharacteristics
{
    public class CharacteristicHolderBase : MonoBehaviour
    {
        #region PrivateData

        // Once upon a time i will write for myself a serializeble Dictionary
        [SerializeField] private List<CharacteristicRegeneratable> _characteristicsRegeneratable;

        #endregion


        #region Properties

        public List<CharacteristicRegeneratable> Characteristics => _characteristicsRegeneratable;

        #endregion


        #region UnityMethods

        private void Awake()
        {
            InitOnAwake();
        }

        private void Update()
        {
            ProcessUpdate();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
        }
#endif

        #endregion


        #region Methods

        protected virtual void InitOnAwake()
        {
        }

        protected virtual void ProcessUpdate()
        {
            var deltaTime = Time.deltaTime;
            foreach (var characteristic in _characteristicsRegeneratable)
            {
                characteristic.Regenerate(deltaTime);
            }
        }

        public bool TryGetCharacteristic(CharacteristicType type, out CharacteristicBase characteristic)
        {
            if (_characteristicsRegeneratable != null)
            {
                foreach (var _characteristic in _characteristicsRegeneratable)
                {
                    if (_characteristic.Type != type) continue;
                    characteristic = _characteristic;
                    return true;
                }
            }
            characteristic = null;
            return false;
        }

        #endregion
    }
}