using UnityEngine;
using System.Collections;

public class CharacterController2D : RaycastController {

    [Tooltip("Chosen layer only registers collision on the bottom of the character.")]
    public string oneWayPlatformLayer = "OneWayPlatform";

    [Range(0, 360)]
    public float maxSlopeAngle = 45;

    [SerializeField]
    private bool m_downwardsInput;

    public bool downwardsInput
    {
        get { return m_downwardsInput; }
    }

    public CollisionInfo collisions;

    [Header("Physics Debug (CAUTION: Affects global 2D Physics)")]
    public bool forceNoQueriesInsideColliders = true;
    public bool forceNoRaycastHitTriggers = true;

    public override void Start() {
		base.Start ();
		collisions.faceDir = 1;

        if (forceNoQueriesInsideColliders)
            Physics2D.queriesStartInColliders = false;
        if (forceNoRaycastHitTriggers)
            Physics2D.queriesHitTriggers = false;
        if (Physics2D.queriesStartInColliders)
            Debug.LogWarning("CharacterController2D: Physics2D.queriesStartInColliders is currently TRUE. This could lead to unwanted behavior when inside one-way-platform objects.");
        if (Physics2D.queriesHitTriggers)
            Debug.LogWarning("CharacterController2D: Physics2D.queriesHitTriggers is currently TRUE. This could lead to unwanted behavior, such as players colliding with triggers.");
	}

	public void MoveWithPlatform(Vector2 moveAmount, bool standingOnPlatform) {
		Move (moveAmount, false, standingOnPlatform);
	}

    public void Move(Vector2 moveAmount, bool downwardsInput, bool standingOnPlatform = false) {

        UpdateRaycastOrigins();

		collisions.Reset ();
		collisions.moveAmountOld = moveAmount;
		m_downwardsInput = downwardsInput;

		if (moveAmount.y < 0) {
			DescendSlope(ref moveAmount);
        }

        if (moveAmount.x != 0) {
			collisions.faceDir = (int)Mathf.Sign(moveAmount.x);
		}

		HorizontalCollisions (ref moveAmount);

        if (moveAmount.y != 0) {
			VerticalCollisions (ref moveAmount);
            SlopeFootCheck(ref moveAmount);
		}

		transform.Translate (moveAmount);

		if (standingOnPlatform) {
			collisions.below = true;
		}
	}

