using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageVolume2D : MonoBehaviour {

    public bool killInstantly = false;
    public int damageDealtOnEnter = 1;
    public bool debugMessages = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlatformerCharacter character = other.gameObject.GetComponent<PlatformerCharacter>();
        if (debugMessages && !character)
            Debug.Log(other.gameObject.name + " entered damageVolume (" + this.gameObject.name + ")");
        if (character)
        {
            if (killInstantly)
                character.Die();
            else
                character.Damage(damageDealtOnEnter);
        }
    }
}
