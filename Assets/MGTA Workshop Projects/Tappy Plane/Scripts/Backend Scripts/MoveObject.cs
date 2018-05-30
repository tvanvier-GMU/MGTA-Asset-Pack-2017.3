using UnityEngine;
using System.Collections;

public class MoveObject : MonoBehaviour {
	public bool usePooling;
	public bool destroyOnEnd = false;

	public float speed;

	public float startX;
	public float endX;

	public bool playerDead = false;

	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {
		if(playerDead == false){
			Vector3 direction = new Vector3(-1,0,0); //Equal to Vector3.Left
			transform.Translate(direction * speed * Time.deltaTime, Space.World);

			if(usePooling){
				if(transform.position.x <= endX){
					transform.position = new Vector3(startX, transform.position.y, transform.position.z);
				}
			}

			if(destroyOnEnd){
				if(transform.position.x <= endX){
					Destroy(this.gameObject);
				}
			}
		}
	}

	public void PlayerDeath(){
		playerDead = true;
	}

}
