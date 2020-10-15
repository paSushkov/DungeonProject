using UnityEngine;

namespace Dungeon.Player
{
    [System.Serializable]
    public class MovementSettings
    {
        #region Fields

        [Range(1f, 20f)] public float movementSmooth = 6f;
        [Range(0f, 1f)] public float animationSmooth = 0.2f;

        [Tooltip("Rotation speed of the character")]
        public float rotationSpeed = 16f;

        [Tooltip("Character will limit the movement to walk instead of running")]
        public bool walkByDefault = false;

        [Tooltip("Rotate with the Camera forward when standing idle")]
        public bool rotateWithCamera = false;

        [Tooltip("Speed to Walk using rigidbody or extra speed if you're using RootMotion")]
        public float walkSpeed = 2f;

        [Tooltip("Speed to Run using rigidbody or extra speed if you're using RootMotion")]
        public float runningSpeed = 4f;

        [Tooltip("Speed to Sprint using rigidbody or extra speed if you're using RootMotion")]
        public float sprintSpeed = 6f;

        #endregion
    }
}