using UnityEngine;
using System.Collections;

public class SidescrollerCamera : MonoBehaviour {

    [Header("Follow Target")]
	public PlatformerCharacter target;

    [Header("Camera Properties")]
	//public float offsetY = 1;
    //public float offsetX = 0;
    public Vector2 offset = new Vector2(0, 1);
    //TODO: SET MINIMUM Y VALUE FOR CAMERA SO THAT IT DOESN'T FOLLOW INTO ABYSS
	public float lookAheadDistanceX = 4;
	public float lookSmoothTimeX = .5f;
	public float verticalSmoothTime = .1f;

    public bool useMinimumHeight = true;
    public float minimumVerticalPosition = -1;

    [Header("Focus Area")]
    public bool showFocusAreaGizmo = true;
	public Vector2 focusAreaSize = new Vector2(3, 5);
    public Color focusAreaColor = new Color(1, 0, 0, .5f);

    FocusArea focusArea;

	public float currentLookAheadX;
	public float targetLookAheadX;
	public float lookAheadDirX;
	float smoothLookVelocityX;
	float smoothVelocityY;

	bool lookAheadStopped;

	void Start() {

        if (!target) {
            Debug.LogError("No target assigned to Sidescroller Camera");
            return;
        }
		focusArea = new FocusArea (target.controllerScript.boxCollider.bounds, focusAreaSize);
	}

	void LateUpdate() {
        if (!target) return;

		focusArea.Update (target.controllerScript.boxCollider.bounds);

        //Vector2 focusPosition = focusArea.centre + Vector2.up * offsetY;
        Vector2 focusPosition = focusArea.centre + offset;

        if (focusArea.velocity.x != 0) {
			lookAheadDirX = Mathf.Sign (focusArea.velocity.x);
			if (Mathf.Sign(target.moveScript.directionalInput.x) == Mathf.Sign(focusArea.velocity.x) && target.moveScript.directionalInput.x != 0) {
				lookAheadStopped = false;
				targetLookAheadX = lookAheadDirX * lookAheadDistanceX;
			}
			else {
				if (!lookAheadStopped) {
					lookAheadStopped = true;
					targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadDistanceX - currentLookAheadX)/4f;
				}
			}
		}


		currentLookAheadX = Mathf.SmoothDamp (currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);

		focusPosition.y = Mathf.SmoothDamp (transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);
		focusPosition += Vector2.right * currentLookAheadX;
		transform.position = (Vector3)focusPosition + Vector3.forward * -10;

        if (useMinimumHeight && transform.position.y < minimumVerticalPosition)
            transform.position = new Vector3(transform.position.x, minimumVerticalPosition, transform.position.z);
	}

	void OnDrawGizmos() {
        Gizmos.color = focusAreaColor;
		Gizmos.DrawCube (focusArea.centre, focusAreaSize);
	}

	struct FocusArea {
		public Vector2 centre;
		public Vector2 velocity;
		float left,right;
		float top,bottom;


		public FocusArea(Bounds targetBounds, Vector2 size) {
			left = targetBounds.center.x - size.x/2;
			right = targetBounds.center.x + size.x/2;
			bottom = targetBounds.min.y;
			top = targetBounds.min.y + size.y;

			velocity = Vector2.zero;
			centre = new Vector2((left+right)/2,(top +bottom)/2);
		}

		public void Update(Bounds targetBounds) {
			float shiftX = 0;
			if (targetBounds.min.x < left) {
				shiftX = targetBounds.min.x - left;
			} else if (targetBounds.max.x > right) {
				shiftX = targetBounds.max.x - right;
			}
			left += shiftX;
			right += shiftX;

			float shiftY = 0;
			if (targetBounds.min.y < bottom) {
				shiftY = targetBounds.min.y - bottom;
			} else if (targetBounds.max.y > top) {
				shiftY = targetBounds.max.y - top;
			}
			top += shiftY;
			bottom += shiftY;
			centre = new Vector2((left+right)/2,(top +bottom)/2);
			velocity = new Vector2 (shiftX, shiftY);
		}
	}

}
