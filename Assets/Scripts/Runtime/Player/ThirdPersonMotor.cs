using UnityEngine;

namespace Dungeon.Player
{
    [RequireComponent(typeof(Rigidbody), typeof(Animator), typeof(CapsuleCollider))]
    public class ThirdPersonMotor : MonoBehaviour
    {
        #region PrivateData

        protected Animator animator;
        protected Rigidbody body;
        protected PhysicMaterial frictionPhysics, maxFrictionPhysics, slippyPhysics;
        protected CapsuleCollider capsuleCollider;
        protected Transform cachedTransform;

        protected float inputMagnitude; // sets the inputMagnitude to update the animations in the animator controller
        protected float verticalSpeed; // set the verticalSpeed based on the verticalInput
        protected float horizontalSpeed; // set the horizontalSpeed based on the horizontalInput       
        protected float moveSpeed; // set the current moveSpeed for the MoveCharacter method
        protected float verticalVelocity; // set the vertical velocity of the rigidbody
        protected float colliderRadius, colliderHeight; // storage capsule collider extra information        
        protected float heightReached; // max height that character reached in air;
        protected float jumpCounter; // used to count the routine to reset the jump
        protected float groundDistance; // used to know the distance from the ground
        protected RaycastHit groundHit; // raycast to hit the ground 
        public bool lockRotation = false; // lock the rotation of the controller (not the animation)        
        protected bool isStrafing; // internally used to set the strafe movement                
        protected Transform rotateTarget; // used as a generic reference for the camera.transform
        public Vector3 input; // generate raw input for the controller
        protected Vector3 colliderCenter; // storage the center of the capsule collider info                
        protected Vector3 inputSmooth; // generate smooth input based on the inputSmooth value       
        protected Vector3 moveDirection; // used to know the direction you're moving 
        private bool _isJumping;

        
        #endregion

        #region Fields

        [Header("Movement")] public bool useRootMotion = false;
        public bool rotateByWorld = false;
        public bool useContinuousSprint = true;
        public bool sprintOnlyFree = true;
        public LocomotionType locomotionType = LocomotionType.FreeWithStrafe;
        public MovementSettings freeSettings, strafeSettings;
        public bool lockMovement = false; 


        [Header("- Airborne")] public bool jumpWithRigidbodyForce = false;
        public bool jumpAndRotate = true;
        public float jumpTimer = 0.3f;

        [Tooltip("Extra jump height")] public float jumpHeight = 4f;
        public float airSpeed = 5f;
        public float airSmooth = 6f;
        public float extraGravity = -10f;
        [HideInInspector] public float limitFallVelocity = -15f;

        [Header("- Ground")] public LayerMask groundLayer = 1 << 0;

        [Tooltip("Distance to became not grounded")]
        public float groundMinDistance = 0.25f;

        public float groundMaxDistance = 0.5f;

        [Tooltip("Slope limit")] [Range(30, 80)]
        public float slopeLimit = 75f;

        #endregion


        #region Properties

        protected bool IsStrafing
        {
            get => isStrafing;
            set => isStrafing = value;
        }

        public bool IsGrounded { get; set; }
        protected bool IsSprinting { get; set; }
        public bool IsStopped { get; protected set; }
        public bool IsJumping
        {
            get => _isJumping;
            set => _isJumping = value;
        }

        #endregion


        #region Methods

        public virtual void Initialize()
        {
            cachedTransform = transform;
            
            animator = GetComponent<Animator>();
            animator.updateMode = AnimatorUpdateMode.AnimatePhysics;

            // slides the character through walls and edges
            frictionPhysics = new PhysicMaterial();
            frictionPhysics.name = "frictionPhysics";
            frictionPhysics.staticFriction = .25f;
            frictionPhysics.dynamicFriction = .25f;
            frictionPhysics.frictionCombine = PhysicMaterialCombine.Multiply;

            // prevents the collider from slipping on ramps
            maxFrictionPhysics = new PhysicMaterial();
            maxFrictionPhysics.name = "maxFrictionPhysics";
            maxFrictionPhysics.staticFriction = 1f;
            maxFrictionPhysics.dynamicFriction = 1f;
            maxFrictionPhysics.frictionCombine = PhysicMaterialCombine.Maximum;

            // air physics 
            slippyPhysics = new PhysicMaterial();
            slippyPhysics.name = "slippyPhysics";
            slippyPhysics.staticFriction = 0f;
            slippyPhysics.dynamicFriction = 0f;
            slippyPhysics.frictionCombine = PhysicMaterialCombine.Minimum;

            // rigidbody info
            body = GetComponent<Rigidbody>();

            // capsule collider info
            capsuleCollider = GetComponent<CapsuleCollider>();

            // save your collider preferences 
            colliderCenter = GetComponent<CapsuleCollider>().center;
            colliderRadius = GetComponent<CapsuleCollider>().radius;
            colliderHeight = GetComponent<CapsuleCollider>().height;
            IsGrounded = true;
        }

