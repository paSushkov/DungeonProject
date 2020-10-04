using Dungeon.Managers;
using UnityEngine;


namespace Dungeon.Player
{
// TODO: refactor - implement Velocities !!! Player should gain speed at least on deep fall !!!
// ... despite the fact he has no place to fall...
    public class PlayerMovement : MonoBehaviour
    {
        #region PrivateData

        [SerializeField] private CharacterController _characterController;
        [SerializeField] private float _walkSpeed;
        [SerializeField] private float _downLookClampdown = -45f;
        [SerializeField] private float _upLookClamp = 90f;
        private Transform _mainCameraTransform;
        private float _horizontalInput;
        private float _verticalInput;
        private float _xMouseInput;
        private float _yMouseInput;
        private float _rotationX = 0f;
        private float _rotationY = 0f;
        private Vector3 _playerGravityForce;
        private Transform _transform;

        #endregion


        #region Fields

        public float ySensitivity = 1f;
        public float xSensitivity = 1f;

        #endregion


        #region UnityMethods

        private void Awake()
        {
            _transform = transform;
            _playerGravityForce = Physics.gravity;
            _mainCameraTransform = Camera.main.transform;
        }

        private void FixedUpdate()
        {
            ProcessMovement();
            if (!_characterController.isGrounded)
            {
            ApplySimpleGravity();
            }
        }

        private void LateUpdate()
        {
            ProcessLook();
        }

        private void OnEnable()
        {
            InputManager.Instance.AxisInputDone += GetAxisInput;
            InputManager.Instance.MouseInputDone += GetMouseMoveInput;
        }

        private void OnDisable()
        {
            InputManager.Instance.AxisInputDone -= GetAxisInput;
            InputManager.Instance.MouseInputDone -= GetMouseMoveInput;
        }

        private void OnDestroy()
        {
            InputManager.Instance.AxisInputDone -= GetAxisInput;
            InputManager.Instance.MouseInputDone -= GetMouseMoveInput;
        }

        #endregion


        #region Methods

        private void GetAxisInput(float horizontalInput, float verticalInput)
        {
            _horizontalInput = horizontalInput;
            _verticalInput = verticalInput;
        }

        private void GetMouseMoveInput(float xMouseInput, float yMouseInput)
        {
            _xMouseInput = xMouseInput;
            _yMouseInput = yMouseInput;
        }

        private void ProcessMovement()
        {
            if (Mathf.Approximately(_horizontalInput, 0f) && Mathf.Approximately(_verticalInput, 0f))
                return;

            var direction = _transform.forward * (_verticalInput * _walkSpeed) +
                            _transform.right * (_horizontalInput * _walkSpeed);
            direction = Vector3.ClampMagnitude(direction, _walkSpeed);

            _characterController.Move(direction * Time.deltaTime);
        }

        private void ProcessLook()
        {
            if (Mathf.Abs(_yMouseInput) > 0f)
            {
                _rotationX += _yMouseInput * ySensitivity;
                _rotationX = Mathf.Clamp(_rotationX, _downLookClampdown, _upLookClamp);
                var localEulerAngles = _mainCameraTransform.localEulerAngles;
                localEulerAngles
                    = new Vector3(-_rotationX,
                        localEulerAngles.y,
                        localEulerAngles.z);
                _mainCameraTransform.localEulerAngles = localEulerAngles;
            }

            if (Mathf.Abs(_xMouseInput) > 0f)
            {
                _rotationY = _xMouseInput * xSensitivity;
                transform.Rotate(0f, _rotationY, 0f);
            }
        }

        private void ApplySimpleGravity()
        {
            _characterController.Move(_playerGravityForce * Time.deltaTime);
        }

        #endregion
    }
}