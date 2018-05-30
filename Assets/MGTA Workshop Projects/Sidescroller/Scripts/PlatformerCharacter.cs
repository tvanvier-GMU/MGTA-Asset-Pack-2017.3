using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlatformerCharacterMovement))]
[RequireComponent(typeof(CharacterController2D))]
public class PlatformerCharacter : MonoBehaviour {

    public static PlatformerCharacter main;

    [Header("Damage and Health")]
    public int health = 3;
    public int maxHealth = 3;
    public int coins = 0;
    public float invulnOnHitDuration = 1.5f;
    public bool temporarilyInvulnerable;
    bool dead;

    [Header("Sound FX")]
    public AudioSource soundSource;
    public AudioClip deathSound;

    [HideInInspector]
    public PlatformerCharacterMovement moveScript;
    [HideInInspector]
    public CharacterController2D controllerScript;

    [Header("Checkpoints and Respawn")]
    public Vector3 currentCheckpointPosition;
    public float respawnDelay = 2;

    [Header("UI")]
    public UnityEngine.UI.Image[] healthImages;
    public UnityEngine.UI.Text coinCountText;

    [Header("Damage Animation")]
    public Animator damageAnimator;

    [Header("Debug")]
    public bool godMode = false;

    private void Awake()
    {
        if (main == null) main = this;
        else Debug.LogError("More than one platformer character exists in scene. PlatformerCharacter.main will access only one instance.");

        moveScript = this.GetComponent<PlatformerCharacterMovement>();
        controllerScript = this.GetComponent<CharacterController2D>();
        currentCheckpointPosition = transform.position;
    }

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// Call this when you're out of health.
    /// </summary>
    public void Die()
    {
        if (dead) return;
        dead = true;
        soundSource.PlayOneShot(deathSound);
        moveScript.enabled = false;
        Invoke("Respawn", respawnDelay);
    }

    /// <summary>
    /// Call this when running into an enemy or trap to take damage
    /// </summary>
    /// <param name="amount"></param>
    public void Damage(int amount)
    {
        if (godMode || temporarilyInvulnerable || dead) return;

        health -= amount;

        for (int i = 0; i < healthImages.Length; i++)
        {
            if (i >= health) healthImages[i].enabled = false;
        }
        if (health <= 0) Die();
        else StartCoroutine(HitInvulnerabilityRoutine());
    }

    /// <summary>
    /// Call this when picking up items to recover health.
    /// </summary>
    /// <param name="amount"></param>
    public void Heal(int amount)
    {
        if (health >= maxHealth) return;

        health += amount;
        if (health > maxHealth) health = maxHealth;

        for (int i = 0; i < healthImages.Length; i++)
        {
            if (i < health) healthImages[i].enabled = true;
        }
    }
    
    /// <summary>
    /// Add coin to inventory
    /// </summary>
    public void AddCoin(int amount)
    {
        coins += amount;
        coinCountText.text = "x " + coins.ToString();
    }

    IEnumerator HitInvulnerabilityRoutine()
    {
        temporarilyInvulnerable = true;
        damageAnimator.Play("SidescrollerPlayerDamage");
        yield return new WaitForSeconds(invulnOnHitDuration);
        temporarilyInvulnerable = false;
        damageAnimator.Play("Default State");
    }

    public void Respawn()
    {
        dead = false;
        transform.position = currentCheckpointPosition;
        moveScript.enabled = true;
        Heal(3);
        moveScript.velocity = Vector3.zero;
    }
}
