using Dungeon.Common;
using Dungeon.StatsAndCharacteristics;
using UnityEngine;

namespace Spells
{
    public class TrapDamageSpell : AOESpell
    {
        #region PrivateData

        [SerializeField] protected float _damage;

        #endregion


        #region Methods

        protected override void SpellEffect(Collider targetCollider)
        {
            if (!targetCollider.transform.TryGetComponent(out CharacteristicHolderBase statHolder)) return;

            if (statHolder.TryGetCharacteristic(CharacteristicType.Health, out CharacteristicBase health))
            {
                health.CurrentValue -= _damage;
                Destroy(gameObject);
            }
        }

        #endregion


        #region ITriggerListenerSubscriber implementation

        public override void SubscribeToTrigger(TriggerListener listener)
        {
            base.SubscribeToTrigger(listener);

            listener.EnterTrigger += SpellEffect;
            isSubscribedToTrigger = true;
        }

        public override void UnsubscribeFromTrigger(TriggerListener listener)
        {
            base.UnsubscribeFromTrigger(listener);

            listener.EnterTrigger -= SpellEffect;
            isSubscribedToTrigger = false;
        }

        #endregion
    }
}