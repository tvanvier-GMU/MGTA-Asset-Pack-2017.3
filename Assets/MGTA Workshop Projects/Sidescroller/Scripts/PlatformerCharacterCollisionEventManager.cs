using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController2D))]
public class PlatformerCharacterCollisionEventManager : MonoBehaviour {
    CharacterController2D controller;

    Collider2D lastAboveCollider;
    Collider2D lastBelowCollider;

	void Start () {
        controller = GetComponent<CharacterController2D>();
	}
	
    //NOTE: Make sure that movement is called on the character in UPDATE. or this breaks
	void LateUpdate () {
        if (controller.collisions.aboveCollider)
        {
            if (controller.collisions.aboveCollider != lastAboveCollider)
            {
                HittableBlock2D hitBlock = controller.collisions.aboveCollider.GetComponent<HittableBlock2D>();
                if (hitBlock) hitBlock.HitFromBelow();
                lastAboveCollider = controller.collisions.aboveCollider;
            }
        }
        else { lastAboveCollider = null; }

        if (controller.collisions.belowCollider)
        {
            if (controller.collisions.belowCollider != lastBelowCollider)
            {
                StompableEnemy2D stompedEnemy = controller.collisions.belowCollider.GetComponent<StompableEnemy2D>();
                if (stompedEnemy) stompedEnemy.HitFromAbove();
                lastBelowCollider = controller.collisions.belowCollider;
            }
        }
        else { lastBelowCollider = null; }
    }
}
