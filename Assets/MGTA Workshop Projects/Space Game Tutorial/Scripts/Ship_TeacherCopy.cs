using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeacherCopies
{
    public class Ship_TeacherCopy : MonoBehaviour
    {

        public GameObject laserboltPrefab;          // firing our blaster creates copies of this prefab! Not the optimized solution, but easy to understand.
        public Transform firePointA, firePointB;    // the alternating locations that we fire a projectile from
        private bool firePointToggle = false;       // controls which location we fire from.

        public float boltVelocity;                  // speed at which the laserbolt travels
        public float moveSpeed = 1;                 // speed at which the ship travels

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            HandleMovement();

            if (Input.GetKeyDown(KeyCode.Space)) Fire();
        }

        void HandleMovement()
        {
            Vector3 directionThisFrame = Vector3.zero;       //start out with no direction, unless we're pressing buttons!

            if (Input.GetKey(KeyCode.RightArrow))
            {
                //what do we do if we press the right arrow?
                directionThisFrame.x += 1;
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                //what do we do if we press the left arrow?
                directionThisFrame.x += -1;
            }

            if (Input.GetKey(KeyCode.UpArrow))
            {
                //what do we do if we press the up arrow?
                directionThisFrame.y += 1;
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                //what do we do if we press the down arrow?
                directionThisFrame.y += -1;
            }

            this.transform.Translate(directionThisFrame * moveSpeed * Time.deltaTime);
        }


        /// <summary>
        /// This function fires projectiles from the active firePoint.
        /// </summary>
        void Fire()
        {
            Transform firePoint = firePointToggle ? firePointA : firePointB;    //Ternary Operator. Is it toggled? Use A. Otherwise, use B.
            GameObject newInstance = Instantiate<GameObject>(laserboltPrefab, firePoint.position, firePoint.rotation); //creates the new laserbolt
            Rigidbody2D rb = newInstance.GetComponent<Rigidbody2D>();           //get the rigidbody from the laserbolt gameobject
            rb.velocity = rb.transform.up * boltVelocity;                  //set the speed of the new laserbolt
            firePointToggle = !firePointToggle;                                 //toggles the firePoint we're using
            Destroy(newInstance, 10);
        }
    }
}