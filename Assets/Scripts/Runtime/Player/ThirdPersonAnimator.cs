using Dungeon.Animations;
using UnityEngine;

namespace Dungeon.Player
{
    public class ThirdPersonAnimator : ThirdPersonMotor
    {
        #region PrivateData

        private int _inputHorizontalParam;
        private int _inputVerticalParam;
        private int _inputMagnitudeParam;
        private int _isGroundedParam;
        private int _isStrafingParam;
        private int _isSprintingParam;
        private int _groundDistanceParam;
        private int _forwardVelocityParam;

        #endregion


        #region Fields

        public const float WalkSpeed = 0.5f;
        public const float RunningSpeed = 1f;
        public const float SprintSpeed = 1.5f;
        public AnimatorParameters animatorParameters;

        #endregion


        #region Methods

        public override void Initialize()
        {
            base.Initialize();
            _inputHorizontalParam = Animator.StringToHash(animatorParameters.inputHorizontal);
            _inputVerticalParam = Animator.StringToHash(animatorParameters.inputVertical);
            _inputMagnitudeParam = Animator.StringToHash(animatorParameters.inputMagnitude);
            _isGroundedParam = Animator.StringToHash(animatorParameters.isGrounded);
            _isStrafingParam = Animator.StringToHash(animatorParameters.isStrafing);
            _isSprintingParam = Animator.StringToHash(animatorParameters.isSprinting);
            _groundDistanceParam = Animator.StringToHash(animatorParameters.groundDistance);
            _forwardVelocityParam = Animator.StringToHash(animatorParameters.forwardVelocity);
        }

        public void UpdateAnimator()
        {
            if (animator == null || !animator.enabled) return;

            animator.SetBool(_isStrafingParam, isStrafing);
            animator.SetBool(_isSprintingParam, IsSprinting);
            animator.SetBool(_isGroundedParam, IsGrounded);
            animator.SetFloat(_groundDistanceParam, groundDistance);

            var localVelocity = cachedTransform.InverseTransformDirection(body.velocity);
            animator.SetFloat(_forwardVelocityParam, localVelocity.z);

            if (isStrafing)
            {
                animator.SetFloat(_inputHorizontalParam, IsStopped ? 0 : horizontalSpeed,
                    strafeSettings.animationSmooth, Time.deltaTime);
                animator.SetFloat(_inputVerticalParam, IsStopped ? 0 : verticalSpeed,
                    strafeSettings.animationSmooth, Time.deltaTime);
            }
            else
            {
                animator.SetFloat(_inputVerticalParam, IsStopped ? 0 : verticalSpeed,
                    freeSettings.animationSmooth, Time.deltaTime);
            }

            animator.SetFloat(_inputMagnitudeParam, IsStopped ? 0f : inputMagnitude,
                isStrafing ? strafeSettings.animationSmooth : freeSettings.animationSmooth, Time.deltaTime);
        }
        
        protected void SetAnimatorMoveSpeed(MovementSettings settings)
        {
            Vector3 relativeInput = cachedTransform.InverseTransformDirection(moveDirection);
            verticalSpeed = relativeInput.z;
            horizontalSpeed = relativeInput.x;

            var newInput = new Vector2(verticalSpeed, horizontalSpeed);

            if (settings.walkByDefault)
                inputMagnitude = Mathf.Clamp(newInput.magnitude, 0, IsSprinting ? RunningSpeed : WalkSpeed);
            else
                inputMagnitude = Mathf.Clamp(IsSprinting ? newInput.magnitude + 0.5f : newInput.magnitude, 0, IsSprinting ? SprintSpeed : RunningSpeed);
        }

        #endregion
    }
}