        public void UpdateMotor()
        {
            CheckGround();
            CheckSlopeLimit();
            ControlJumpBehaviour();
            AirControl();
        }

        protected void SetControllerMoveSpeed(MovementSettings settings)
        {
            if (settings.walkByDefault)
                moveSpeed = Mathf.Lerp(moveSpeed, IsSprinting ? settings.runningSpeed : settings.walkSpeed,
                    settings.movementSmooth * Time.deltaTime);
            else
                moveSpeed = Mathf.Lerp(moveSpeed, IsSprinting ? settings.sprintSpeed : settings.runningSpeed,
                    settings.movementSmooth * Time.deltaTime);
        }

        protected void MoveCharacter(Vector3 direction)
        {
            // calculate input smooth
            inputSmooth = Vector3.Lerp(inputSmooth, input,
                (isStrafing ? strafeSettings.movementSmooth : freeSettings.movementSmooth) * Time.deltaTime);

            if (!IsGrounded || IsJumping) return;

            direction.y = 0;
            direction.x = Mathf.Clamp(direction.x, -1f, 1f);
            direction.z = Mathf.Clamp(direction.z, -1f, 1f);

            // limit the input
            if (direction.magnitude > 1f)
                direction.Normalize();

            var targetPosition = (useRootMotion ? animator.rootPosition : body.position) +
                                 direction * ((IsStopped ? 0 : moveSpeed) * Time.deltaTime);
            var targetVelocity = (targetPosition - cachedTransform.position) / Time.deltaTime;

            targetVelocity.y = body.velocity.y;

            body.velocity = targetVelocity;
        }

        protected virtual void CheckSlopeLimit()
        {
            if (input.sqrMagnitude < 0.1) return;

            RaycastHit hitinfo;
            var hitAngle = 0f;

            if (Physics.Linecast(cachedTransform.position + Vector3.up * (capsuleCollider.height * 0.5f),
                cachedTransform.position + moveDirection.normalized * (capsuleCollider.radius + 0.2f), out hitinfo,
                groundLayer))
            {
                hitAngle = Vector3.Angle(Vector3.up, hitinfo.normal);

                var targetPoint = hitinfo.point + moveDirection.normalized * capsuleCollider.radius;
                if ((hitAngle > slopeLimit) &&
                    Physics.Linecast(cachedTransform.position + Vector3.up * (capsuleCollider.height * 0.5f), targetPoint,
                        out hitinfo, groundLayer))
                {
                    hitAngle = Vector3.Angle(Vector3.up, hitinfo.normal);

                    if (hitAngle > slopeLimit && hitAngle < 85f)
                    {
                        IsStopped = true;
                        return;
                    }
                }
            }

            IsStopped = false;
        }

        protected void RotateToPosition(Vector3 position)
        {
            Vector3 desiredDirection = position - cachedTransform.position;
            RotateToDirection(desiredDirection.normalized);
        }

        protected void RotateToDirection(Vector3 direction)
        {
            RotateToDirection(direction, isStrafing ? strafeSettings.rotationSpeed : freeSettings.rotationSpeed);
        }

        private void RotateToDirection(Vector3 direction, float rotationSpeed)
        {
            if (!jumpAndRotate && !IsGrounded) return;
            direction.y = 0f;
            Vector3 desiredForward = Vector3.RotateTowards(cachedTransform.forward, direction.normalized,
                rotationSpeed * Time.deltaTime, .1f);
            Quaternion newRotation = Quaternion.LookRotation(desiredForward);
            cachedTransform.rotation = newRotation;
        }

        protected void ControlJumpBehaviour()
        {
            if (!IsJumping) return;

            jumpCounter -= Time.deltaTime;
            if (jumpCounter <= 0)
            {
                jumpCounter = 0;
                IsJumping = false;
            }

            // apply extra force to the jump height   
            var vel = body.velocity;
            vel.y = jumpHeight;
            body.velocity = vel;
        }

