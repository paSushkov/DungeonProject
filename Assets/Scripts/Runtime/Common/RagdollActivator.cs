using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Dungeon.Common
{
    public class RagdollActivator : MonoBehaviour
    {
        #region PrivateData

        private Collider _mainCollider;
        private Rigidbody _body;
        private List<Collider> _allColliders;
        private List<Rigidbody> _bodies;
        private bool _isKinematic;

        #endregion


        #region Fields

        public List<Collider> serviceColliders = new List<Collider>();

        #endregion

        #region UnityMethods

        private void Awake()
        {
            TryGetComponent(out _mainCollider);
            TryGetComponent(out _body);
            _isKinematic = _body.isKinematic;
            _allColliders = GetComponentsInChildren<Collider>(true).ToList();
            _bodies = GetComponentsInChildren<Rigidbody>(true).ToList();
            _bodies.Remove(_body);
            
            foreach (var serviceCollider in serviceColliders)
            {
                if (serviceColliders is null)
                    break;
                if (_allColliders.Contains(serviceCollider))
                    _allColliders.Remove(serviceCollider);
            }

            DoRagdoll(false);
        }

        #endregion


        #region Methods

        public void DoRagdoll(bool isRagdoll)
        {
            foreach (var myCollider in _allColliders)
                myCollider.enabled = isRagdoll;
            foreach (var miniBody in _bodies)
            {
                miniBody.isKinematic = !isRagdoll;
                miniBody.velocity = _body.velocity;
            }


            _mainCollider.enabled = !isRagdoll;
            if (TryGetComponent(out Rigidbody body))
                body.isKinematic = isRagdoll || _isKinematic;
            if (TryGetComponent(out Animator animator))
                animator.enabled = !isRagdoll;
            if (TryGetComponent(out NavMeshAgent agent))
                agent.enabled = !isRagdoll;
        }

        #endregion
    }
}