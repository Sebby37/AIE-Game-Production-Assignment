using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Written by Sebastian Cramond
public class Golem : MonoBehaviour
{
    [Header("Movement")]
    public float pursuitForce = 5.0f;
    public float pursuitRadius = 5.5f;
    public bool pursuingPlayer = false;

    [Header("Damage + Health")]
    public Transform healthBar;
    public float health = 5.0f;
    public float knockbackForce = 3.5f;
    public int damageBlinks = 4;
    public float timeBetweenDamageBlinks = 0.125f;

    float maxHealth;
    bool dead = false;
    float healthBarWidth;
    Shader damagedShader;
    Shader normalShader;

    [Header("Animation")]
    public int direction = 1; // Up: 0, Down: 1, Left: 2, Right: 3
    public float animationSpeed = 0.25f;

    // Various components and scripts
    Rigidbody2D rb;
    Animator animator;
    GameObject player;
    SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        // Getting Components
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Getting the player
        player = GameObject.FindGameObjectWithTag("Player");

        // Setting animation variables
        animator.SetFloat("Speed", animationSpeed);

        // Setting the maximum health
        maxHealth = health;

        // Hiding the healthbar
        healthBar.transform.parent.gameObject.SetActive(false);

        // Getting the healthbar width
        healthBarWidth = healthBar.transform.localScale.x;

        // Getting shaders for damage and normal
        damagedShader = Shader.Find("GUI/Text Shader");
        normalShader = Shader.Find("Sprites/Default");
    }

    // Update is called once per frame
    void Update()
    {
        if (!dead)
        {
            // Pursuing player
            PursuePlayer();

            // Updating animations
            UpdateAnimations();
        }
    }

    // Function to pursue the player
    void PursuePlayer()
    {
        // Setting the golem to pursue the player if they are close enough to the player
        pursuingPlayer = Vector2.Distance(transform.position, player.transform.position) <= pursuitRadius;

        // Returning if the player is not being pursued
        if (!pursuingPlayer)
            return;

        // Getting the direction towards the player
        Vector2 directionToPlayer = (player.transform.position - transform.position).normalized;

        // Adding a force towards the player
        rb.AddForce(directionToPlayer * pursuitForce);
    }

    // Function to update animations
    void UpdateAnimations()
    {
        // Setting the animation to idle if not pursuing player
        if (!pursuingPlayer)
            animator.SetTrigger("Idle");

        // Returning if the golem is not moving or there is no animation controller
        if (animator == null || !pursuingPlayer)
            return;

        // Getting the normalised velocity
        Vector2 vel = rb.velocity.normalized;

        // Finding and setting the direction based on the axis of the velocity vector through dot product
        if (Mathf.Abs(vel.y) > Mathf.Abs(vel.x))
        {
            if (Vector2.Dot(transform.up, vel) >= 0)
                direction = 0;
            else if (Vector2.Dot(transform.up, vel) < 0)
                direction = 1;
        }
        else
        {
            if (Vector2.Dot(transform.right, vel) < 0)
                direction = 2;
            else if (Vector2.Dot(transform.right, vel) >= 0)
                direction = 3;
        }

        // Setting animation variables
        animator.SetTrigger("Walk");
        animator.SetInteger("Direction", direction);
    }

    // Function to take damage
    void TakeDamage(float damage, Vector2 attackerDirection)
    {
        // Returning if there is no health left
        if (health <= 0)
            return;

        // Enabling the healthbar
        if (health == maxHealth)
            healthBar.transform.parent.gameObject.SetActive(true);

        // Taking damage
        health -= damage;

        // Setting the width of the healthbar
        Vector3 tempHealthbarScale = healthBar.localScale;
        tempHealthbarScale.x = ((health >= 0.0f ? health : 0.0f) / maxHealth) * healthBarWidth;
        healthBar.localScale = tempHealthbarScale;

        // Dying if there is no health left
        if (health <= 0)
        {
            Die();
            return;
        }

        // Playing the damaged animation
        StartCoroutine(DamagedAnimation());

        // Applying knockback
        rb.AddForce(attackerDirection * knockbackForce, ForceMode2D.Impulse);
    }

    // Function to die
    void Die()
    {
        // Setting the golem to be dead
        dead = true;

        // Playing the death animation
        animator.SetTrigger("Die");
        animator.Play("Die", 0);

        // Disabling the health bar
        healthBar.transform.parent.gameObject.SetActive(false);

        // Setting the golem to be destroyed after 30 seconds
        Destroy(gameObject, 30.0f);

        // Disabling components and resetting velocity
        rb.velocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
        enabled = false;
    }

    // The damage "animation" coroutine
    IEnumerator DamagedAnimation()
    {
        for (int i = 0; i < damageBlinks && !dead; i++)
        {
            // Setting the shader to the damaged shader and waiting for the next blink
            spriteRenderer.material.shader = damagedShader;
            yield return new WaitForSeconds(timeBetweenDamageBlinks);

            // Setting the shader to the normal shader
            spriteRenderer.material.shader = normalShader;
            if (i < damageBlinks - 1) yield return new WaitForSeconds(timeBetweenDamageBlinks);
        }
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
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pursuitRadius);
    }
}
