using System;
using UnityEngine;

namespace Dungeon.Spells
{
    public class Bomb : MonoBehaviour
    {
        #region PrivateData

        private Vector2 _gravity;
        private RaycastHit _hit;
        private Ray _ray;
        private int bounces;
        [SerializeField] private LayerMask _spawnLayerMask;
        [SerializeField] private LayerMask _bounceLayerMask;
        [SerializeField] private GameObject _trap;

        #endregion


        #region Fields

        public Vector3 velocity;

        #endregion


        #region UnityMethods

        private void Awake()
        {
            _gravity = Physics.gravity;
        }

        private void FixedUpdate()
        {
            ApplyGravity(_gravity);

            CheckForCollisionsAndMove(transform.position, velocity * Time.fixedDeltaTime);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (_spawnLayerMask == (_spawnLayerMask | (1 << other.transform.GetComponent<Collider>().gameObject.layer)))
            {
                // WRONG!:
                var trapRotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
                Instantiate(_trap, other.contacts[0].point, trapRotation);
            }
        }

        #endregion


        #region Methods

        /// <summary>
        /// Recursively checks by Raycast if we will meet some obstacles and move after resolving all path distance 
        /// </summary>
        private void CheckForCollisionsAndMove(Vector3 from, Vector3 direction)
        {
            var magnitude = direction.magnitude;
            _ray = new Ray(from, direction);

            if (Physics.Raycast(_ray, out _hit, magnitude, _bounceLayerMask))
            {
                var reflectDistance = direction.magnitude - _hit.distance;
                direction = Vector3.Reflect(direction, _hit.normal);
                var reflectVector = direction.normalized * reflectDistance;

                if (_spawnLayerMask ==
                    (_spawnLayerMask | (1 << _hit.transform.GetComponent<Collider>().gameObject.layer)))
                {
                    // WRONG!:
                    Instantiate(_trap, _hit.point, Quaternion.identity);
                    bounces++;
                if (bounces >= 3)
                    Destroy(gameObject);
                }



                CheckForCollisionsAndMove(_hit.point, reflectVector);
            }
            else
            {
                transform.position = from + direction;
                velocity = direction.normalized * velocity.magnitude;
            }
        }

        private void ApplyGravity(Vector3 gravity)
        {
            velocity += gravity * Time.deltaTime;
        }

        #endregion
    }
}