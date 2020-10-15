using System.Collections;
using System.Collections.Generic;
using Dungeon.Characters;
using Dungeon.Common;
using Dungeon.Spells;
using Dungeon.StatsAndCharacteristics;
using Spells;
using UnityEngine;

public class ManaVial : AOESpell
{
    #region PrivateData

    [SerializeField] protected float _mana;

    #endregion


    #region Methods

    protected override void SpellEffect(Collider targetCollider)
    {
        if (!targetCollider.transform.TryGetComponent(out CharacterBase character)) return;
        if (!character.TryGetCharacteristic(CharacteristicType.Mana, out CharacteristicRegeneratable mana)) return;
        mana.CurrentValue += _mana;
        Destroy(gameObject);
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

        listener.ExitTrigger -= SpellEffect;
        isSubscribedToTrigger = false;
    }

    #endregion
}