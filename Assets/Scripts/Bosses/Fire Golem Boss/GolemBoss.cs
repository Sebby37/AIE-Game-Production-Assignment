using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GolemStates
{
    Inactive,
    Intro,
    Idle,
    Dead,
    Jumping,
    Attacking
}

// Written by Sebastian Cramond
public class GolemBoss : MonoBehaviour
{
    [Header("State Information")]
    public GolemStates state = GolemStates.Inactive;
    public GameObject healthBarCanvas;

    [Header("Animation Config")]
    public float animationSpeed = 1.0f;
    public int animationSampleRate = 24;

    [Header("Health Config")]
    public float health = 30.0f;
    public int damageBlinks = 4;
    public float timeBetweenDamageBlinks = 0.125f;
    public RectTransform healthBar;

    float maxHealth;
    // I'm using shaders to make the boss blink white when damaged
    // Because setting the sprite's colour to white gives it it's
    // Normal colours
    Shader damagedShader;
    Shader normalShader;

    [Header("Boss AI Behaviour")]
    public float timeBetweenAttacks = 5.0f;

    int previousAttack = 0;

    [Header("Attack 1 Config")]
    public GameObject fireBall;
    public int castFireBallFrame = 0;
    public Vector2 fireBallCastOffset;
    public int throwFireBallFrame = 15;
    public float fireBallSpeed = 3.5f;

    [Header("Attack 2 Config")]
    public int groundPoundFrame = 8;
    public GameObject slamObject;
    public float slamObjectLifetime = 1.0f;
    public int slamsPerAttack = 3;
    public float timeBetweenSlamSpawns = 0.5f;
    public Vector2 slamSpawnOffset;

    [Header("Attack 3 Config")]
    public int dashBeginFrame = 6;
    public int dashEndFrame = 10;
    public float dashSpeed = 5.0f;
    public int dashTimes = 3;
    public float timeBetweenDashes = 0.5f;

    SpriteRenderer spriteRenderer;
    Animator animator;
    Rigidbody2D rb;
    
    // Start is called before the first frame update
    void Start()
    {
        // Getting Components (See what I did there??!?!?!??)
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // Getting shaders for damage and normal
        damagedShader = Shader.Find("GUI/Text Shader");
        normalShader = Shader.Find("Sprites/Default");

        // Setting variables
        maxHealth = health;
    }

