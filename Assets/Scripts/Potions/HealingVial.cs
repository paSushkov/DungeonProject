using System.Collections;
using System.Collections.Generic;
using Dungeon.Common;
using Dungeon.Spells;
using Dungeon.StatsAndCharacteristics;
using Spells;
using UnityEngine;

public class HealingVial : AOESpell
{
    #region PrivateData

    [SerializeField] protected float _heal;

    #endregion


    #region Methods

    protected override void SpellEffect(Collider targetCollider)
    {
        if (!targetCollider.transform.TryGetComponent(out CharacteristicHolderBase statHolder)) return;

        if (statHolder.TryGetCharacteristic(CharacteristicType.Health, out CharacteristicBase health))
        {
            health.CurrentValue += _heal;
            Destroy(gameObject);
        }
    }

    #endregion


    #region ITriggerListenerSubscriber implementation

    public override void SubscribeToTrigger(TriggerListener triggerListener)
    {
        base.SubscribeToTrigger(triggerListener);

        triggerListener.EnterTrigger += SpellEffect;
        isSubscribedToTrigger = true;
    }

    public override void UnsubscribeFromTrigger(TriggerListener triggerListener)
    {
        base.UnsubscribeFromTrigger(triggerListener);

        triggerListener.ExitTrigger -= SpellEffect;
        isSubscribedToTrigger = false;
    }

    #endregion
}
