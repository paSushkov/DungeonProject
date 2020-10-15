using Dungeon.Player;
using UnityEngine;

namespace Dungeon.Characters
{
    public class JumpStart : CharacterStateBase
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var controller = GetCharacterController(animator);
            controller.Jump();
        }
    }
}