using System;
using System.Collections.Generic;
using Dungeon.StatsAndCharacteristics;
using UnityEngine;

namespace Dungeon.Characters
{
    public class CharacterBase : MonoBehaviour
    {
        #region PrivateData

        [SerializeField]
        private List<CharacteristicRegeneratable> characteristics = new List<CharacteristicRegeneratable>();

        [SerializeField] private Transform hitTarget = null;

        #endregion


        #region Fields

        public GameObject hitParticles;

        #endregion
        

        #region Properties

        public List<CharacteristicRegeneratable> Characteristics => characteristics;

        public Transform HitTarget => hitTarget;

        #endregion


        #region UnityMethods

        private void FixedUpdate()
        {
            foreach (var characteristic in Characteristics)
            {
                characteristic.Regenerate(Time.fixedDeltaTime);
            }
        }

        #endregion


        #region Methods

        public bool TryGetCharacteristic(CharacteristicType type, out CharacteristicRegeneratable characteristic)
        {
            foreach (var myCharacteristic in characteristics)
            {
                if (myCharacteristic.Type != type) continue;
                characteristic = myCharacteristic;
                return true;
            }

            characteristic = null;
            return false;
        }

        public void GetHealthHit(float damage)
        {
            if (hitParticles != null)
            {
                var spawnPosition = hitTarget is null ? transform.position : hitTarget.position;
                Instantiate(hitParticles, spawnPosition, Quaternion.identity);
            }

            if (TryGetCharacteristic(CharacteristicType.Health, out CharacteristicRegeneratable health))
            {
                health.CurrentValue -= damage;
            }
        }
        
        public void GetHealthHeal(float heal)
        {
            if (TryGetCharacteristic(CharacteristicType.Health, out CharacteristicRegeneratable health))
            {
                health.CurrentValue += heal;
            }
        }


        #endregion
    }
}