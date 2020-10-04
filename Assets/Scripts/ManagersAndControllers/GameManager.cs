using UnityEngine;

namespace Dungeon.Managers
{
    public sealed class GameManager : MonoBehaviour
    {
        #region PrivateData

        private bool _isCursorLocked = true;

        #endregion


        #region UnityMethods

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (_isCursorLocked)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    _isCursorLocked = false;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    _isCursorLocked = true;
                }
            }
        }

        #endregion
    }
}