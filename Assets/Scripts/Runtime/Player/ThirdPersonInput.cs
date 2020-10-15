using UnityEngine;

namespace Dungeon.Player
{
    public class ThirdPersonInput : MonoBehaviour
    {
        #region PrivateData

        [SerializeField] private Transform mainCamera;
        [SerializeField] private ThirdPersonController characterController;

        #endregion


        #region Fields

        [Header("Controller Input")] public string horizontalInput = "Horizontal";
        public string verticalInput = "Vertical";
        public KeyCode jumpInput = KeyCode.Space;
        public KeyCode strafeInput = KeyCode.Tab;
        public KeyCode sprintInput = KeyCode.LeftShift;

        #endregion


        #region UnityMethods

        protected virtual void Start()
        {
            InitializeController();
        }

        protected virtual void FixedUpdate()
        {
            characterController.UpdateMotor();
            characterController.ControlLocomotionType();
            characterController.ControlRotationType();
        }

        protected virtual void Update()
        {
            InputHandle();
            characterController.UpdateAnimator();
        }

        public virtual void OnAnimatorMove()
        {
            characterController.ControlAnimatorRootMotion();
        }

        #endregion


        #region Methods

        protected virtual void InitializeController()
        {
            characterController = GetComponent<ThirdPersonController>();

            if (characterController != null)
                characterController.Initialize();
        }

        protected virtual void InputHandle()
        {
            MoveInput();
            characterController.UpdateMoveDirection(mainCamera);
            SprintInput();
            StrafeInput();
            JumpInput();
        }

        protected virtual void MoveInput()
        {
            characterController.input.x = Input.GetAxis(horizontalInput);
            characterController.input.z = Input.GetAxis(verticalInput);
        }

        protected virtual void StrafeInput()
        {
            if (Input.GetKeyDown(strafeInput))
                characterController.Strafe();
        }

        protected virtual void SprintInput()
        {
            if (Input.GetKeyDown(sprintInput))
                characterController.Sprint(true);
            else if (Input.GetKeyUp(sprintInput))
                characterController.Sprint(false);
        }


        protected virtual bool JumpConditions()
        {
            return characterController.IsGrounded &&
                   characterController.GroundAngle() < characterController.slopeLimit &&
                   !characterController.IsJumping && !characterController.IsStopped;
        }

        protected virtual void JumpInput()
        {
            if (Input.GetKeyDown(jumpInput) && JumpConditions())
                characterController.Jump();
        }

        #endregion
    }
}