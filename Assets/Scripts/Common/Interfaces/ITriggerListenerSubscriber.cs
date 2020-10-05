namespace Dungeon.Common
{
    public interface ITriggerListenerSubscriber
    {
        void SubscribeToTrigger(TriggerListener triggerListener);
        void UnsubscribeFromTrigger(TriggerListener triggerListener);
    }
}