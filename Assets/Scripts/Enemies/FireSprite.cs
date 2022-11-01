using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSprite : MonoBehaviour
{
    [Header("Movement Settings")]
    public float pursuitDistance = 5.0f;
    public float pursuitSpeed = 1.0f;

    Vector2 directionToPlayer;
    bool pursuingPlayer = false;

    [Header("Attack Settings")]
    public GameObject fireballPrefab;
    public float timeBetweenAttacks = 3.5f;
    public float fireballSpeedMultiplier = 0.5f;

    float attackTimer = 0.0f;

    [Header("Health + Damage")]
    public Transform healthBar;
    public float health = 2;
    public float knockback = 3.0f;
    public float knockbackTime = 0.2f;

    Vector2 knockbackVelocity;
    float knockbackTimer;
    float healthbarWidth;
    float maxHealth;

    Rigidbody2D rb;
    Animator animator;
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        // Getting components and objects
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");

        // Setting default objects
        maxHealth = health;
        healthbarWidth = healthBar.localScale.x;

        // Hiding the healthbar as the fire sprite is at full health
        healthBar.transform.parent.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Setting the pursuing player bool
        pursuingPlayer = (Vector2.Distance(transform.position, player.transform.position) <= pursuitDistance && health > 0);

        // Getting the direction to the player (This is zero if the player is not being pursued
        if (pursuingPlayer)
            directionToPlayer = (player.transform.position - transform.position).normalized;
        else
            directionToPlayer = Vector2.zero;

        // Moving towards the player
        if (knockbackTimer >= 0.0f && knockbackTimer < knockbackTime)
            rb.velocity = knockbackVelocity;
        else
            rb.velocity = directionToPlayer * pursuitSpeed;

        // Incrementing the attack timer
        attackTimer += pursuingPlayer && health > 0 ? Time.deltaTime : 0.0f;

        // Throwing a fireball if the attack timer has exceeded the time between attacks
        if (attackTimer >= timeBetweenAttacks)
            AttackPlayer();

        // Incrementing the knockback timer and resetting it when it exceedes the knockback timer
        knockbackTimer += (knockbackTimer >= 0.0f && knockbackTimer <= knockbackTime) ? Time.deltaTime : 0.0f;
        knockbackTimer = (knockbackTimer >= knockbackTime) ? -1.0f : knockbackTimer;
    }

    // Function to attack the player
    [ContextMenu("Attack Player")]
    void AttackPlayer()
    {
        // Returning if the fireball is not attacking and the distance to player is zero
        if (!pursuingPlayer || directionToPlayer == Vector2.zero)
            return;

        // Playing the attack animation
        animator.SetTrigger("Attack");

        // Resetting the attack timer
        attackTimer = 0.0f;

        // Beginning the attack coroutine
        StartCoroutine(AttackAfterDelay());
    }

    // Coroutine to throw an attack after a delay for animation
    IEnumerator AttackAfterDelay()
    {
        yield return new WaitForSeconds((13.0f / 60.0f) * (1.0f / 0.3f));

        // Returning if the fireball is not attacking and the distance to player is zero
        if (!pursuingPlayer || directionToPlayer == Vector2.zero)
            yield return null;

        // Creating the fireball
        GameObject fireball = Instantiate(fireballPrefab);
        FireSpell fireComponent = fireball.GetComponent<FireSpell>();
        fireball.transform.position = transform.position;

        // Throwing the fireball and setting some variables
        fireComponent.playerPosition = player.transform.position;
        fireComponent.speed *= fireballSpeedMultiplier;
        fireComponent.Throw(GetComponent<Collider2D>(), directionToPlayer);
    }

    // Function to be damaged
    void TakeDamage(float damage, Vector2? knockbackVector = null)
    {
        // Returning if there is no health left
        if (health <= 0) return;

        // Showing the healthbar
        if (health == maxHealth) healthBar.transform.parent.gameObject.SetActive(true);

        // Subtracting the damage from the health
        health -= damage;

        // Lowering the health bar
        Vector3 tempHealthbarScale = healthBar.localScale;
        tempHealthbarScale.x -= damage / maxHealth * healthbarWidth;
        healthBar.localScale = tempHealthbarScale;

        // If there is no more health left, the player dies
        if (health <= 0)
        {
            Die();
            return;
        }

        // Applying knockback
        knockbackVelocity = (Vector2) knockbackVector * knockback;
        knockbackTimer = 0.0f;
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

        // Setting the velocity to zero
        rb.velocity = Vector2.zero;
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

    // Code for collision entering (Basically fireball collisions)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Checking if the collider is a fireball
        if (collision.gameObject.CompareTag("Fire Ball"))
        {
            // Getting the FireSpell class, returning if there is none
            FireSpell fireBall = collision.gameObject.GetComponent<FireSpell>();
            if (fireBall == null) return;

            // Finding the vector that points towards the fireball to apply knockback
            Vector2 directionToFireBall = (transform.position - fireBall.transform.position).normalized;

            // Triggering the damage function
            TakeDamage(fireBall.damage, directionToFireBall);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = pursuingPlayer ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, pursuitDistance);
    }
}
