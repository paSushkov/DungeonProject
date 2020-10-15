using UnityEngine;

namespace Dungeon.AI
{
    public class EnemyMeleeAttack : EnemyStateBase
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (GetEnemyBase(animator) is WalkingEnemy walker)
            {
                walker.IsPerformingAttack = true;
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (GetEnemyBase(animator) is WalkingEnemy walker)
            {
                walker.IsPerformingAttack = false;
            }
        }
    }
}