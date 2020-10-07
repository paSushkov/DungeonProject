using Dungeon.Managers;
using UnityEngine;

namespace Dungeon.Player
{
    public sealed class CameraFollow : MonoBehaviour
    {
        #region PrivateData

        [SerializeField] private float cameraFollowSpeed = 120f;
        [SerializeField] private GameObject cameraFollowObject;
        [SerializeField] private float clampAngle;
        [SerializeField] private float inputSensitivity;
        private Vector3 _followPosition;
        private float _mouseX;
        private float _mouseY;
        private float _rotX = 0.0f;
        private float _rotY = 0.0f;
        private Transform _transform;

        #endregion

        
        #region UnityMethods

        void Start()
        {
            _transform = transform;
            _transform.position = cameraFollowObject.transform.position;
            Vector3 rot = _transform.localRotation.eulerAngles;
            _rotY = rot.y;
            _rotX = rot.x;
        }

        void LateUpdate()
        {
            CameraUpdater();
        }
        
        private void OnEnable()
        {
            InputManager.Instance.MouseInputDone += GetMouseInput;
        }

        private void OnDisable()
        {
            InputManager.Instance.MouseInputDone -= GetMouseInput;
        }
        #endregion


        #region Methods

        private void GetMouseInput(float xInput, float yInput)
        {
            _mouseX = xInput;
            _mouseY = -yInput;
        }

        void CameraUpdater()
        {
            _rotY += _mouseX * inputSensitivity * Time.deltaTime;
            _rotX += _mouseY * inputSensitivity * Time.deltaTime;
            _rotX = Mathf.Clamp(_rotX, -clampAngle, clampAngle);
            var localRotation = Quaternion.Euler(_rotX, _rotY, 0f);
            transform.rotation = localRotation;

            var step = cameraFollowSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(_transform.position, cameraFollowObject.transform.position, step);
        }
        
        #endregion
    }
}