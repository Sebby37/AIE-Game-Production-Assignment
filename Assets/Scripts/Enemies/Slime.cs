using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Written by Sebastian Cramond
public class Slime : MonoBehaviour
{
    [Header("Leap Settings")]
    public bool animationIsLeaping = false;

    public float leapCooldown = 1.0f;
    public float pursueLeapDistance = 3.0f;

    // Local leap variables
    float leapTime;
    float leapVelocity;
    float leapCooldownTimer = -1.0f;
    Vector2 leapDirection;

    [Header("Health + Damage Settings")]
    public Transform healthBar;

    public float health = 3.0f;
    public float knockback = 3.0f;
    public float knockbackTime = 0.2f;

    // Local Health + Damage variables
    float maxHealth;
    float healthbarWidth;
    Vector2 knockbackVelocity;
    float knockbackTimer = -1.0f;
    
    // Components
    Rigidbody2D rb;
    Animator animator;
    GameObject player;
    
    // Start is called before the first frame update
    void Start()
    {
        // Calculating leap variables
        leapTime = 38f / 60f;

        // Calculating health + damage variables
        maxHealth = health;
        healthbarWidth = healthBar.localScale.x;

        // Hiding the healthbar as the slime is full health
        healthBar.transform.parent.gameObject.SetActive(false);

        // Getting the components
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // Checking whether the leaping animation is playing
        animationIsLeaping = AnimationIsPlaying("Jump Airborne");

        // Incrementing the knockback timer and resetting it when it exceedes the knockback timer
        knockbackTimer += (knockbackTimer >= 0.0f && knockbackTimer <= knockbackTime) ? Time.deltaTime : 0.0f;
        knockbackTimer = (knockbackTimer >= knockbackTime) ? -1.0f : knockbackTimer;

        // Setting leap and knockback velocity
        if (animationIsLeaping && knockbackTimer < 0.0f)
            rb.velocity = leapDirection * leapVelocity;
        else if (knockbackTimer >= 0.0f && knockbackTimer < knockbackTime)
            rb.velocity = knockbackVelocity;
        else
            rb.velocity = Vector3.zero;

        // Incrementing the leap cooldown timer and resetting it if it exceedes the cooldown time
        leapCooldownTimer += (leapCooldownTimer >= 0.0f && leapCooldownTimer <= leapCooldown + 1.5f) ? Time.deltaTime : 0.0f;
        leapCooldownTimer = (leapCooldownTimer >= leapCooldown) ? -1.0f : leapCooldownTimer;

        // Checking if the player is near, pursuing them if so, idling if not
        if (Vector2.Distance(transform.position, player.transform.position) <= pursueLeapDistance)
            PursuePlayer();
        else
            IdleMovement();

        // TESTING PURPOSES - Leaping in the direction of the mouse pointer
        //if (Input.GetMouseButtonDown(0)) Leap(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    // Function to trigger a leap
    void Leap(Vector2 leapPosition)
    {
        // Returning if a leap is already in progress or the slime is dead
        if (animationIsLeaping && health <= 0 && leapCooldownTimer >= 0.0f) return;
        
        // Triggering the jump animation code
        animator.SetTrigger("Jump");

        // Calculating the leap direction and velocity
        leapVelocity = pursueLeapDistance / leapTime;
        leapDirection = (leapPosition - (Vector2)transform.position).normalized;

        // Disabling the knockback timer
        knockbackTimer = -1.0f;

        // Enabling the leap cooldown timer
        leapCooldownTimer = 0.0f;
    }

    // Function to check if an animation is playing
    bool AnimationIsPlaying(string animationName, int layerIndex = 0, int clipIndex = 0)
    {
        if (animator.GetCurrentAnimatorClipInfo(layerIndex).Length <= 0) return false;
        return animator.GetCurrentAnimatorClipInfo(layerIndex)[clipIndex].clip.name == animationName;
    }

    // Code for trigger entering (Basically player sword swing)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Checking if the trigger is that of the player's sword
        if (collision.CompareTag("Player Damager") && health > 0)
        {
            // Getting the PlayerMovement class, returning if there is none
            PlayerMovement player = collision.GetComponentInParent<PlayerMovement>();
            if (player == null) return;

            // Finding the vector that points towards the player to apply knockback
            Vector2 directionToPlayer = (transform.position - player.transform.position).normalized;

            // Triggering the damage function
            TakeDamage(player.damage, directionToPlayer);
        }
    }

    // Function for taking damage
    void TakeDamage(float damage, Vector2 knockbackVector)
    {
        // Returning if there is no health left
        if (health <= 0) return;

        // Showing the healthbar if this is the first time damage was dealt
        if (health == maxHealth) healthBar.transform.parent.gameObject.SetActive(true);

        // Subtracting the damage from the health
        health -= damage;

        // Lowering the health bar
        Vector3 tempHealthbarScale = healthBar.localScale;
        tempHealthbarScale.x -= damage / maxHealth * healthbarWidth;
        healthBar.localScale = tempHealthbarScale;

        // Dying if the health is <= 0
        if (health <= 0)
        {
            Die();
            return;
        }

        // Applying knockback
        knockbackVelocity = knockbackVector * knockback;
        knockbackTimer = 0.0f;

        // Triggering the knockback animation
        // This doesn't look very good, but there isn't really a better animation at this moment
        animator.SetTrigger("Knockback");
    }

    // Function to die
    void Die()
    {
        // Triggering the death animation
        animator.SetTrigger("Die");

        // Destroying the slime after the animation finishes
        Destroy(gameObject, 0.75f);

        // Disabling the collision and behavior components
        enabled = false;
        GetComponent<Collider2D>().enabled = false;
    }

    // Function to move idle
    void IdleMovement()
    {
        // Returning if the leap is cooling down
        if (leapCooldownTimer >= 0.0f) return;
        
        // Creating a random movement vector
        Vector2 movementVector = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));

        // Leaping in the random vector
        Leap(movementVector);
    }

    // Function to pursue player
    void PursuePlayer()
    {
        // Returning if the leap is cooling down
        if (leapCooldownTimer >= 0.0f) return;

        // Creating a vector that points towards the player
        Vector2 movementVector = (player.transform.position - transform.position).normalized;

        // Leaping towards the player
        Leap(movementVector);
    }
}
