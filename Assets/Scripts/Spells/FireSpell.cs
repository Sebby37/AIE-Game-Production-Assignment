using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Written by Sebastian Cramond
public enum FireSpellState
{
    Casting,
    Thrown
}

public class FireSpell : MonoBehaviour
{
    [Header("Projectile Behaviour")]
    public float damage = 2;
    public float speed = 3.0f;
    public float lifeTime = 5.0f;
    public bool castByPlayer = false;

    public float curveTowardsMouseForce = 1.0f;

    public Vector3 playerPosition;
    Rigidbody2D rb;
    float timeSinceCreation = 0.0f;
    FireSpellState state = FireSpellState.Casting;
    
    [Header("Animation Parameters")]
    public float castSpeed = 1.0f;
    public float idleSpeed = 1.0f;

    Animator animator;
    Collider2D fireCollider;

    // Start is called before the first frame update
    void Start()
    {
        // Getting the rigidbody of the fireball
        rb = GetComponent<Rigidbody2D>();

        // Getting the collider component of the fireball
        fireCollider = GetComponent<Collider2D>();
        
        // Setting animator values and running the cast animation
        animator = GetComponent<Animator>();
        animator.SetFloat("Cast Speed", castSpeed);
        animator.SetFloat("Idle Speed", idleSpeed);
        animator.SetTrigger("Cast");
    }

    // Update is called once per frame
    void Update()
    {
        // Adding the time since the previous frame to the time since creation
        timeSinceCreation += Time.deltaTime;

        // If the time since creation has exceeded the lifetime, the spell is destroyed
        if (timeSinceCreation > lifeTime && state != FireSpellState.Casting) Destroy(gameObject);

        // Curving towards the mouse if the fireball is thrown
        if (castByPlayer && state == FireSpellState.Thrown) CurveTowardMouse();

        // Enabling / disabling the collider based on whether the fireball is thrown
        fireCollider.enabled = (state == FireSpellState.Thrown);
    }

    // Calls when gameObject collides with another object
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Explode();
    }

    // Calls when the gameObject enters a trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Exploding if the fireball is hit by the player's sword and was not cast by player
        if (!castByPlayer && collision.CompareTag("Player Damager")) Explode();
    }

    public void Throw(Collider2D casterCollider, Vector2? direction = null)
    {
        // Destroying the fireball if it has not existed for longer than the minimum cast time
        if (castByPlayer && timeSinceCreation <= 1 / castSpeed)
        {
            // Destroying and triggering the Un-Cast animation
            Destroy(gameObject, castSpeed);
            if (animator != null) animator.SetTrigger("Un-Cast");

            // Returning from the function to prevent setting the state of the fireball
            return;
        }
        
        // Adding a force to the fireball to throw it
        state = FireSpellState.Thrown;
        if (rb == null) rb = GetComponent<Rigidbody2D>();

        // Calculating the angle towards the mouse needed to cast the fireball if the fireball was cast by the player
        Vector2 towardsMouse = Vector2.zero;
        if (castByPlayer)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            towardsMouse = (mousePosition - transform.position).normalized;
        }

        // Adding a force towards the mouse/position
        if (direction == null) direction = transform.up;
        rb.AddForce((castByPlayer ? towardsMouse : (Vector2) direction) * speed, ForceMode2D.Impulse);
        
        // Increasing the lifetime of the fireball to prevent despawning if it is held for a long period of time
        lifeTime += timeSinceCreation;

        // Setting the fireball to ignore collisions with the caster
        // THIS CODE SHOULD BE REWORKED TO ACCOUNT FOR WHEN A COLLIDER IS NULL
        Physics2D.IgnoreCollision(casterCollider, GetComponent<Collider2D>());
    }

    // Handles the fireball explosion
    public void Explode()
    {
        Destroy(gameObject);
    }

    // Function to curve towards the mouse
    void CurveTowardMouse() 
    {
        // Getting the mouse position as a world position
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Calculating the vector direction to point towards the mouse
        Vector2 towardsMouse = mousePosition - transform.position;
        towardsMouse = towardsMouse.normalized;

        // Calculating the distance between the points
        float distanceToMouse = Vector3.Distance(playerPosition, transform.position);

        // Adding a force to the fireball
        rb.AddForce(towardsMouse * curveTowardsMouseForce * (1 / distanceToMouse));
    }
}
