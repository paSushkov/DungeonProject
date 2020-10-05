using Dungeon.Common;
using Dungeon.StatsAndCharacteristics;
using Spells;
using UnityEngine;

public class AoeDotTrap : AOESpell
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
            health.CurrentValue -= _damage * Time.deltaTime;
        }
    }

    #endregion


    #region ITriggerListenerSubscriber implementation

    public override void SubscribeToTrigger(TriggerListener triggerListener)
    {
        base.SubscribeToTrigger(triggerListener);

        triggerListener.StayingInTrigger += SpellEffect;
        isSubscribedToTrigger = true;
    }

    public override void UnsubscribeFromTrigger(TriggerListener triggerListener)
    {
        base.UnsubscribeFromTrigger(triggerListener);

        triggerListener.StayingInTrigger -= SpellEffect;
        isSubscribedToTrigger = false;
    }

    #endregion
}