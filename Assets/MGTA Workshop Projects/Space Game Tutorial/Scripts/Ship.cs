using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour {

    public GameObject laserboltPrefab;          // firing our blaster creates copies of this prefab! Not the optimized solution, but easy to understand.
    public Transform firePointA, firePointB;    // the alternating locations that we fire a projectile from
    private bool firePointToggle = false;       // controls which location we fire from.

    public float boltVelocity;                  // speed at which the laserbolt travels
    public float moveSpeed = 1;                 // speed at which the ship travels

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
 
	}


    void HandleMovement()
    {
        Vector3 directionThisFrame = Vector3.zero;       //start out with no direction, unless we're pressing buttons!

        if (Input.GetKey(KeyCode.RightArrow))
        {
            //what do we do if we press the right arrow?

        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            //what do we do if we press the left arrow?

        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            //what do we do if we press the up arrow?

        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            //what do we do if we press the down arrow?

        }

        //What happens when multiple buttons are held?

        this.transform.Translate(directionThisFrame * moveSpeed);        //multiply the 
    }


    /// <summary>
    /// This function fires projectiles from the active firePoint.
    /// </summary>
    void Fire()
    {
        Transform firePoint = firePointToggle ? firePointA : firePointB;    //Ternary Operator. Is it toggled? Use A. Otherwise, use B.
        GameObject newInstance = Instantiate<GameObject>(laserboltPrefab, firePoint.position, firePoint.rotation); //creates the new laserbolt
        Rigidbody2D rb = newInstance.GetComponent<Rigidbody2D>();           //get the rigidbody from the laserbolt gameobject
        rb.velocity = rb.transform.forward * boltVelocity;                  //set the speed of the new laserbolt
        firePointToggle = !firePointToggle;                                 //toggles the firePoint we're using
        Destroy(newInstance, 10);
    }
}
