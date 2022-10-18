using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Written by Sebastian Cramond
// Maybe move this to a seperate file
public enum Directions
{
    Up,
    Down,
    Left,
    Right
}

// Player movement states enum
public enum PlayerMovementStates
{
    Still,
    Walking,
    Rolling,
    Attacking,
    Casting
}

public class PlayerMovement : MonoBehaviour
{
    [Header("Player State Settings")]
    public PlayerMovementStates playerState = PlayerMovementStates.Still;
    
    private Rigidbody2D rb;

    [Header("Sprite + Animation Settings")]
    public SpriteRenderer playerSprite;
    public Animator playerAnimator;

    [Header("Walking Settings")]
    public float movementSpeed = 1.4f; // How fast the player walks in units/second
    public Directions movementDirection = Directions.Up; // The direction the player is moving

    [Header("Roll Settings")]
    public float rollTime = 0.35f; // How long a roll will last in seconds
    public float rollSpeedMultiplier = 1.5f; // The speed multiplier at which the player rolls
    public float rollCooldown = 0.5f; // The time in seconds after a roll that the player cannot roll
    
    float rollTimer = 0;
    float rollCooldownTimer = -1;

    [Header("Attack Settings")]
    public float damage = 1;
    public float slashSpeed = 6;
    public Animator swordAnimator;
    public BoxCollider2D damageHitbox;

    float slashTime;
    float slashTimer = 0;

    [Header("Spell Settings")]
    public Transform castPoint;
    public GameObject fireSpell;

    GameObject currentSpell;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        swordAnimator = GetComponent<Animator>();

        slashTime = 1 / slashSpeed;
        swordAnimator.SetFloat("Slash Speed", slashSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        // Function to update the spell casting code
        CastFireSpell();

        // Function to update the sword slash code
        SwordSlash();

        // Updating the player movement
        UpdatePlayerMovement4Directions();

        // Running roll code
        UpdateRoll();
    }

    // Function to update player movement in 4 directions (Retro Zelda style)
    void UpdatePlayerMovement4Directions()
    {
        // Setting the player state to walking if any of the movement keys are pressed and if they are not performing another action
        if (playerState == PlayerMovementStates.Still || playerState == PlayerMovementStates.Walking)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
                playerState = PlayerMovementStates.Walking;
            else 
                playerState = PlayerMovementStates.Still;
        }

        // Returning if the player is not walking and setting their velocity to zero
        if (playerState != PlayerMovementStates.Walking)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        // Setting the movement direction based on what keys are pressed
        if (Input.GetKey(KeyCode.W)) movementDirection = Directions.Up;
        if (Input.GetKey(KeyCode.A)) movementDirection = Directions.Left;
        if (Input.GetKey(KeyCode.S)) movementDirection = Directions.Down;
        if (Input.GetKey(KeyCode.D)) movementDirection = Directions.Right;

        // Calculating the velocity of the player based on their movement direction and adding it to a Vector2
        float xVelocity, yVelocity;
        xVelocity = movementDirection == Directions.Left ? -1 : 0;
        xVelocity = movementDirection == Directions.Right ? 1 : xVelocity;

        yVelocity = movementDirection == Directions.Down ? -1 : 0;
        yVelocity = movementDirection == Directions.Up ? 1 : yVelocity;

        Vector2 velocityVector = new Vector2(xVelocity, yVelocity) * movementSpeed;

        // Setting the rigidbody velocity to the velocity vector
        rb.velocity = velocityVector;

        // Setting the rotation of the player based on their direction
        float rotation = 0;
        if (movementDirection == Directions.Up) rotation = 0;
        if (movementDirection == Directions.Down) rotation = 180;
        if (movementDirection == Directions.Left) rotation = 90;
        if (movementDirection == Directions.Right) rotation = -90;

        transform.rotation = Quaternion.Euler(0, 0, rotation);
    }

    // Function to roll
    void UpdateRoll()
    {
        // Setting the player state to rolling if the Q key is pressed
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (rollCooldownTimer < 0) playerState = PlayerMovementStates.Rolling;
        }
        
        // Updating the roll cooldown timer and resetting it if it counts up to it's maximum
        if (rollCooldownTimer >= 0) rollCooldownTimer += Time.deltaTime;
        if (rollCooldownTimer >= rollCooldown) rollCooldownTimer = -1;

        // Returning if the player is not rolling, as to not run roll update code
        if (playerState != PlayerMovementStates.Rolling)
        {
            rollTimer = 0;
            return;
        }

        // Calculating the direction of the player as floats of -1 to 1
        float xVelocity, yVelocity;
        xVelocity = movementDirection == Directions.Left ? -1 : 0;
        xVelocity = movementDirection == Directions.Right ? 1 : xVelocity;

        yVelocity = movementDirection == Directions.Down ? -1 : 0;
        yVelocity = movementDirection == Directions.Up ? 1 : yVelocity;

        // Adding the velocity to the rigidbody and updating the timer
        rb.velocity = new Vector2(xVelocity, yVelocity) * movementSpeed * rollSpeedMultiplier;
        rollTimer += Time.deltaTime;

        // Stopping the roll if the time has exceeded the roll time
        if (rollTimer > rollTime)
        {
            playerState = PlayerMovementStates.Still;
            rollCooldownTimer = 0;
        }
    }

    // TEMPORARY UNTIL PROPER SLASH SCRIPT IS WRITTEN
    // Function to run sword slash code
    void SwordSlash()
    {
        if (Input.GetMouseButtonDown(0) && playerState == PlayerMovementStates.Still)
        {
            playerState = PlayerMovementStates.Attacking;
            swordAnimator.SetTrigger("Slash");
        }
        
        // Enabling the damager hitbox based on whether the player is attacking
        damageHitbox.enabled = (playerState == PlayerMovementStates.Attacking);

        // Incrementing and resetting the slash timer depending on whether the player is attacking or not
        slashTimer += (playerState == PlayerMovementStates.Attacking) ? Time.deltaTime : 0;
        slashTimer = (playerState == PlayerMovementStates.Attacking) ? slashTimer : 0;

        // Setting the player state depending on whether the slash timer is greater than the slash time
        playerState = (playerState == PlayerMovementStates.Attacking && slashTimer >= slashTime) ? PlayerMovementStates.Still : playerState;
    }

    // Function to cast a fire spell
    void CastFireSpell()
    {
        // If the player is still and the cast button is pressed, the player state is set to casting and the spell is created
        if (playerState == PlayerMovementStates.Still && Input.GetMouseButtonDown(1))
        {
            // Setting the player movement state to casting and instantiating the fire spell
            playerState = PlayerMovementStates.Casting;
            currentSpell = Instantiate(fireSpell, castPoint.position, transform.rotation);
        }

        // If the player is casting and the cast button is released, the spell is thrown and the player is set to still
        if (playerState == PlayerMovementStates.Casting && Input.GetMouseButtonUp(1))
        {
            // Throwing the FireBall if the variable is not null
            if (currentSpell != null)
            {
                // Getting the FireSpell component and calling the throw function
                FireSpell currentFire = currentSpell.GetComponent<FireSpell>();
                currentFire.playerPosition = transform.position;
                currentFire.castByPlayer = true;
                currentFire.Throw(GetComponent<Collider2D>());
            }

            // Clearing the currentSpell variable
            currentSpell = null;

            // Resetting the player movement state to still
            playerState = PlayerMovementStates.Still;
        }
    }
}
