using UnityEngine;
using System;
using Dungeon.Common;


namespace Dungeon.StatsAndCharacteristics
{
    [Serializable]
    public class CharacteristicBase
    {
        #region PrivateData

        [SerializeField] private CharacteristicType _characteristicType;
        [SerializeField] private float _minValue;
        [SerializeField] private float _maxValue;
        [SerializeField] private float _currentValue;

        #endregion


        #region Properties

        public float MinValue => _minValue;
        public float MaxValue => _maxValue;
        public CharacteristicType Type => _characteristicType;
        public float CurrentValue
        {
            get => _currentValue;
            set
            {
                _currentValue = Mathf.Clamp(value, _minValue, _maxValue);
                CurrentChanged?.Invoke(_currentValue);
            }
        }

        #endregion


        #region ClassLifeCycles

        public CharacteristicBase(CharacteristicType type, MinMaxCurrent minMaxCurrent)
        {
            _characteristicType = type;
            _minValue = minMaxCurrent.minValue;
            SetMaxValue(minMaxCurrent.maxValue);
            CurrentValue = minMaxCurrent.currentValue;
        }

        #endregion


        #region Methods

        public void SetMaxValue(float value)
        {
            if (value < _minValue || Mathf.Approximately(value, _minValue))
                _maxValue = _minValue;
            else
                _maxValue = value;
            MinMaxChanged?.Invoke(_minValue, _maxValue);
            
            if (_currentValue > _maxValue)
                CurrentValue = _maxValue;
        }

        public void SetMinValue(float value)
        {
            if (value > _maxValue || Mathf.Approximately(value, _maxValue))
                _minValue = _maxValue;
            else
                _minValue = value;
            MinMaxChanged?.Invoke(_minValue, _maxValue);
            
            if (_currentValue < _minValue)
                CurrentValue = _minValue;
        }

        #endregion


        #region Events

        public event CharacteristicMinMaxChanged MinMaxChanged;
        public event CharacteristicCurrentChanged CurrentChanged;

        #endregion
    }
}