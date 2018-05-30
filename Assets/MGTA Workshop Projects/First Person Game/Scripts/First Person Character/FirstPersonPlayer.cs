using UnityEngine;
using System.Collections;

namespace MGTA
{
    [AddComponentMenu("MGTA/Characters/FirstPerson/FirstPersonPlayer")]
    [RequireComponent(typeof(PlayerInputManager))]
    public class FirstPersonPlayer : CharacterEngine
    {
        void Start()
        {
            base.controller = GetComponent<CharacterController>();
            playerInput = GetComponent<PlayerInputManager>();
            selectedMoveSpeed = walkSpeed;
            locationLastFrame = transform.position;
        }

        void Update()
        {
            //if attempting to move downwards, check for ground.
            if (globalizedMoveDelta.y <= 0) base.GroundCheck();

            slideVelocity = Vector3.zero;

            //JUMP: What do we do when we input a jump?
            //NOTE: This needs to be performed before doing the ISGROUNDED routine below.
            bool isJumping = (playerInput.GetInputDown("Jump")
                && IsGrounded
                && (!IsSliding || (IsSliding && enableJumpWhileSliding)));

            if (isJumping) HandleJump();

            if (playerInput.GetInputUp("Jump")) OnJumpInputUp();

            //IS GROUNDED: What actions do we take when grounded?
            if (IsGrounded) //ON GROUND
            {
                base.CheckSlope();

                if (currentSlope > 0)
                     moveDelta.y = -groundStickFactor;
                else moveDelta.y = 0;

                /*if (!isJumping)
                {
                    if (attachedElevator) verticalMoveDelta = 0; 
                    else verticalMoveDelta = -groundStickFactor; //if firmly on the ground, stick to it.
                }*/

                //if we're grounded on an elevator, make sure we're parented to it.
                if (groundHit.collider && groundHit.collider.CompareTag(elevatorTag))
                {
                    transform.SetParent(groundHit.collider.transform, true);
                    attachedElevator = groundHit.collider.transform;
                }
                else
                {
                    transform.SetParent(null, true);
                    attachedElevator = null;
                }

            }

            //IN AIR: What actions do we take when NOT grounded?
            else
            {
                //STARTED FALLING:
                if (groundedLastFrame && !isJumping)
                {
                    //If we were grounded last frame, and we're not jumping, zero out our velocity.
                    moveDelta.y = 0;
                }

                if (attachedElevator)
                {
                    Debug.Log("became deattached from elevator and not grounded");
                    transform.SetParent(null, true);
                    attachedElevator = null;
                }
            }

            //if air control is not allowed, then retain velocity from the last frame when in air.
            //otherwise, allow movement input.
            if ((IsGrounded) || (enableAirMovement))
                inputVector = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);

            // If running isn't on a toggle, then use the appropriate speed depending on whether the run button is down
            if (toggleSprint)
                if (playerInput.GetInputDown("Sprint")) sprintEnabled = !sprintEnabled;
                else sprintEnabled = playerInput.GetInput("Sprint");

            selectedMoveSpeed = sprintEnabled ? sprintSpeed : walkSpeed;

            //HORIZONTAL MOVE: Determine how much we intend to move on the horizontal plane
            Vector3 horizontalMoveDelta = new Vector3();
            horizontalMoveDelta.x = inputVector.x * selectedMoveSpeed;
            horizontalMoveDelta.z = inputVector.y * selectedMoveSpeed;
            horizontalMoveDelta = Vector3.ClampMagnitude(horizontalMoveDelta, selectedMoveSpeed);
            moveDelta.x = horizontalMoveDelta.x;
            moveDelta.z = horizontalMoveDelta.z;

            //GRAVITY ACCELERATION: apply gravity, if not on an elevator.
            //NOTE: verticalMoveDelta is neutralized if grounded. SEE ABOVE.
            if (!attachedElevator) moveDelta.y -= gravity * Time.deltaTime;

            //SLIDING: if sliding, handle it in a way that makes sense for this controller.
            if (IsSliding) HandleSliding();
            else slideVelocity = Vector3.zero;

            //WORLD MOVEMENT CONVERSION: Movement must be converted to a global delta for CharacterController.Move()
            globalizedMoveDelta = transform.TransformDirection(moveDelta);
        }

        private void LateUpdate()
        {
            controller.Move(((globalizedMoveDelta + slideVelocity) * Time.deltaTime));
            Velocity = (transform.position - locationLastFrame) / Time.deltaTime;
            locationLastFrame = transform.position;
            groundedLastFrame = IsGrounded;
        }

        /// <summary>What happens when set to be sliding?</summary>
        void HandleSliding()
        {
            Vector3 hitNormal = groundHit.normal;
            slideVelocity = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
            Vector3.OrthoNormalize(ref hitNormal, ref slideVelocity);
            slideVelocity *= slideSpeed;
            moveDelta.z = 0;
        }

        /// <summary>What happens when you execute a jump?</summary>
        void HandleJump()
        {
            IsGrounded = false;
            moveDelta.y = JumpSpeed(jumpHeight.max, gravity);
        }

        void OnJumpInputUp()
        {
            float minimumSpeed = JumpSpeed(jumpHeight.min, gravity);
            if (moveDelta.y > minimumSpeed)
                moveDelta.y = minimumSpeed;
        }

        void OnControllerColliderHit(ControllerColliderHit colliderHit)
        {
            controllerHit = colliderHit;

            if ((controller.collisionFlags & CollisionFlags.CollidedAbove) != 0)
            {
                Debug.Log("Controller struck collider above, negating vertical velocity.");
                moveDelta.y = 0;
            }

            if ((controller.collisionFlags & CollisionFlags.CollidedSides) != 0)
            {

            }

            if ((controller.collisionFlags & CollisionFlags.Below) != 0)
            {

            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(slopeHit.point, slopeHit.normal * 2);
        }
    }
}