using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SidescrollerCheckpoint : MonoBehaviour {

    public bool triggered = false;
    SpriteRenderer rend;

    // Use this for initialization
    void Start()
    {
        rend = this.GetComponent<SpriteRenderer>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlatformerCharacter character = other.gameObject.GetComponent<PlatformerCharacter>();
        if (character && !triggered)
        {
            triggered = true;
            character.currentCheckpointPosition = this.transform.position;
            if(rend) rend.color = Color.blue;
        }
    }
}
