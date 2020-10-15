using Dungeon.Managers;
using Dungeon.Settings;
using UnityEngine;


namespace Dungeon.Player
{
    public sealed class CharacterController : MonoBehaviour
    {
        #region PrivateData

        [SerializeField] private MoveSettings moveSettings = null;
        [SerializeField] private AnimationController animationController = null;
        
        
        
        public bool _isGrounded;
        private bool _isRunning;
        public bool _isJumpPerformed;

        private float _bodyRotationX;
        private Vector3 _directionIntentX;
        private Vector3 _directionIntentY;
        private float _forwardSpeed;
        private float _strafeSpeed;

        #region TechData

        private Transform _transform;
        private Transform _cameraTransform;
        private Rigidbody _body;
        private float _horizontalInput;
        private float _verticalInput;
        private bool _isInputPerformed;
        
        private int _inputHorizontalParam = Animator.StringToHash("InputHorizontal");
        private int _inputVerticalParam = Animator.StringToHash("InputVertical");
        private int _inputMagnitudeParam = Animator.StringToHash("InputMagnitude");
        private int _isGroundedParam = Animator.StringToHash("IsGrounded");
        private int _isStrafingParam = Animator.StringToHash("IsStrafing");
        private int _isSprintingParam = Animator.StringToHash("IsSprinting");
        private int _groundDistanceParam = Animator.StringToHash("GroundDistance");
        
        

        #endregion


        #region Fields

        public bool ableToMove = true;

        #endregion

        #endregion


        #region UnityMethods

        private void Awake()
        {
            _transform = transform;
            _cameraTransform = Camera.main.transform;
            TryGetComponent(out _body);
        }

        private void Update()
        {
            _isInputPerformed = CheckInput(_horizontalInput, _verticalInput);
            _isRunning = CheckRunning();
        }

        private void FixedUpdate()
        {
            CheckGround();
            if (_isInputPerformed)
            {
                if (_isGrounded && !_isJumpPerformed && ableToMove)
                    Movement();
                Rotation();
            }

            if (Input.GetAxis("Jump") > 0 && _isGrounded && !_isJumpPerformed)
            {
                animationController.Jump();
                ableToMove = false;
                _isJumpPerformed = true;
            }
        }

        private void OnEnable()
        {
            InputManager.Instance.OnAxisInputDone += GetAxisInput;
        }

        private void OnDisable()
        {
            InputManager.Instance.OnAxisInputDone -= GetAxisInput;
        }

        // void OnCollisionStay(Collision collision)
        // {
        //     if (moveSettings.groundMask == (moveSettings.groundMask | (1 << collision.gameObject.layer)))
        //     {
        //         _isGrounded = true;
        //     }
        // }

        void OnCollisionExit(Collision collision)
        {
            if (moveSettings.groundMask == (moveSettings.groundMask | (1 << collision.gameObject.layer)))
            {
                _isGrounded = false;
            }
        }

        #endregion


        #region Methods

        private void CheckGround()
        {
            if (Physics.SphereCast(transform.position + Vector3.up, 0.3f, Vector3.down, out var hit, 0.8f,
                moveSettings.groundMask, QueryTriggerInteraction.Ignore))
            {
                if (!_isGrounded)
                    _isGrounded = true;
            }
            else
            {
                if (_isGrounded)
                    _isGrounded = false;
            }
        }

        private void Movement()
        {
            _strafeSpeed = _isRunning ? moveSettings.runStrafeSpeed : moveSettings.walkStrafeSpeed;
            _forwardSpeed = _isRunning ? moveSettings.runForwardSpeed : moveSettings.walkForwardSpeed;

            _directionIntentX = _cameraTransform.right;
            _directionIntentX.y = 0f;
            _directionIntentX.Normalize();

            _directionIntentY = _cameraTransform.forward;
            _directionIntentY.y = 0f;
            _directionIntentY.Normalize();

            var newVelocity =
                _directionIntentY * (_verticalInput * _forwardSpeed) +
                _directionIntentX * (_horizontalInput * _forwardSpeed) +
                Vector3.up * +_body.velocity.y;
            _body.velocity = Vector3.ClampMagnitude(newVelocity, _forwardSpeed);
        }

        private void Rotation()
        {
            var camForward = _body.velocity.normalized;
            camForward.y = 0f;
            if (camForward.normalized != Vector3.zero)
                transform.rotation =
                    Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(camForward.normalized), 0.2f);
        }

        public void Jump()
        {
            var speed = _body.velocity.magnitude;
            _body.AddForce(Vector3.up * moveSettings.jumpPower * _body.mass, ForceMode.Impulse);
            if (speed > 0.1f)
                _body.AddForce(
                    _body.transform.forward * (_isRunning ? moveSettings.runForwardSpeed : moveSettings.walkForwardSpeed) * _body.mass,
                    ForceMode.Impulse);
        }

        private bool CheckRunning()
        {
            return Input.GetKey(KeyCode.LeftShift);
        }

        private void GetAxisInput(float horizontalInput, float verticalInput)
        {
            _horizontalInput = horizontalInput;
            _verticalInput = verticalInput;
        }

        private bool CheckInput(float horizontalInput, float verticalInput)
        {
            return !Mathf.Approximately(horizontalInput, 0f) || !Mathf.Approximately(verticalInput, 0f);
        }

        #endregion
    }
}