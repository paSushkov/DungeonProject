using UnityEngine;

namespace Dungeon.Environment
{
    [RequireComponent(typeof(Light))]
    public sealed class LightTrembler : MonoBehaviour
    {
        #region PrivateData

        [SerializeField] private float _defaultRange = 1;
        [SerializeField] private float _defaultIntensity = 1;
        [SerializeField, Range(0, 1f)] private float _rangeMaxRandom = 1;
        [SerializeField, Range(0, 1f)] private float _intensityMaxRandom = 1;
        [SerializeField, Range(0, 1f)] private float _lerpSpeed = 0.2f;
        private Light _myLight;
        private float _targetRange;
        private float _targetIntensity;

        #endregion


        #region UnityMethods

        private void Awake()
        {
            TryGetComponent(out _myLight);
            _targetIntensity = _defaultIntensity +
                               _defaultIntensity *
                               Random.Range(-_intensityMaxRandom, _intensityMaxRandom);
            _targetRange = _defaultRange +
                           _defaultRange *
                           Random.Range(-_rangeMaxRandom, _rangeMaxRandom);
        }

        void LateUpdate()
        {
            if (Mathf.Abs(_myLight.intensity - _targetIntensity) < 0.01)
            {
                _targetIntensity = _defaultIntensity +
                                   _defaultIntensity * Random.Range(-_intensityMaxRandom, _intensityMaxRandom);
            }

            if (Mathf.Abs(_myLight.range - _targetRange) < 0.1)
            {
                _targetRange = _defaultRange + _defaultRange * Random.Range(-_rangeMaxRandom, _rangeMaxRandom);
            }

            _myLight.intensity = Mathf.Lerp(_myLight.intensity, _targetIntensity, _lerpSpeed * Time.deltaTime * 100);
            _myLight.range = Mathf.Lerp(_myLight.range, _targetRange, _lerpSpeed * Time.deltaTime * 100);
        }

        #endregion
    }
}