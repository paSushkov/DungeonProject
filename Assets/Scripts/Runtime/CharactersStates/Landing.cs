using UnityEngine;

namespace Dungeon.Player
{
    public class Landing : CharacterStateBase
    {
        
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var controller = GetCharacterController(animator);
            controller.ableToMove = true;
            controller._isJumpPerformed = false;
        }
    }
}