using UnityEngine;

namespace Dungeon.Settings
{
    [CreateAssetMenu(fileName = "PlayerMoveSettings", menuName = "ScriptableObjects/PlayerMoveSettings", order = 1)]
    public class CharacterMoveSettings : ScriptableObject
    {
        #region Fields

        [Range(1f, 20f)] public float movementSmooth = 6f;
        [Range(0f, 1f)] public float animationSmooth = 0.2f;
        public bool walkByDefault = false;

        public bool rotateWhileJump = false;
        public float rotationSpeed = 16f;
        public float walkSpeed = 2f;
        public float runningSpeed = 4f;
        public float sprintSpeed = 6f;
        
        public float groundMinDistance = 0.25f;
        public float groundMaxDistance = 0.5f;
        public LayerMask groundMask;
        
        public float jumpTimer = 0.3f;
        public float jumpHeight = 4f;
        public float airSmooth = 6f;
        public float airSpeed = 5f;
        public float extraGravity = -10f;

        #endregion
    }
}