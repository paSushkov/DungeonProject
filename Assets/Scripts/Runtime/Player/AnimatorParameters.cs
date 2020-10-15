using UnityEngine;

namespace Dungeon.Animations
{
    [CreateAssetMenu(fileName = "AnimatorParameters", menuName = "ScriptableObjects/AnimatorParameters", order = 1)]
    public class AnimatorParameters : ScriptableObject
    {
        public string inputHorizontal = "InputHorizontal";
        public string inputVertical = "InputVertical";
        public string inputMagnitude ="InputMagnitude";
        public string isGrounded = "IsGrounded";
        public string isStrafing = "IsStrafing";
        public string isSprinting = "IsSprinting";
        public string groundDistance = "GroundDistance";
        public string forwardVelocity = "ForwardVelocity";
    }
}