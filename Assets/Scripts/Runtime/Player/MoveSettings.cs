using UnityEngine;

namespace Dungeon.Settings

{
    [CreateAssetMenu(fileName = "MovingSettings", menuName = "ScriptableObjects/MoveSettings", order = 1)]
    public sealed class MoveSettings : ScriptableObject
    {
        #region Fields

        public LayerMask groundMask;
        public float walkForwardSpeed = 9f;
        public float walkStrafeSpeed = 5f;
        
        public float runForwardSpeed = 15f;
        public float runStrafeSpeed = 9f;

        public float jumpPower = 30f;
        

        #endregion
    }
}