using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    [Header("Variables")]
    Rigidbody2D rb;                 //Rigidbody controls the physics simulation of the object
    public float jumpForce;         //This is the amount of force used in each jump.
    public Vector2 currentVelocity; //The current velocity of the rigidbody. Needs to be updated each frame.

    // Use this for initialization
    void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update ()
    {

    }
}
