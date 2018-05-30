using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeacherCopies
{
    public class PlayerMovementTeacherCopy : MonoBehaviour
    {
        Rigidbody2D rb;
        public float jumpForce = 1250; //Play around with this value to get it to feel right.
        public Vector2 currentVelocity;

        // Use this for initialization
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        // Update is called once per frame
        void Update()
        {
            currentVelocity = rb.velocity;      //Add this so you can see the velocity of the rigidbody!

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                rb.velocity = Vector2.zero;
                rb.AddForce(Vector2.up * jumpForce);
            }
        }
    }
}
