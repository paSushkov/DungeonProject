namespace Dungeon.Common
{
    public interface ITriggerListenerSubscriber
    {
        void SubscribeToTrigger(TriggerListener listener);
        void UnsubscribeFromTrigger(TriggerListener listener);
    }
}