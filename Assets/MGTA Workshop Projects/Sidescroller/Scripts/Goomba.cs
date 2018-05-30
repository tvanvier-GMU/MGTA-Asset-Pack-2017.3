using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CharacterController2D))]
public class Goomba : StompableEnemy2D {
    public float moveSpeed = 1;
    public float YVelocityOnBounce = 8;
    CharacterController2D controller; 
	// Use this for initialization
	void Start () {
        controller = this.GetComponent<CharacterController2D>();
	}
	
	// Update is called once per frame
	void Update () {
        controller.Move(Vector2.left * moveSpeed * Time.deltaTime, false);
	}

    public override void HitFromAbove()
    {
        base.HitFromAbove();
        PlatformerCharacter.main.moveScript.velocity.y = YVelocityOnBounce;
        Destroy(this.gameObject);
    }
}
