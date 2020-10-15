using UnityEngine;


namespace Dungeon.Player
{
    public class CharacterStateBase : StateMachineBehaviour
    {
        #region PrivateData

        private CharacterController _characterController;

        #endregion


        #region Methods

        public CharacterController GetCharacterController(Animator animator)
        {
            if (!(_characterController is null)) 
                return _characterController;
            
            if (animator.transform.root.TryGetComponent(out CharacterController controller))
            {
                _characterController = controller;
            }
            return _characterController;
        }

        #endregion
    }
}