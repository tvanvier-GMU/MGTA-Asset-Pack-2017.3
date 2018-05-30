using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGTA
{
    public class ThirdPersonPlayer : CharacterEngine
    {

        [Header("Third Person Player")]
        public Transform cam;
        public Transform lockTarget;
        public bool lockCursor;
        public Cinemachine.CinemachineVirtualCamera lockCamera;
        public Cinemachine.CinemachineFreeLook freeLookCamera;

        public float rotationSpeed = 360;
        bool isJumping;

        bool isAttacking;

        public Animator anim;
        public float walkSpeedWhileAttackingMultiplier = .25f;
        public float walkSpeedModificationSpeed = .25f;
        public float moveSpeedModifier = 1;

        // Use this for initialization
        void Start()
        {
            base.controller = GetComponent<CharacterController>();
            playerInput = GetComponent<PlayerInputManager>();
            selectedMoveSpeed = walkSpeed;
            locationLastFrame = transform.position;
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
            Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !lockCursor;
        }

        // Update is called once per frame
        void Update()
        {
            inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            rawInputVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));


            isAttacking = anim.GetCurrentAnimatorStateInfo(0).IsName("SwordCombo");
            if (isAttacking)
            {
                moveSpeedModifier = Mathf.MoveTowards(moveSpeedModifier, walkSpeedWhileAttackingMultiplier, walkSpeedModificationSpeed * Time.deltaTime);
            }
            else
            {
                moveSpeedModifier = Mathf.MoveTowards(moveSpeedModifier, 1, walkSpeedModificationSpeed * Time.deltaTime);
                if (playerInput.GetInput("Block")) anim.Play("Block");
            }

            if (playerInput.GetInput("Attack"))
            {
                if (!isAttacking)
                {
                    anim.Play("SwordCombo");
                }
            }
            if (playerInput.GetInputUp("Attack")) anim.Play("Idle");

            if (playerInput.GetInputDown("LockOn"))
            {
                if (lockTarget == null)
                {
                    //if player attempts lock on, attempt to get closest visible target;
                    LockOnTarget target = LockOnTarget.ClosestVisibleTarget(transform.position);
                    if (target) lockTarget = target.transform;
                }
                else
                    lockTarget = null;
            }

            if (lockTarget)
            {
                lockCamera.Priority = 10;
                //freeLookCamera.gameObject.SetActive(false);
                freeLookCamera.Priority = 0;
                freeLookCamera.m_BindingMode = Cinemachine.CinemachineTransposer.BindingMode.LockToTarget;
                freeLookCamera.m_XAxis.m_MaxSpeed = 0;
            }
            else
            {
                //freeLookCamera.gameObject.SetActive(true);
                lockCamera.Priority = 0;
                freeLookCamera.Priority = 10;
                freeLookCamera.m_BindingMode = Cinemachine.CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp;
                freeLookCamera.m_XAxis.m_MaxSpeed = 200;
            }

            // If running isn't on a toggle, then use the appropriate speed depending on whether the run button is down
            if (toggleSprint)
            {
                if (playerInput.GetInputDown("Sprint")) sprintEnabled = !sprintEnabled;
            }
            else sprintEnabled = playerInput.GetInput("Sprint");

            selectedMoveSpeed = (sprintEnabled ? sprintSpeed : walkSpeed) * moveSpeedModifier;
        }

        private void FixedUpdate()
        {
            if (!attachedElevator) moveDelta.y = -gravity;
            if (lockTarget) LockOnMove();
            else FreeCamMove();
        }

        /*void MoveV1()
        {
            horizontalMoveDelta.x = inputVector.x * selectedMoveSpeed;
            horizontalMoveDelta.z = inputVector.y * selectedMoveSpeed;

            //adapted legacy code
            //targetLookPoint = cam.transform.position + (cam.transform.forward * 100);
            //transform.LookAt(new Vector3(targetLookPoint.x, transform.position.y, targetLookPoint.z));

            Vector3 heading = cam.position - transform.position;
            Vector3 direction = heading / heading.magnitude;
            Vector3 temp = transform.eulerAngles;
            transform.rotation = Quaternion.LookRotation(-direction);
            transform.eulerAngles = new Vector3(temp.x, transform.eulerAngles.y, temp.z);

            localMoveDelta = new Vector3(horizontalMoveDelta.x, verticalMoveDelta, horizontalMoveDelta.z);
            globalizedMoveDelta = transform.TransformDirection(localMoveDelta);
            //if (inputVector.x != 0) modelRotationVector.x = (inputVector.x > 0) ? 1 : -1;
            //if (inputVector.y != 0) modelRotationVector.z = (inputVector.x > 0) ? 1 : -1;
            // model.rotation = transform.rotation * Quaternion.LookRotation(modelRotationVector);

            controller.Move(((globalizedMoveDelta + slideVelocity) * Time.deltaTime));

        }*/
        void OnDisable()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        void FreeCamMove()
        {
            if (rawInputVector.x != 0 || rawInputVector.y != 0)
            {
                float angle = Mathf.Atan2(rawInputVector.x, rawInputVector.y) * Mathf.Rad2Deg;

                Quaternion targetRotation = Quaternion.Euler(0, cam.eulerAngles.y + angle, 0);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            moveDelta.x = 0;
            moveDelta.z = Mathf.Max(Mathf.Abs(inputVector.y), Mathf.Abs(inputVector.x)) * selectedMoveSpeed;
            globalizedMoveDelta = transform.TransformDirection(moveDelta);
            controller.Move(((globalizedMoveDelta + slideVelocity) * Time.deltaTime));
        }

        void LockOnMove()
        {
            Vector3 temp = transform.eulerAngles;
            transform.LookAt(lockTarget);
            transform.eulerAngles = new Vector3(temp.x, transform.eulerAngles.y, temp.z);


            moveDelta.x = inputVector.x * selectedMoveSpeed;
            moveDelta.z = inputVector.y * selectedMoveSpeed;

            globalizedMoveDelta = transform.TransformDirection(moveDelta);
            controller.Move(((globalizedMoveDelta + slideVelocity) * Time.deltaTime));
            //transform.Translate((globalizedMoveDelta + slideVelocity) * Time.fixedDeltaTime);
        }
    }
}