	void HorizontalCollisions(ref Vector2 moveAmount) {
		float directionX = collisions.faceDir;
		float rayLength = Mathf.Abs (moveAmount.x) + skinWidth;

        //create a layerMask from collisionMask, but exclude OneWayPlatforms on the sides.
        LayerMask adjustedMaskHorizontal = LayerMaskExtensions.RemoveFromMask(collisionMask, oneWayPlatformLayer);

		if (Mathf.Abs(moveAmount.x) < skinWidth) {
		//	rayLength = 2*skinWidth;
		}

		for (int i = 0; i < horizontalRayCount; i ++) {

            //disables the bottom horizontal collision check when moving upwards
            //This was causing the player to 'hit' the slope when jumping, cancelling its velocity
            //if (i == 0 && rawMoveAmount.y > 0) continue;

			Vector2 rayOrigin = (directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, adjustedMaskHorizontal);
			//Debug.DrawRay(rayOrigin, Vector2.right * directionX / 2, Color.white);
            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);
            if (hit) {

				if (hit.distance == 0) {
					continue;
				}

				float forwardSlopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                //collisions.slopeAngle = forwardSlopeAngle;

                //forward hit from foot has detected slope ahead
				if (i == 0 && forwardSlopeAngle <= maxSlopeAngle) {
					if (collisions.descendingSlope) {
                        Debug.Log("Case 1");
						collisions.descendingSlope = false;
						moveAmount = collisions.moveAmountOld;
					}
					float distanceToSlopeStart = 0;
                    //slope has changed!
					if (forwardSlopeAngle != collisions.slopeAngleOld) {
                        //Debug.Log("Slope ahead of player changed from " + collisions.slopeAngleOld + " to " + forwardSlopeAngle);
						distanceToSlopeStart = hit.distance-skinWidth;
						moveAmount.x -= distanceToSlopeStart * directionX;
					}

					ClimbSlope(ref moveAmount, forwardSlopeAngle, hit.normal);
					moveAmount.x += distanceToSlopeStart * directionX;
				}

				if (!collisions.climbingSlope || forwardSlopeAngle > maxSlopeAngle) {
                    moveAmount.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;

					if (collisions.climbingSlope) {
						moveAmount.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);
					}

					collisions.left = directionX == -1;
					collisions.right = directionX == 1;
                    if (collisions.left) collisions.leftCollider = hit.collider;
                    if (collisions.right) collisions.rightCollider = hit.collider;
                }
			}
		}
	}

	void VerticalCollisions(ref Vector2 moveAmount) {
		float directionY = Mathf.Sign (moveAmount.y);
		float rayLength = Mathf.Abs (moveAmount.y) + skinWidth;

        //if falling through platforms, remove oneWayPlatform layer from the mask.
        LayerMask adjustedMaskVertical = collisionMask;
        if (collisions.fallingThroughPlatform)
        {
            adjustedMaskVertical = LayerMaskExtensions.RemoveFromMask(adjustedMaskVertical, oneWayPlatformLayer);
            Debug.Log(adjustedMaskVertical.ToString());
        }

		for (int i = 0; i < verticalRayCount; i ++) {

			Vector2 rayOrigin = (directionY == -1)?raycastOrigins.bottomLeft:raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, adjustedMaskVertical);

			//Debug.DrawRay(rayOrigin, Vector2.up * directionY / 2, Color.white);
            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            if (hit) {
                //if the Layer matches the name of the OneWayPlatformLayer, then ignore it when going up.
				if (LayerMask.LayerToName(hit.collider.gameObject.layer) == oneWayPlatformLayer) {
					if (directionY == 1 || hit.distance == 0) {
						continue;
					}

                    //BUG: 

                    //disregard hits if falling through platform.
					//if (collisions.fallingThroughPlatform) {
						//continue;
					//}

                    //if the player inputs down, will pass through oneway platforms briefly.
					if (downwardsInput) {
						collisions.fallingThroughPlatform = true;
						Invoke("ResetFallingThroughPlatform",.5f);
						continue;
					}
				}

                //main collision property
				moveAmount.y = (hit.distance - skinWidth) * directionY;
				rayLength = hit.distance;

				if (collisions.climbingSlope) {
                    Debug.Log("Vertical Collisions Climbing Slope moveAmountX mod");
					moveAmount.x = moveAmount.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveAmount.x);
				}

				collisions.below = directionY == -1;
				collisions.above = directionY == 1;

                if (collisions.below) collisions.belowCollider = hit.collider;
                if (collisions.above) collisions.aboveCollider = hit.collider;
			}
		}
	}

    void SlopeFootCheck(ref Vector2 moveAmount)
    {
        //float directionY = Mathf.Sign(moveAmount.y);
        float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;
        if (collisions.climbingSlope)
        {
            float directionX = Mathf.Sign(moveAmount.x);
            rayLength = Mathf.Abs(moveAmount.x) + skinWidth;
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * moveAmount.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
            Debug.DrawRay(rayOrigin, Vector2.right * directionX * .01f, Color.green);
            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != collisions.slopeAngle)
                {
                    Debug.Log("Horizontal SlopeFootCheck has detected change in slope.");
                    moveAmount.x = (hit.distance - skinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                    collisions.slopeNormal = hit.normal;
                }
            }
        }
    }

    //horizontal foot check.
	void ClimbSlope(ref Vector2 moveAmount, float slopeAngle, Vector2 slopeNormal) {
		float moveDistance = Mathf.Abs (moveAmount.x);
		float climbmoveAmountY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;

		if (moveAmount.y <= climbmoveAmountY) {
            Debug.Log("Climb Slope Adjusting");
			moveAmount.y = climbmoveAmountY;
			moveAmount.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (moveAmount.x);
			collisions.below = true;
			collisions.climbingSlope = true;
			collisions.slopeAngle = slopeAngle;
			collisions.slopeNormal = slopeNormal;
		}
	}

	void DescendSlope(ref Vector2 moveAmount) {
        //cast rays from the bottom left and bottom right of the character
		RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast (raycastOrigins.bottomLeft, Vector2.down, Mathf.Abs (moveAmount.y) + skinWidth, collisionMask);
		RaycastHit2D maxSlopeHitRight = Physics2D.Raycast (raycastOrigins.bottomRight, Vector2.down, Mathf.Abs (moveAmount.y) + skinWidth, collisionMask);
        Debug.DrawRay(raycastOrigins.bottomLeft, Vector2.down * (Mathf.Abs(moveAmount.y) + skinWidth) + Vector2.down * .1f, Color.yellow);
        Debug.DrawRay(raycastOrigins.bottomRight, Vector2.down * (Mathf.Abs(moveAmount.y) + skinWidth) + Vector2.down * .1f, Color.yellow);

        //if ONLY ONE of the two is true... Implies we are on slope.
        if (maxSlopeHitLeft ^ maxSlopeHitRight) {
            Debug.Log("On Slope");
			SlideDownMaxSlope (maxSlopeHitLeft, ref moveAmount);
			SlideDownMaxSlope (maxSlopeHitRight, ref moveAmount);
		}

        //if not auto-sliding...
		if (!collisions.slidingDownMaxSlope) {
			float directionX = Mathf.Sign (moveAmount.x);
			Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);
            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != 0 && slopeAngle <= maxSlopeAngle)
                {
                    if (Mathf.Sign(hit.normal.x) == directionX)
                    {
                        if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x))
                        {
                            float moveDistance = Mathf.Abs(moveAmount.x);
                            float descendmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
                            moveAmount.y -= descendmoveAmountY;
                            collisions.slopeAngle = slopeAngle;
                            collisions.descendingSlope = true;
                            collisions.below = true;
                            collisions.slopeNormal = hit.normal;
                        }
                    }
                }
            }
		}
	}

	void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 moveAmount) {
		if (hit) {
			float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
			if (slopeAngle > maxSlopeAngle) {
				moveAmount.x = Mathf.Sign(hit.normal.x) * (Mathf.Abs (moveAmount.y) - hit.distance) / Mathf.Tan (slopeAngle * Mathf.Deg2Rad);

				collisions.slopeAngle = slopeAngle;
				collisions.slidingDownMaxSlope = true;
				collisions.slopeNormal = hit.normal;
			}
		}

	}

	void ResetFallingThroughPlatform() {
		collisions.fallingThroughPlatform = false;
	}

    [System.Serializable]
	public struct CollisionInfo {
		public bool above, below;
		public bool left, right;

		public bool climbingSlope;
		public bool descendingSlope;
		public bool slidingDownMaxSlope;

		public float slopeAngle, slopeAngleOld;
		public Vector2 slopeNormal;
		public Vector2 moveAmountOld;
		public int faceDir;
		public bool fallingThroughPlatform;

        public Collider2D aboveCollider, belowCollider, leftCollider, rightCollider;

		public void Reset() {
			above = below = false;
			left = right = false;
			climbingSlope = false;
			descendingSlope = false;
			slidingDownMaxSlope = false;
			slopeNormal = Vector2.zero;

            aboveCollider = belowCollider = leftCollider = rightCollider = null;

			slopeAngleOld = slopeAngle;
			slopeAngle = 0;
		}
	}

}
