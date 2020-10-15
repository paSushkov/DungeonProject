using UnityEngine;


namespace Dungeon.Common
{
    public sealed class TriggerListener : MonoBehaviour
    {
        #region MyRegion

        public Collider listeningCollider;

        #endregion
        
        
        #region StaticMetods

        public static bool IsInLayerMask(int layer, LayerMask layerMask)
        {
            return layerMask == (layerMask | (1 << layer));
        }

        #endregion
        
        
        #region UnityMethods

        private void Awake()
        {
            TryGetComponent(out listeningCollider);
        }

        private void OnTriggerStay(Collider other)
        {
            StayingInTrigger?.Invoke(other);
        }

        private void OnTriggerEnter(Collider other)
        {
            EnterTrigger?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            ExitTrigger?.Invoke(other);
        }

        #endregion

        
        #region Events
        
        public event TriggerEventHandler StayingInTrigger;
        public event TriggerEventHandler EnterTrigger;
        public event TriggerEventHandler ExitTrigger;

        #endregion
    }
}