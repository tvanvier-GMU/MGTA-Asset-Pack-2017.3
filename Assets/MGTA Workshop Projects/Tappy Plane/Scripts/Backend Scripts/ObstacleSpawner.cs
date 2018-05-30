using UnityEngine;
using System.Collections;

public class ObstacleSpawner : MonoBehaviour {
	public float spawnInterval;

	public float timer;

	public GameObject[] obstacles;

	public bool playerDead = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(playerDead == false){
			timer += Time.deltaTime;
			if(timer >= spawnInterval){
				int index = Random.Range(0, obstacles.Length);
				Instantiate(obstacles[index], transform.position, Quaternion.identity);
				timer = 0f;
			}
		}
	}

	public void PlayerDeath(){
		playerDead = true;
	}

}
