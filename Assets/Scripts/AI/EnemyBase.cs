using UnityEngine;

namespace Dungeon.AI
{
    public class EnemyBase : MonoBehaviour
    {
        #region PrivateData

        protected AIState state;
        protected Transform _transform;

        #endregion

        #region UnityMethods

        protected virtual void Awake()
        {
            InitAwake();
        }

        #endregion

        #region Methods

        protected virtual void InitAwake()
        {
            _transform = transform;
        }

        #endregion
    }
}