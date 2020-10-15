using UnityEngine;


namespace Dungeon.Player
{
    public class CharacterState : StateMachineBehaviour
    {
        #region PrivateData

        private ThirdPersonController _characterController;

        #endregion


        #region Methods

        public ThirdPersonController GetCharacterController(Animator animator)
        {
            if (!(_characterController is null)) 
                return _characterController;
            
            if (animator.transform.root.TryGetComponent(out ThirdPersonController controller))
            {
                _characterController = controller;
            }
            return _characterController;
        }

        #endregion
    }
}