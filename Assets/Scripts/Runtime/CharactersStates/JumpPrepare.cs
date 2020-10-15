using Dungeon.Player;
using UnityEngine;

namespace Dungeon.Characters
{
    public class JumpPrepare : CharacterStateBase
    {
        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var controller = GetCharacterController(animator);
            controller.Jump();
        }
    }
}