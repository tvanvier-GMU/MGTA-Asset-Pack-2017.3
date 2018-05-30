using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

	public Canvas menuCanvas;
	public AudioSource mainTrack;

	public bool hidden = false;

	void Awake(){
		Time.timeScale = 0;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space)){
			if(hidden == false){
				Unfreeze();
			}
		}
	}

	public void Unfreeze(){
		hidden = true;
		Time.timeScale = 1;
		mainTrack.Play();
		menuCanvas.enabled = false;
	}
}
