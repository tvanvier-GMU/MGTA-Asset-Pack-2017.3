using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SidescrollerAnimationController : MonoBehaviour {

    public PlatformerCharacterMovement myPlayer;
    public Animator myAnimator;

    public float runVelocityScale = .15f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
        int animationDirectionScale = 1;
        if (myPlayer.directionalInput.x < 0)
        {
            myAnimator.transform.rotation = Quaternion.Euler(0, 180, 0);
            animationDirectionScale = -1;
        }
        else if (myPlayer.directionalInput.x > 0)
        {
            myAnimator.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        if (!myPlayer.isGrounded || myPlayer.isSliding)
        {
            //falling
            myAnimator.SetBool("isGrounded", false);
        }
        else
        {
            myAnimator.SetBool("isGrounded", true);
        }

        if(myPlayer.jumpedThisFrame)
        {
            //jumped!
            //myAnimator.Play("Robot_Jump");
        }


        if (myPlayer.isGrounded && myPlayer.velocity.x != 0)
        {
            //running
            myAnimator.SetBool("isRunning", true);
            myAnimator.SetFloat("runSpeed", myPlayer.velocity.x * runVelocityScale * animationDirectionScale);
        }
        else
        {
            myAnimator.SetBool("isRunning", false);
        }
    }
}
