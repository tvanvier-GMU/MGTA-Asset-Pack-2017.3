using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterController2D))]
public class PlatformerCharacterMovement : MonoBehaviour {

    [Header("Walking (Base) Speed")]
    public float baseMoveSpeed = 8;
    public float baseJumpHeight = 5;
    public float minJumpHeight = 1;

    [Header("Sprinting (Bonus) Speed")]
    public float sprintJumpHeight = 8;
    public float sprintSpeed = 16;

    [Header("Gravity")]
    public float gravity = 40;

    [Header("Acceleration Values")]
    public float baseAirAccelerationTime = .4f;
    public float baseGroundAccelerationTime = .2f;
    public float sprintAccelerationTime = 3;

    [Header("Movement Options")]
    public bool allowJumpingWhileSliding = false;

    [Header("Velocity and Input")]
    public Vector3 velocity;
    [SerializeField]
    Vector2 m_directionalInput;
    public Vector2 directionalInput
    {
        get { return m_directionalInput; }
    }

    [Header("Debug")]
    [SerializeField]
    private bool isSprinting = false;
    [SerializeField]
    private bool sprintProtection = false; //prevents loss of sprint bonuses due to collision. Gained by jumping. Needed to work right on slopes.
    private float sprintProtectionTime = .1f;
    private float currentWalkSpeed = 0, currentMaxJumpVelocity, currentMinJumpVelocity, bonusJumpHeight, currentSprintBonus = 0, sprintPercentage, velocityXSmoothing;

    private bool m_jumpedThisFrame;

    public bool jumpedThisFrame
    {
        get { return m_jumpedThisFrame; }
    }

    public bool isGrounded
    {
        get { return controller.collisions.below; }
    }

    public bool isSliding
    {
        get { return controller.collisions.slidingDownMaxSlope; }
    }

    [HideInInspector]
    public CharacterController2D controller;

	void Start()
    {
		controller = GetComponent<CharacterController2D> ();
    }

	void Update()
    {
        GetPlayerInput();
        
		CalculateVelocity ();

		controller.Move (velocity * Time.deltaTime, m_directionalInput.y < 0);

		if (controller.collisions.above || controller.collisions.below) {
			if (controller.collisions.slidingDownMaxSlope) {
				velocity.y += controller.collisions.slopeNormal.y * gravity * Time.deltaTime;
			} else {
				velocity.y = 0;
			}
		}
	}

    void GetPlayerInput()
    {

        m_jumpedThisFrame = false;

        m_directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        //will only change sprinting state when grounded. If you jump while sprinting, you will continue to use sprint speeds until you land.
        if (isGrounded)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                isSprinting = true;
            }
            else
                isSprinting = false;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnJumpInputDown();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            OnJumpInputUp();
        }
    }

	public void OnJumpInputDown() {
		if (isGrounded)
        {
			if (controller.collisions.slidingDownMaxSlope)
            {
                if(allowJumpingWhileSliding == false)
                {
                    return; //simply slide down if not allowed to jump...
                }
                if (m_directionalInput.x != -Mathf.Sign (controller.collisions.slopeNormal.x)) //do not allow jumping towards the slope to prevent abuse
                {
                    velocity.y = currentMaxJumpVelocity * controller.collisions.slopeNormal.y;
					velocity.x = currentMaxJumpVelocity * controller.collisions.slopeNormal.x;
                    m_jumpedThisFrame = true;
                    sprintProtection = true;
                    if (!IsInvoking("RemoveSprintProtection")) Invoke("RemoveSprintProtection", sprintProtectionTime);
				}
			}
            else
            {
                velocity.y = currentMaxJumpVelocity;
                m_jumpedThisFrame = true;
                sprintProtection = true;
                if (!IsInvoking("RemoveSprintProtection")) Invoke("RemoveSprintProtection", sprintProtectionTime);
            }
        }
	}

    void RemoveSprintProtection()
    {
        sprintProtection = false;
    }

	public void OnJumpInputUp() {
		if (velocity.y > currentMinJumpVelocity)
        {
			velocity.y = currentMinJumpVelocity;
		}
	}

    void CalculateVelocity() {

        float maxSprintBonus = sprintSpeed - baseMoveSpeed;

        //if not holding sprint OR you are not moving, decrease sprint bonus (approach zero)
        if (!isSprinting || m_directionalInput.x == 0)
        {
            currentSprintBonus = Mathf.MoveTowards(currentSprintBonus, 0, (maxSprintBonus / (controller.collisions.below ? baseGroundAccelerationTime : baseAirAccelerationTime)) * Time.deltaTime);
        }

        if (m_directionalInput.x != 0)
        {
            //if moving, AND holding sprint, add to sprint bonus.
            if (isSprinting)
                currentSprintBonus += m_directionalInput.x * (maxSprintBonus / sprintAccelerationTime) * Time.deltaTime;

            //handle walk acceleration if moving left / right
            currentWalkSpeed += m_directionalInput.x * (baseMoveSpeed / (controller.collisions.below ? baseGroundAccelerationTime : baseAirAccelerationTime)) * Time.deltaTime;
        }
        else
        {
            currentWalkSpeed = Mathf.MoveTowards(currentWalkSpeed, 0, (baseMoveSpeed / (controller.collisions.below ? baseGroundAccelerationTime : baseAirAccelerationTime)) * Time.deltaTime);
        }

        //clamp speeds to their maximums to prevent endless acceleration
        currentWalkSpeed = Mathf.Clamp(currentWalkSpeed, -baseMoveSpeed, baseMoveSpeed);
        currentSprintBonus = Mathf.Clamp(currentSprintBonus, -maxSprintBonus, maxSprintBonus);

        //Finally, set the X velocity of the character using a combination of base walk value and sprint bonus
        velocity.x = currentWalkSpeed + currentSprintBonus;

        sprintPercentage = Mathf.Abs(currentSprintBonus) / maxSprintBonus;
        bonusJumpHeight = (sprintJumpHeight - baseJumpHeight) * sprintPercentage; //calculate the amount of extra jump we get due to sprinting

        //set the actual min and max jump velocity, using bonuses from the sprint
        currentMaxJumpVelocity = Mathf.Sqrt(2f * (baseJumpHeight + bonusJumpHeight) * Mathf.Abs(gravity));
        currentMinJumpVelocity = Mathf.Sqrt(2f * (minJumpHeight) * Mathf.Abs(gravity));

		velocity.y += -gravity * Time.deltaTime;

        if(velocity.x != 0)
        {
            //if moving into collision, set velocity to zero.
            bool movedIntoCollision = false;
            if((velocity.x < 0 && controller.collisions.left) || (velocity.x > 0 && controller.collisions.right))
            {
                if (!sprintProtection)
                {
                    velocity.x = 0;
                    currentSprintBonus = 0;
                    currentWalkSpeed = 0;
                    bonusJumpHeight = 0;
                    sprintPercentage = 0;
                    isSprinting = false;
                    movedIntoCollision = true;
                }
            }
            //if you change direction, decrease sprint bonus to zero.
            //hitting a wall (from the right) is excluded as a direction change.
            if (!movedIntoCollision && (m_directionalInput.x != 0) && (Mathf.Sign(m_directionalInput.x) != Mathf.Sign(velocity.x)))
            {
                currentSprintBonus = 0;
                Debug.Log("Pivoted Direction, sprint reset");
            }
        }

	}
}
