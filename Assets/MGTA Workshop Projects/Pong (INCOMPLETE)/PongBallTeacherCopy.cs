using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PongBallTeacherCopy : MonoBehaviour {

    Rigidbody rig;

    public float startingSpeed = 20;
    public float accelerationOnHit = 1.05f;
    public float maxSpeed = 50f;
    public Vector3 rigVelocity;
    public float hitangleMultiplier = 10;

	// Use this for initialization
	void Start () {
        rig = GetComponent<Rigidbody>();
        rig.velocity = new Vector3(1,Random.Range(-1f, 1f),0) * startingSpeed;
	}
	
	// Update is called once per frame
	void Update () {
        rigVelocity = rig.velocity;
	}

    private void OnCollisionEnter(Collision collision)
    {
        rig.velocity = rig.velocity * accelerationOnHit;
        if (collision.collider.CompareTag("Player"))
        {
            //rig.velocity = accelerationOnHit * Vector3.Reflect(-rig.velocity, collision.contacts[0].normal);
            float distFromCenter = this.transform.position.y - collision.collider.transform.position.y;
            float angleFactor = distFromCenter * hitangleMultiplier;
            float speed = rig.velocity.magnitude;
            rig.velocity = new Vector3(rig.velocity.x, distFromCenter).normalized * speed;
        }

        if (rig.velocity.magnitude > maxSpeed) rig.velocity = rig.velocity.normalized * maxSpeed;
    }
}
