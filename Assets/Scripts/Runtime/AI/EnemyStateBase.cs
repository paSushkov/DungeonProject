using UnityEngine;


namespace Dungeon.AI
{
    public class EnemyStateBase : StateMachineBehaviour
    {
        #region PrivateData

        private EnemyBase _enemyBase;

        #endregion


        #region Methods

        public EnemyBase GetEnemyBase(Animator animator)
        {
            if (!(_enemyBase is null)) 
                return _enemyBase;
            
            if (animator.transform.root.TryGetComponent(out EnemyBase enemy))
            {
                _enemyBase = enemy;
            }
            return _enemyBase;
        }

        #endregion
    }
}