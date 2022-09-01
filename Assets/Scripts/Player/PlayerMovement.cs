using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private SpriteRenderer spriteRenderer;

    [Header("Walking Settings")]
    public float movementSpeed = 1.4f; // How fast the player walks in units/second
    public Directions movementDirection = Directions.Up; // The direction the player is moving

    [Header("Roll Settings")]
    public float rollTime = 0.35f; // How long a roll will last in seconds
    public float rollSpeedMultiplier = 1.5f; // The speed multiplier at which the player rolls
    public float rollCooldown = 0.5f; // The time in seconds after a roll that the player cannot roll
    float rollTimer = 0;
    float rollCooldownTimer = -1;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = rb.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Updating the player movement
        UpdatePlayerMovement4Directions();

        // Running roll code
        Roll();

        // Updating the player colour
        UpdatePlayerColour();
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
    void Roll()
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

    // Function to update the player's colour based on their movement state
    void UpdatePlayerColour()
    {
        switch (playerState)
        {
            case PlayerMovementStates.Rolling:
                spriteRenderer.color = Color.cyan;
                break;
            default:
                spriteRenderer.color = Color.green;
                break;
        }
    }
}
