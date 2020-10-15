using Dungeon.Characters;
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
        if (!targetCollider.transform.root.transform.TryGetComponent(out CharacterBase character)) return;
        if (character.TryGetCharacteristic(CharacteristicType.Health, out var health))
        {
            ApplyEffect(health);
        }
    }

    protected override void ApplyEffect(CharacteristicBase characteristicBase)
    {
        characteristicBase.CurrentValue -= _damage*Time.fixedDeltaTime;
    }
    
    #endregion


    #region ITriggerListenerSubscriber implementation

    public override void SubscribeToTrigger(TriggerListener listener)
    {
        base.SubscribeToTrigger(listener);

        listener.StayingInTrigger += SpellEffect;
        isSubscribedToTrigger = true;
    }

    public override void UnsubscribeFromTrigger(TriggerListener listener)
    {
        base.UnsubscribeFromTrigger(listener);

        listener.StayingInTrigger -= SpellEffect;
        isSubscribedToTrigger = false;
    }

    #endregion
}