        protected virtual void AirControl()
        {
            if ((IsGrounded && !IsJumping)) return;
            if (cachedTransform.position.y > heightReached) heightReached = cachedTransform.position.y;
            inputSmooth = Vector3.Lerp(inputSmooth, input, airSmooth * Time.deltaTime);

            if (jumpWithRigidbodyForce && !IsGrounded)
            {
                body.AddForce(moveDirection * airSpeed * Time.deltaTime, ForceMode.VelocityChange);
                return;
            }

            moveDirection.y = 0;
            moveDirection.x = Mathf.Clamp(moveDirection.x, -1f, 1f);
            moveDirection.z = Mathf.Clamp(moveDirection.z, -1f, 1f);

            Vector3 targetPosition = body.position + (moveDirection * airSpeed) * Time.deltaTime;
            Vector3 targetVelocity = (targetPosition - cachedTransform.position) / Time.deltaTime;

            targetVelocity.y = body.velocity.y;
            body.velocity = Vector3.Lerp(body.velocity, targetVelocity, airSmooth * Time.deltaTime);
        }

        protected virtual void CheckGround()
        {
            CheckGroundDistance();
            ControlMaterialPhysics();

            if (groundDistance <= groundMinDistance)
            {
                IsGrounded = true;
                if (!IsJumping && groundDistance > 0.05f)
                    body.AddForce(cachedTransform.up * (extraGravity * 2 * Time.deltaTime), ForceMode.VelocityChange);

                heightReached = cachedTransform.position.y;
            }
            else
            {
                if (groundDistance >= groundMaxDistance)
                {
                    IsGrounded = false;
                    verticalVelocity = body.velocity.y;
                    // apply extra gravity when falling
                    if (!IsJumping)
                    {
                        body.AddForce(cachedTransform.up * extraGravity * Time.deltaTime, ForceMode.VelocityChange);
                    }
                }
                else if (!IsJumping)
                {
                    body.AddForce(cachedTransform.up * (extraGravity * 2 * Time.deltaTime), ForceMode.VelocityChange);
                }
            }
        }

        protected virtual void ControlMaterialPhysics()
        {
            // change the physics material to very slip when not grounded
            capsuleCollider.material =
                (IsGrounded && GroundAngle() <= slopeLimit + 1) ? frictionPhysics : slippyPhysics;

            if (IsGrounded && input == Vector3.zero)
                capsuleCollider.material = maxFrictionPhysics;
            else if (IsGrounded && input != Vector3.zero)
                capsuleCollider.material = frictionPhysics;
            else
                capsuleCollider.material = slippyPhysics;
        }

        protected virtual void CheckGroundDistance()
        {
            if (capsuleCollider is null) return;

            // radius of the SphereCast
            float radius = capsuleCollider.radius * 0.9f;
            var dist = 10f;
            // ray for RayCast
            Ray ray2 = new Ray(cachedTransform.position + new Vector3(0, colliderHeight / 2, 0), Vector3.down);
            // raycast for check the ground distance
            if (Physics.Raycast(ray2, out groundHit, (colliderHeight / 2) + dist, groundLayer) &&
                !groundHit.collider.isTrigger)
                dist = cachedTransform.position.y - groundHit.point.y;
            // sphere cast around the base of the capsule to check the ground distance
            if (dist >= groundMinDistance)
            {
                Vector3 pos = cachedTransform.position + Vector3.up * (capsuleCollider.radius);
                Ray ray = new Ray(pos, -Vector3.up);
                if (Physics.SphereCast(ray, radius, out groundHit, capsuleCollider.radius + groundMaxDistance,
                    groundLayer) && !groundHit.collider.isTrigger)
                {
                    Physics.Linecast(groundHit.point + (Vector3.up * 0.1f), groundHit.point + Vector3.down * 0.15f,
                        out groundHit, groundLayer);
                    float newDist = cachedTransform.position.y - groundHit.point.y;
                    if (dist > newDist) dist = newDist;
                }
            }

            groundDistance = (float) System.Math.Round(dist, 2);
        }

        public virtual float GroundAngle()
        {
            var groundAngle = Vector3.Angle(groundHit.normal, Vector3.up);
            return groundAngle;
        }

        #endregion
    }
}