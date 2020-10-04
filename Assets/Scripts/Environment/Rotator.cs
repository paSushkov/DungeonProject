using UnityEngine;


namespace Dungeon.Environment
{
    public sealed class Rotator : MonoBehaviour
    {
        #region PrivateData

        private Transform _transform;

        #endregion


        #region Fields

        public float xSpeed = 1f;
        public float ySpeed = 1f;
        public float zSpeed = 1f;
        public bool isUsingGlobal;

        #endregion


        #region UnityMethods

        private void Awake()
        {
            _transform = transform;
        }

        private void FixedUpdate()
        {
            if (isUsingGlobal)
            {
                _transform.Rotate(Vector3.up, ySpeed * Time.fixedDeltaTime);
                _transform.Rotate(Vector3.forward, xSpeed * Time.fixedDeltaTime);
                _transform.Rotate(Vector3.right, zSpeed * Time.fixedDeltaTime);
            }
            else
            {
                _transform.Rotate(_transform.up, ySpeed * Time.fixedDeltaTime);
                _transform.Rotate(_transform.forward, xSpeed * Time.fixedDeltaTime);
                _transform.Rotate(_transform.right, zSpeed * Time.fixedDeltaTime);
            }
        }

        #endregion

    }
}