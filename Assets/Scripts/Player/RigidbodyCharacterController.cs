using Dungeon.Managers;
using Dungeon.Settings;
using UnityEngine;


namespace Dungeon.Player
{
    public sealed class RigidbodyCharacterController : MonoBehaviour
    {
        #region PrivateData

        [SerializeField] private MoveSettings _settings;
        private bool _isGrounded;
        private bool _isRunning;
        private bool _isJumpPerformed;

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
            if (_isInputPerformed)
            {
                if (_isGrounded)
                    Movement();
                Rotation();
            }

            Jump();
        }

        private void OnEnable()
        {
            InputManager.Instance.AxisInputDone += GetAxisInput;
        }

        private void OnDisable()
        {
            InputManager.Instance.AxisInputDone -= GetAxisInput;
        }

        void OnCollisionStay(Collision collision)
        {
            if (_settings.groundMask == (_settings.groundMask | (1 << collision.gameObject.layer)))
            {
                _isGrounded = true;
            }
        }

        void OnCollisionExit(Collision collision)
        {
            if (_settings.groundMask == (_settings.groundMask | (1 << collision.gameObject.layer)))
            {
                _isGrounded = false;
            }
        }

        #endregion

        #region Methods

        private void Movement()
        {
            _strafeSpeed = _isRunning ? _settings.runStrafeSpeed : _settings.walkStrafeSpeed;
            _forwardSpeed = _isRunning ? _settings.runForwardSpeed : _settings.walkForwardSpeed;

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

        private void Jump()
        {
            if (Input.GetAxis("Jump") > 0 && _isGrounded)
                _body.AddForce(Vector3.up * _settings.jumpPower, ForceMode.Impulse);
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