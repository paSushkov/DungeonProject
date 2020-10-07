using UnityEngine;


namespace Dungeon.Player
{
    public sealed class CameraCollision : MonoBehaviour
    {
        #region PrivateData
        [SerializeField] private LayerMask castRayMask;
        [SerializeField] private float minDistance = 1.0f;
        [SerializeField] private float maxDistance = 4.0f;
        [SerializeField] private float smooth = 10.0f;
        private Vector3 _dollyDir;
        private Vector3 _dollyDirAdjusted;
        private float _distance;
        private Transform _transform;

        #endregion


        #region UnityMethods

        void Awake()
        {
            _transform = transform;
            _dollyDir = _transform.localPosition.normalized;
            _distance = _transform.localPosition.magnitude;
        }
        
        void Update()
        {
            Vector3 desiredCameraPos = transform.parent.TransformPoint(_dollyDir * maxDistance);
            RaycastHit hit;
            if (Physics.Linecast(transform.parent.position, desiredCameraPos, out hit, castRayMask))
                _distance = Mathf.Clamp((hit.distance * 0.9f), minDistance, maxDistance);
            else
                _distance = maxDistance;

            transform.localPosition = Vector3.Lerp(transform.localPosition, (_dollyDir * _distance),
                smooth * Time.deltaTime);
        }
        #endregion
    }
}