using Dungeon.AI;
using Dungeon.Common;
using UnityEngine;

namespace Dungeon.Managers
{
    public sealed class GameManager : Singleton<GameManager>
    {
        #region PrivateData

        [SerializeField] private PatrolPointsProvider patrolPointsProvider;
        private bool _isCursorLocked = true;

        #endregion

        
        #region Properties

        public PatrolPointsProvider PointsProvider => patrolPointsProvider;

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