using UnityEngine;

namespace Dungeon.Player
{
    public sealed class ThirdPersonController : ThirdPersonAnimator
    {
        public void ControlAnimatorRootMotion()
        {
            if (!enabled) return;

            if (inputSmooth == Vector3.zero)
            {
                cachedTransform.position = animator.rootPosition;
                cachedTransform.rotation = animator.rootRotation;
            }

            if (useRootMotion)
                MoveCharacter(moveDirection);
        }

        public void ControlLocomotionType()
        {
            if (lockMovement) return;

            if (locomotionType == LocomotionType.FreeWithStrafe && !isStrafing ||
                locomotionType == LocomotionType.OnlyFree)
            {
                SetControllerMoveSpeed(freeSettings);
                SetAnimatorMoveSpeed(freeSettings);
            }
            else if (locomotionType == LocomotionType.OnlyStrafe ||
                     locomotionType == LocomotionType.FreeWithStrafe && isStrafing)
            {
                IsStrafing = true;
                SetControllerMoveSpeed(strafeSettings);
                SetAnimatorMoveSpeed(strafeSettings);
            }

            if (!useRootMotion)
                MoveCharacter(moveDirection);
        }

        public void ControlRotationType()
        {
            if (lockRotation) return;

            bool validInput = input != Vector3.zero ||
                              (IsStrafing ? strafeSettings.rotateWithCamera : freeSettings.rotateWithCamera);

            if (!validInput) return;
            inputSmooth = Vector3.Lerp(inputSmooth, input,
                (isStrafing ? strafeSettings.movementSmooth : freeSettings.movementSmooth) * Time.deltaTime);

            Vector3 dir =
                (isStrafing && (!IsSprinting || sprintOnlyFree == false) ||
                 (freeSettings.rotateWithCamera && input == Vector3.zero)) && rotateTarget
                    ? rotateTarget.forward
                    : moveDirection;
            RotateToDirection(dir);
        }

        public void UpdateMoveDirection(Transform referenceTransform = null)
        {
            if (input.magnitude <= 0.01)
            {
                moveDirection = Vector3.Lerp(moveDirection, Vector3.zero,
                    (isStrafing ? strafeSettings.movementSmooth : freeSettings.movementSmooth) * Time.deltaTime);
                return;
            }

            if (referenceTransform && !rotateByWorld)
            {
                //get the right-facing direction of the referenceTransform
                var right = referenceTransform.right;
                right.y = 0;
                //get the forward direction relative to referenceTransform Right
                var forward = Quaternion.AngleAxis(-90, Vector3.up) * right;
                // determine the direction the player will face based on input and the referenceTransform's right and forward directions
                moveDirection = (inputSmooth.x * right) + (inputSmooth.z * forward);
            }
            else
            {
                moveDirection = new Vector3(inputSmooth.x, 0, inputSmooth.z);
            }
        }

        public void Sprint(bool value)
        {
            var sprintConditions = (input.sqrMagnitude > 0.1f && IsGrounded &&
                                    !(isStrafing && !strafeSettings.walkByDefault && (horizontalSpeed >= 0.5 ||
                                        horizontalSpeed <= -0.5 || verticalSpeed <= 0.1f)));

            if (value && sprintConditions)
            {
                if (input.sqrMagnitude > 0.1f)
                {
                    if (IsGrounded && useContinuousSprint)
                    {
                        IsSprinting = !IsSprinting;
                    }
                    else if (!IsSprinting)
                    {
                        IsSprinting = true;
                    }
                }
                else if (!useContinuousSprint && IsSprinting)
                {
                    IsSprinting = false;
                }
            }
            else if (IsSprinting)
            {
                IsSprinting = false;
            }
        }

        public void Strafe()
        {
            isStrafing = !isStrafing;
        }

        public void Jump()
        {
            jumpCounter = jumpTimer;
            IsJumping = true;

            if (input.sqrMagnitude < 0.1f)
                animator.CrossFadeInFixedTime("Jump", 0.1f);
            else
                animator.CrossFadeInFixedTime("JumpMove", .2f);
        }
    }
}