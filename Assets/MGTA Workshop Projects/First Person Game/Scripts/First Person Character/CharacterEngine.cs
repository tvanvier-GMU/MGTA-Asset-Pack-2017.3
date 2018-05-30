using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGTA
{
    [RequireComponent(typeof(CharacterController))]
    public class CharacterEngine : MonoBehaviour
    {

        protected CharacterController controller;
        protected PlayerInputManager playerInput;

        [Header("Character Engine")]
        public float walkSpeed = 6.0f;
        public float sprintSpeed = 10.0f;
        protected float selectedMoveSpeed;
        public MinMaxValue jumpHeight = new MinMaxValue(1, 2.5f);

        public float gravity = 20.0f;
        public bool enableAirMovement = false;          //If checked, then the player can change direction while in the air
        public bool toggleSprint = true;
        protected bool sprintEnabled = false;

        [Header("Slope")]
        public bool slideWhenOverSlopeLimit = false;    //When on a slope greater than the controller's slope limit, will slide down it.
        [Range(0, 90)]
        public float maxSlidableSlopeAngle = 80;
        public float slideSpeed = 10;
        public bool enableJumpWhileSliding = false;
        [SerializeField]
        protected float currentSlope;

        [Header("Advanced")]
        public Transform attachedElevator;              //elevator that the player is parented to.
        public string elevatorTag = "Elevator";

        [SerializeField] protected Vector3 moveDelta;

        /// <summary>
        /// Horizontal (X,Z) move delta is set directly each frame, based on current input axes.
        /// Vertical (Y) move delta is retained each frame when falling, as gravity accelerates it downwards. 
        /// When grounded, is set directly to the groundStickFactor. Reset when a fall begins.
        /// </summary>
        /// 

        [Header("Grounding")]
        public float groundTolerance = .1f;             //Distance that the ground raycasting extends past the bottom of the controller.
        public float groundCastRadius = .5f;            //Radius of the spherecast that detects the ground.
        public float groundCastOriginOffset = .5f;      //Offset of the spherecast from the bottom of the player.
        public LayerMask groundCastMask;                //Determines which layers can be considered 'ground'
                                                        /// <summary>
                                                        /// Downwards velocity set on the player when grounded. 
                                                        /// Prevents skipping down ramps! 
                                                        /// </summary>
        public float groundStickFactor = 3f;

        protected Vector2 inputVector, rawInputVector;
        protected Vector3 localMoveDelta, globalizedMoveDelta, slideVelocity;
        protected Vector3 locationLastFrame;
        protected bool hitGroundWithSphereCast, hitGroundFromGroundPoint;

        [Header("Debug")]
        [SerializeField]
        protected bool m_sliding = false;
        public bool IsSliding { get { return m_sliding; } protected set { m_sliding = value; } }

        [SerializeField]
        protected bool m_grounded = false;
        public bool IsGrounded { get { return m_grounded; } protected set { m_grounded = value; } }

        [SerializeField]
        protected bool m_groundedLastFrame;
        public bool groundedLastFrame { get { return m_groundedLastFrame; } protected set { m_groundedLastFrame = value; } }

        [SerializeField]
        protected Vector3 m_velocity; //the actual difference in movement each frame.
        public Vector3 Velocity { get { return m_velocity; } protected set { m_velocity = value; } }

        protected ControllerColliderHit controllerHit;
        protected RaycastHit groundHit, slopeHit;

        [System.Serializable]
        public struct MinMaxValue
        {
            public float min;
            public float max;
            public MinMaxValue(float _min, float _max)
            {
                this.min = _min;
                this.max = _max;
            }
        }

        /// <summary>GroundCheck casts a sphere downwards to determine if the controller is 'Grounded'. Goes a little past the controller's capsule, based on the groundTolerance.</summary>
        protected void GroundCheck()
        {
            hitGroundWithSphereCast = false;
            hitGroundFromGroundPoint = false;

            //GROUNDCAST to detect whether or not we are above ground. There is a tolerance so that you can jump a little bit before you actually hit ground.
            //if (Physics.Raycast(transform.position, Vector3.down, out slideHit, controller.height * .5f + controller.radius))
            if (Physics.SphereCast(transform.position + new Vector3(0, controller.skinWidth + groundCastOriginOffset, 0), groundCastRadius, Vector3.down, out groundHit, groundTolerance + controller.skinWidth, groundCastMask))
            {
                hitGroundWithSphereCast = true;
            }
            else if (Physics.Raycast(transform.position, Vector3.down, out groundHit, groundTolerance, groundCastMask))
            {
                hitGroundFromGroundPoint = true;
            }
            IsGrounded = (hitGroundWithSphereCast || hitGroundFromGroundPoint);

        }

        /// <summary>
        /// Check if the surface immediately below should be slid down. If not already sliding, uses a raycast from the center of the player.
        /// Until the player STOPS sliding, it will raycast from the point at which the controller is touching the slope.
        /// CheckSlope should be called only when grounded.
        ///</summary>
        protected void CheckSlope()
        {
            if (!IsSliding || (controllerHit.collider == null))
            {
                //if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, controller.height * .5f + controller.radius))
                if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, 1, groundCastMask))
                {
                    currentSlope = Vector3.Angle(slopeHit.normal, Vector3.up);
                }
            }
            else
            {
                Physics.Raycast(controllerHit.point + Vector3.up, Vector3.down, out slopeHit);
                currentSlope = Vector3.Angle(slopeHit.normal, Vector3.up);
            }

            if (currentSlope > controller.slopeLimit)
            {
                if ((currentSlope > controller.slopeLimit) && (slideWhenOverSlopeLimit))
                    IsSliding = true;
                else
                    IsSliding = false;
            }
            else
                IsSliding = false;
        }

        protected float JumpSpeed(float jumpHeight, float gravity)
        {
            return Mathf.Sqrt(2 * jumpHeight * gravity);
        }
    }
}