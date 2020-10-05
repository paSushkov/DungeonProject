using System.Collections;
using Dungeon.Common;
using UnityEngine;


namespace Dungeon.Environment
{
    public class DoorController : MonoBehaviour, ITriggerListenerSubscriber
    {
        #region PrivateData

        [SerializeField] private Transform _leftDoor;
        [SerializeField] private Transform _rightDoor;
        private bool _isCLosed = true;
        private bool _shouldBeClosed = true;
        private float _openState;
        private Quaternion _leftDoorClosedState;
        private Quaternion _rightDoorClosedState;
        private Quaternion leftDoorOpenState;
        private Quaternion rightDoorOpenState;
        private WaitForFixedUpdate _cachedWaitForFixedUpdate;
        private TriggerListener _triggerListener;
        private bool _isSubscribedToTrigger;


        #endregion


        #region Fields

        [Range(0f, 1f)] public float speed;

        #endregion


        #region UnityMethods

        private void OnEnable()
        {
            SubscribeToTrigger(_triggerListener);
        }

        private void OnDisable()
        {
            UnsubscribeFromTrigger(_triggerListener);
        }

        private void Awake()
        {
            _leftDoorClosedState = _leftDoor.localRotation;
            _rightDoorClosedState = _rightDoor.localRotation;

            leftDoorOpenState = _leftDoorClosedState * Quaternion.Euler(0f, -75f, 0f);
            rightDoorOpenState = _rightDoorClosedState * Quaternion.Euler(0f, 75f, 0f);
            
            _cachedWaitForFixedUpdate = new WaitForFixedUpdate();
            if (!TryGetComponent(out _triggerListener))
                Debug.LogWarning("Cant find TriggerListener!", gameObject);
        }

        #endregion

        #region MyRegion

        public void OpenDoors(Collider targetCollider)
        {
            if (targetCollider.gameObject.CompareTag("Player"))
            {
                StopAllCoroutines();

                _shouldBeClosed = false;
                StartCoroutine(OpeningDoors());
            }
        }

        public void CloseDoors(Collider targetCollider)
        {
            if (targetCollider.gameObject.CompareTag("Player"))
            {
                StopAllCoroutines();
                _shouldBeClosed = true;
                StartCoroutine(ClosingDoors());
            }
        }


        private IEnumerator ClosingDoors()
        {
            if (!_shouldBeClosed)
                yield break;
            
            _openState -= speed * Time.fixedDeltaTime;
            _openState = Mathf.Clamp(_openState, 0f, 1f);
            _leftDoor.localRotation = Quaternion.Slerp(leftDoorOpenState, _leftDoorClosedState, 1f - _openState);
            _rightDoor.localRotation = Quaternion.Slerp(rightDoorOpenState, _rightDoorClosedState, 1f - _openState);

            if (Mathf.Approximately(_openState, 0f))
            {
                _isCLosed = true;
                yield break;
            }

            yield return _cachedWaitForFixedUpdate;
            yield return StartCoroutine(ClosingDoors());
        }

        private IEnumerator OpeningDoors()
        {
            if (_shouldBeClosed)
                yield break;

            _openState += speed * Time.fixedDeltaTime;
            _openState = Mathf.Clamp(_openState, 0f, 1f);
            _leftDoor.localRotation = Quaternion.Slerp(_leftDoorClosedState, leftDoorOpenState, _openState);
            _rightDoor.localRotation = Quaternion.Slerp(_rightDoorClosedState, rightDoorOpenState, _openState);

            if (Mathf.Approximately(_openState, 1f))
            {
                _isCLosed = false;
                yield break;
            }

            yield return _cachedWaitForFixedUpdate;
            yield return StartCoroutine(OpeningDoors());
        }

        #endregion


        #region ITriggerListenerSubscriber implementation

        public void SubscribeToTrigger(TriggerListener triggerListener)
        {
            if (_isSubscribedToTrigger || triggerListener == null)
                return;
            _triggerListener.EnterTrigger += OpenDoors;
            _triggerListener.ExitTrigger += CloseDoors;
        }

        public void UnsubscribeFromTrigger(TriggerListener triggerListener)
        {
            if (!_isSubscribedToTrigger || triggerListener == null)
            {
                _isSubscribedToTrigger = false;
                return;
            }

            _triggerListener.EnterTrigger -= OpenDoors;
            _triggerListener.ExitTrigger -= CloseDoors;
        }
    }

    #endregion
}