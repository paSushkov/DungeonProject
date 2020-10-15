using Dungeon.AI;
using Dungeon.Common;
using UnityEngine;
using UnityEngine.AI;


namespace Dungeon.Environment
{
    public class DoorController : MonoBehaviour, ITriggerListenerSubscriber
    {
        #region PrivateData

        [SerializeField] private Transform leftDoor = null;
        [SerializeField] private Transform rightDoor = null;
        [SerializeField] private LayerMask reactLayers = new LayerMask();
        [SerializeField] private TriggerListener triggerListener = null;
        private bool _isCLosed = true;
        private float _openState;
        private Quaternion _leftDoorClosedState;
        private Quaternion _rightDoorClosedState;
        private Quaternion _leftDoorOpenState;
        private Quaternion _rightDoorOpenState;
        private bool _isSubscribedToTrigger;

        #endregion


        #region Fields

        public bool shouldBeClosed = true;
        [Range(0f, 1f)] public float speed;

        #endregion


        #region Properties

        public bool IsCLosed => _isCLosed;
        public float OpenState => _openState;

        public bool ShouldBeClosed
        {
            get => shouldBeClosed;
            set => shouldBeClosed = value;
        }


        #endregion


        #region UnityMethods

        private void OnEnable()
        {
            SubscribeToTrigger(triggerListener);
        }

        private void OnDisable()
        {
            UnsubscribeFromTrigger(triggerListener);
        }

        private void Awake()
        {
            _leftDoorClosedState = leftDoor.localRotation;
            _rightDoorClosedState = rightDoor.localRotation;

            _leftDoorOpenState = _leftDoorClosedState * Quaternion.Euler(0f, -75f, 0f);
            _rightDoorOpenState = _rightDoorClosedState * Quaternion.Euler(0f, 75f, 0f);
        }

        private void Update()
        {
            if (shouldBeClosed && (!_isCLosed || _openState > 0f))
                ClosingDoors();
            if (!shouldBeClosed && (_isCLosed || _openState < 1f))
                OpeningDoors();
        }

        #endregion

        #region Methods

        private void OpenDoors(Collider targetCollider)
        {
            if (!TriggerListener.IsInLayerMask(targetCollider.gameObject.layer, reactLayers)) return;
            shouldBeClosed = false;
        }

        private void CloseDoors(Collider targetCollider)
        {
            if (!TriggerListener.IsInLayerMask(targetCollider.gameObject.layer, reactLayers)) return;
            shouldBeClosed = true;
        }

        private void PushAgents(Collider targetCollider)
        {
            if (!TriggerListener.IsInLayerMask(targetCollider.gameObject.layer, reactLayers)) return;
            if (targetCollider.transform.TryGetComponent(out WalkingEnemy walker) && 
                targetCollider.transform.TryGetComponent(out NavMeshAgent agent))
            {
                if (!_isCLosed)
                {
                    if (walker._onPatrol && walker.state != AIState.Patrol)
                    {
                        walker.state = AIState.Patrol;
                        agent.enabled = true;
                    }
                }
            }
        }

        private void ClosingDoors()
        {
            if (!shouldBeClosed)
                return;;
            _openState -= speed * Time.fixedDeltaTime;
            _openState = Mathf.Clamp(_openState, 0f, 1f);
            leftDoor.localRotation = Quaternion.Slerp(_leftDoorOpenState, _leftDoorClosedState, 1f - _openState);
            rightDoor.localRotation = Quaternion.Slerp(_rightDoorOpenState, _rightDoorClosedState, 1f - _openState);

            if (Mathf.Approximately(_openState, 0f))
            {
                _isCLosed = true;
            }
        }

        private void OpeningDoors()
        {
            if (shouldBeClosed)
                return;
            _openState += speed * Time.fixedDeltaTime;
            _openState = Mathf.Clamp(_openState, 0f, 1f);
            leftDoor.localRotation = Quaternion.Slerp(_leftDoorClosedState, _leftDoorOpenState, _openState);
            rightDoor.localRotation = Quaternion.Slerp(_rightDoorClosedState, _rightDoorOpenState, _openState);

            if (Mathf.Approximately(_openState, 1f))
            {
                _isCLosed = false;
            }
        }
        

        #endregion


        #region ITriggerListenerSubscriber implementation

        public void SubscribeToTrigger(TriggerListener listener)
        {
            if (_isSubscribedToTrigger || listener == null)
                return;
            listener.EnterTrigger += OpenDoors;
            listener.StayingInTrigger += OpenDoors;
            listener.StayingInTrigger += PushAgents;
            listener.ExitTrigger += CloseDoors;
        }

        public void UnsubscribeFromTrigger(TriggerListener listener)
        {
            if (!_isSubscribedToTrigger || listener == null)
            {
                _isSubscribedToTrigger = false;
                return;
            }

            listener.EnterTrigger -= OpenDoors;
            listener.StayingInTrigger -= OpenDoors;
            listener.StayingInTrigger += PushAgents;
            listener.ExitTrigger -= CloseDoors;
        }
    }

    #endregion
}