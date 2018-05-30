using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerPickup : MonoBehaviour {

    public int coinValue = 1;
    public int healthValue = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlatformerCharacter>())
        {
            PlatformerCharacter.main.AddCoin(coinValue);
            PlatformerCharacter.main.Heal(healthValue);
            Destroy(this.gameObject);
        }
    }
}
