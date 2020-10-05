using Dungeon.Common;
using Dungeon.Spells;
using UnityEngine;

namespace Spells
{
    public class AOESpell : SpellBase, ITriggerListenerSubscriber
    {
        #region PrivateData

        [SerializeField] protected TriggerListener _triggerListener;
        protected bool isSubscribedToTrigger;

        #endregion


        #region UnityMethods

        private void OnEnable()
        {
            SubscribeToTrigger(_triggerListener);
        }

        private void OnDisable()
        {
            UnsubscribeFromTrigger(_triggerListener);
        }

        #endregion


        #region Methods

        protected virtual void SpellEffect(Collider targetCollider)
        {
        }

        #endregion


        #region ITriggerListenerSubscriber implementation

        public virtual void SubscribeToTrigger(TriggerListener triggerListener)
        {
            if (isSubscribedToTrigger || triggerListener == null)
                return;
        }

        public virtual void UnsubscribeFromTrigger(TriggerListener triggerListener)
        {
            if (!isSubscribedToTrigger || triggerListener == null)
            {
                isSubscribedToTrigger = false;
                return;
            }
        }

        #endregion
    }
}