    // Update is called once per frame
    void Update()
    {
        // Setting the animation speed variable in the animation controller each frame
        animator.SetFloat("Speed", animationSpeed);

        // Debug keys to trigger attacks
        if (false)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                Attack(1);

            if (Input.GetKeyDown(KeyCode.Alpha2))
                Attack(2);

            if (Input.GetKeyDown(KeyCode.Alpha3))
                Attack(3);

            if (Input.GetKeyDown(KeyCode.R))
                TakeDamage(1);
        }
    }

    // Function to begin the Intro
    [ContextMenu("Begin Boss Battle")]
    public void Intro()
    {
        // Setting the state
        state = GolemStates.Intro;

        // Triggering the animation
        animator.SetTrigger("Intro");

        // Enabling the health bar canvas
        healthBarCanvas.SetActive(true);

        // Starting the intro coroutine
        StartCoroutine(IntroCoroutine());
    }

    // Function to attack
    void Attack(int attackNumber)
    {
        // Returning if the current state is not idle
        if (state != GolemStates.Idle)
            return;

        // Setting the state
        state = GolemStates.Attacking;

        // Attacking based on the attack's number
        switch (attackNumber)
        {
            case 1:
                StartCoroutine(Attack1Coroutine());
                break;
            case 2:
                StartCoroutine(Attack2Coroutine());
                break;
            case 3:
                StartCoroutine(Attack3Coroutine());
                break;
            default:
                state = GolemStates.Idle;
                break;
        }
    }

    // Function to take damage
    void TakeDamage(float takenDamage)
    {
        // Returning if ボスは死
        if (state == GolemStates.Dead || health <= 0)
            return;

        // Subtracting the health
        health -= takenDamage;

        // Lowering the health bar
        float newScale = health >= 0.0f ? health / maxHealth : 0.0f;
        healthBar.localScale = new Vector3(newScale, 1.0f, 1.0f);
        healthBar.localPosition = new Vector3(300.0f * newScale - 300.0f, healthBar.localPosition.y, healthBar.localPosition.z);

        // Dying if health is less than or equal to zero
        if (health <= 0)
            Die();
        // Starting the damaged "animation" coroutine if the boss isn't dead
        else
            StartCoroutine(DamagedAnimation());
    }

    // Function to die
    void Die()
    {
        // Setting the state to dead
        state = GolemStates.Dead;

        // Triggering the death animation
        animator.SetTrigger("Die");

        // Disabling the collider and setting the velocity to zero
        GetComponent<Collider2D>().enabled = false;
        rb.velocity = Vector2.zero;
    }

    // The intro coroutine
    IEnumerator IntroCoroutine()
    {
        // Waiting for the intro animation to end
        yield return new WaitForSeconds(1.0f);

        // Setting the state
        state = GolemStates.Idle;

        // Starting the attack timing coroutine
        StartCoroutine(AttackTimingCoroutine());
    }

    // The damage "animation" coroutine
    IEnumerator DamagedAnimation()
    {
        for (int i = 0; i < damageBlinks; i++)
        {
            // Setting the shader to the damaged shader and waiting for the next blink
            spriteRenderer.material.shader = damagedShader;
            yield return new WaitForSeconds(timeBetweenDamageBlinks);

            // Setting the shader to the normal shader
            spriteRenderer.material.shader = normalShader;
            if (i < damageBlinks - 1) yield return new WaitForSeconds(timeBetweenDamageBlinks);
        }
    }

    // The attack timing coroutine
    IEnumerator AttackTimingCoroutine()
    {
        while (true)
        {
            // Waiting to start the next attack
            yield return new WaitForSeconds(timeBetweenAttacks);

            // Getting a random attack to perform
            int chosenAttack = previousAttack;
            while (chosenAttack == previousAttack)
                chosenAttack = Random.Range(1, 4);

            previousAttack = chosenAttack;

            // Performing the chosen attack
            Attack(chosenAttack);

            // Waiting for the golem boss to be idle before beginning another attack
            while (state != GolemStates.Idle)
            {
                // Golem is attacking
                yield return null;
            }
        }
    }

    // The Attack 1 Coroutine
    IEnumerator Attack1Coroutine()
    {
        // Triggering the animation
        animator.SetTrigger("Attack 1");

        // Waiting until the fireball needs to be cast
        float timeToWait = ((float)castFireBallFrame / (float)animationSampleRate) * (1 / animationSpeed);
        yield return new WaitForSeconds(timeToWait);

        // Instantiating the fireball
        GameObject newFireBall = Instantiate(fireBall, (Vector2) transform.position + fireBallCastOffset, Quaternion.identity);

        // Making sure the fireball ignores collision with the boss
        Physics2D.IgnoreCollision(newFireBall.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        // Waiting until the fireball is to be thrown
        timeToWait = ((float)(throwFireBallFrame - castFireBallFrame) / (float)animationSampleRate) * (1 / animationSpeed);
        yield return new WaitForSeconds(timeToWait);
        
        // Throwing the fireball
        BossFireBall fireBallComponent = newFireBall.GetComponent<BossFireBall>();
        fireBallComponent.Throw(fireBallSpeed, gameObject);

        // Setting the attack state to idle as the attack is complete
        state = GolemStates.Idle;
    }

    // The Attack 2 Coroutine
    IEnumerator Attack2Coroutine()
    {
        // Triggering the animation
        animator.SetTrigger("Attack 2");

        // Waiting until the ground pound is to happen
        float timeToWait = ((float) groundPoundFrame / (float) animationSampleRate) * (1 / animationSpeed);
        yield return new WaitForSeconds(timeToWait);

        // Getting the player's GameObject to spawn slam attacks on them
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // Spawning slam objects
        for (int i = 0; i < slamsPerAttack; i++)
        {
            // Instantiating the object at the player's position
            GameObject currentSlam = Instantiate(slamObject, (Vector2) player.transform.position + slamSpawnOffset, Quaternion.identity);

            // Setting the slam object to be destroyed after an interval
            Destroy(currentSlam, slamObjectLifetime);

            // Waiting to spawn the next slam object
            if (i < slamsPerAttack - 1)
                yield return new WaitForSeconds(timeBetweenSlamSpawns);
        }
        
        // Setting the attack state to idle as the attack is complete
        state = GolemStates.Idle;
    }

    // The Attack 3 Coroutine
    IEnumerator Attack3Coroutine()
    {
        for (int i = 0; i < dashTimes; i++)
        {
            // Triggering the animation
            animator.SetTrigger("Attack 3");

            // Finding the direction to the player
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            Vector2 directionToPlayer = (((Vector2)player.transform.position + slamSpawnOffset) - (Vector2)transform.position).normalized;

            // Waiting until the beginning of the attack behaviour
            float timeToWait = ((float)dashBeginFrame / (float)animationSampleRate) * (1 / animationSpeed);
            yield return new WaitForSeconds(timeToWait);

            // Setting the golem's velocity
            rb.velocity = directionToPlayer * dashSpeed;

            // Waiting until the end of the dash behaviour
            timeToWait = ((float)(dashEndFrame - dashBeginFrame) / (float)animationSampleRate) * (1 / animationSpeed);
            yield return new WaitForSeconds(timeToWait);

            // Resetting the velocity to zero as the dash is complete
            rb.velocity = Vector2.zero;

            // Setting the attack state to idle as the attack is complete
            state = GolemStates.Idle;

            // Delaying for the next dash
            if (i < dashTimes - 1)
                yield return new WaitForSeconds(timeBetweenDashes);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If the collision is that of the player's sword
        if (collision.CompareTag("Player Damager") && health > 0)
        {
            // Getting the PlayerMovement class, returning if there is none
            PlayerMovement player = collision.GetComponentInParent<PlayerMovement>();
            if (player == null) return;

            // Triggering the damage function
            TakeDamage(player.damage);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Checking if the collider is a fireball
        if (collision.gameObject.CompareTag("Fire Ball"))
        {
            // The damage to take
            float? damageToTake = null;
            
            // Getting the damage from the FireSpell component
            FireSpell fireBall = collision.gameObject.GetComponent<FireSpell>();
            if (fireBall != null)
                damageToTake = fireBall.damage;

            // Getting the damage from the BossFireBall component
            BossFireBall bossFireBall = collision.gameObject.GetComponent<BossFireBall>();
            if (bossFireBall != null)
                damageToTake = bossFireBall.bossDamage;

            // Damaging the boss based on the fireball's damage
            if (damageToTake != null)
                TakeDamage((float) damageToTake);
        }
    }
}
