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
    public Transform swordParent;
    public Animator swordAnimator;
    public BoxCollider2D damageHitbox;

    float slashTime;
    float slashTimer = 0;

    [Header("Spell Settings")]
    public static bool canCastFireball = false;
    public Transform castPoint;
    public GameObject fireSpell;

    GameObject currentSpell;

    // Start is called before the first frame update
    void Start()
    {
        // Getting the rigidbody
        rb = GetComponent<Rigidbody2D>();

        // Setting slash parameters
        slashTime = 1 / slashSpeed;
        swordAnimator.SetFloat("Speed", slashSpeed);

        // Setting the player state to still
        SetPlayerState(PlayerMovementStates.Still);
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
                SetPlayerState(PlayerMovementStates.Walking);
            else
                SetPlayerState(PlayerMovementStates.Still);
        }

        // Returning if the player is not walking and setting their velocity to zero
        if (playerState != PlayerMovementStates.Walking)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        // Setting the movement direction based on what keys are pressed
        if (Input.GetKey(KeyCode.W)) SetDirection(Directions.Up);
        else if (Input.GetKey(KeyCode.A)) SetDirection(Directions.Left);
        else if (Input.GetKey(KeyCode.S)) SetDirection(Directions.Down);
        else if (Input.GetKey(KeyCode.D)) SetDirection(Directions.Right);

        // Calculating the velocity of the player based on their movement direction and adding it to a Vector2
        float xVelocity, yVelocity;
        xVelocity = movementDirection == Directions.Left ? -1 : 0;
        xVelocity = movementDirection == Directions.Right ? 1 : xVelocity;

        yVelocity = movementDirection == Directions.Down ? -1 : 0;
        yVelocity = movementDirection == Directions.Up ? 1 : yVelocity;

        Vector2 velocityVector = new Vector2(xVelocity, yVelocity) * movementSpeed;

        // Setting the rigidbody velocity to the velocity vector
        rb.velocity = velocityVector;

        // Setting the rotation of the sword based on the direction
        float rotation = 0;
        if (movementDirection == Directions.Up) rotation = 0;
        if (movementDirection == Directions.Down) rotation = 180;
        if (movementDirection == Directions.Left) rotation = 90;
        if (movementDirection == Directions.Right) rotation = -90;

        swordParent.rotation = Quaternion.Euler(0, 0, rotation);
    }

    // Function to roll
    void UpdateRoll()
    {
        // Setting the player state to rolling if the Q/Shift key is pressed
        if ((Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.LeftShift)) && playerState != PlayerMovementStates.Casting)
        {
            if (rollCooldownTimer < 0) SetPlayerState(PlayerMovementStates.Rolling);
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
            SetPlayerState(PlayerMovementStates.Still);
            rollCooldownTimer = 0;
        }
    }

    // Function to run sword slash code
    void SwordSlash()
    {
        if (Input.GetMouseButtonDown(0) && playerState == PlayerMovementStates.Still)
        {
            SetPlayerState(PlayerMovementStates.Attacking);
            swordAnimator.SetTrigger("Slash");
        }
        
        // Enabling the damager hitbox based on whether the player is attacking
        damageHitbox.enabled = (playerState == PlayerMovementStates.Attacking);

        // Incrementing and resetting the slash timer depending on whether the player is attacking or not
        slashTimer += (playerState == PlayerMovementStates.Attacking) ? Time.deltaTime : 0;
        slashTimer = (playerState == PlayerMovementStates.Attacking) ? slashTimer : 0;

        // Setting the player state depending on whether the slash timer is greater than the slash time
        if (playerState == PlayerMovementStates.Attacking && slashTimer >= slashTime)
            SetPlayerState(PlayerMovementStates.Still);
    }
    
    // Function to cast a fire spell
    void CastFireSpell()
    {

        //This line below was done by Toby Mcdonald to check the current value of the mana, so that the player cannot cast while the current mana is above 10%
        PlayerHealth playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();

        // If the player is still and the cast button is pressed, the player state is set to casting and the spell is created
        if (canCastFireball && playerState == PlayerMovementStates.Still && Input.GetMouseButtonDown(1) && (playerHealth != null && playerHealth.currentMana > 10))
        {
            // Setting the player movement state to casting and instantiating the fire spell
            SetPlayerState(PlayerMovementStates.Casting);
            currentSpell = Instantiate(fireSpell, castPoint.position, transform.rotation);
        }

        // Removing the fireball if the player is no longer casting
        if (playerState != PlayerMovementStates.Casting && currentSpell != null && Input.GetMouseButton(1))
        {
            // Exploding the fireball to get rid of it
            FireSpell currentFire = currentSpell.GetComponent<FireSpell>();
            currentFire.Explode();

            // Setting the current spell to null
            currentSpell = null;
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
            SetPlayerState(PlayerMovementStates.Still);
        }
    }

    // Function to convert a direction enum into an animation direction int
    int DirectionToAnimationInt(Directions direction)
    {
        switch (direction)
        {
            case Directions.Up:
                return 0;
            case Directions.Down:
                return 1;
            case Directions.Left:
                return 2;
            case Directions.Right:
                return 3;
        }

        return 0;
    }
    int DirectionToAnimationInt()
    {
        return DirectionToAnimationInt(movementDirection);
    }

    // Function to set the player's direction
    void SetDirection(Directions direction)
    {
        if (movementDirection == direction)
            return;

        movementDirection = direction;
        int animatorDirection = DirectionToAnimationInt();

        playerAnimator.SetInteger("Direction", animatorDirection);
        SetPlayerAnimationState();
    }

    // Function to set the player's state
    void SetPlayerState(PlayerMovementStates state)
    {
        // Returning if the player's state won't change
        if (playerState == state)
            return;

        // Setting the player's state
        playerState = state;

        // Setting the animation trigger for the state
        SetPlayerAnimationState();
    }

    // Function to update the player's animation state
    void SetPlayerAnimationState(PlayerMovementStates state)
    {
        switch (state)
        {
            case PlayerMovementStates.Still:
                playerAnimator.SetTrigger("Idle");
                break;
            case PlayerMovementStates.Walking:
                playerAnimator.SetTrigger("Walk");
                break;
            case PlayerMovementStates.Rolling:
                playerAnimator.SetTrigger("Roll");
                break;
            case PlayerMovementStates.Attacking:
                playerAnimator.SetTrigger("Slash");
                break;
            case PlayerMovementStates.Casting:
                playerAnimator.SetTrigger("Cast");
                break;
        }
    }
    void SetPlayerAnimationState()
    {
        SetPlayerAnimationState(playerState);
    }

    // Function to die
    [ContextMenu("Die")]
    public void Die()
    {
        // Triggering the death animation
        if (playerAnimator != null)
            playerAnimator.SetTrigger("Die");

        // Disabling the movement script so the player has no control as they are dead (duh)
        enabled = false;
        
        // Disabling the collider so enemies don't knock the player's deceased carcass around
        Collider2D collider = GetComponent<Collider2D>();
        collider.enabled = false;

        // Setting the player's velocity to zero
        rb.velocity = Vector2.zero;
    }

    // Function to enable casting a fireball
    public void EnableFireballCast()
    {
        canCastFireball = true;
    }

    // Function to disable casting a fireball
    public void DisableFireballCast()
    {
        canCastFireball = false;
    }
}
