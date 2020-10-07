using Dungeon.Common;
using UnityEngine;


namespace Dungeon.Managers
{
    public sealed class InputManager : Singleton<InputManager>
    {
        #region DelegatesAndEvents

        public delegate void AxisInputProcessor(float horizontal, float vertical);

        public delegate void MouseMoveProcessor(float X, float Y);

        public delegate void ShootProcessor();


        public event AxisInputProcessor AxisInputDone;
        public event MouseMoveProcessor MouseInputDone;
        public event ShootProcessor ShootInputDone;

        #endregion


        #region PrivateData

        private float _horizintalInput;
        private float _verticalInput;
        private bool _emptyAxisWasSent;

        private float _mouseXMoveInput;
        private float _mouseYMoveInput;
        private bool _isEmptyMouseMoveWasSent;

        #endregion


        #region UnityMethods

        private void Update()
        {
            ReadAxisInput();
            BroadcastAxisInput();

            ReadMouseMoveInput();
            BroadcastMouseMoveInput();

            ReadAndBroadcastShoot();
        }

        #endregion


        #region Methods

        private void ReadAxisInput()
        {
            _horizintalInput = Input.GetAxis("Horizontal");
            _verticalInput = Input.GetAxis("Vertical");
        }

        private void BroadcastAxisInput()
        {
            if (Mathf.Abs(_horizintalInput) > 0f || Mathf.Abs(_verticalInput) > 0f)
            {
                AxisInputDone?.Invoke(_horizintalInput, _verticalInput);
                _emptyAxisWasSent = false;
            }
            else if (!_emptyAxisWasSent)
            {
                AxisInputDone?.Invoke(0f, 0f);
                _emptyAxisWasSent = true;
            }
        }

        private void ReadMouseMoveInput()
        {
            _mouseXMoveInput = Input.GetAxis("Mouse X");
            _mouseYMoveInput = Input.GetAxis("Mouse Y");
        }

        private void BroadcastMouseMoveInput()
        {
            if (Mathf.Abs(_mouseXMoveInput) > 0 || Mathf.Abs(_mouseXMoveInput) > 0)
            {
                MouseInputDone?.Invoke(_mouseXMoveInput, _mouseYMoveInput);
                _isEmptyMouseMoveWasSent = false;
            }
            else if (!_isEmptyMouseMoveWasSent)
            {
                MouseInputDone?.Invoke(0f, 0f);
                _isEmptyMouseMoveWasSent = true;
            }
        }

        private void ReadAndBroadcastShoot()
        {
            if (Input.GetMouseButtonDown(0))
            {
                ShootInputDone?.Invoke();
            }
        }

        #endregion
    }
}